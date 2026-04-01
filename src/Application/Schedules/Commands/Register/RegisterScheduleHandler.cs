using Application.Abstractions.Extensions;
using Application.Abstractions.Interfaces;
using Domain.Club;
using Domain.Club.Entities;
using Domain.User;
using MediatR;
using SharedKernel;
using DayOfWeek = Domain.Club.Enum.DayOfWeek;

namespace Application.Schedules.Commands.Register;

internal sealed class RegisterScheduleHandler : IRequestHandler<RegisterSchedule, Result<int>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClubMemberQueries _clubMemberQueries;
    private readonly IUserContext _userContext;
    private readonly IClubRepository _clubRepository;

    public RegisterScheduleHandler(IUnitOfWork unitOfWork, IClubMemberQueries clubMemberQueries, IUserContext userContext,  IClubRepository clubRepository)
    {
        _unitOfWork = unitOfWork;
        _clubMemberQueries = clubMemberQueries;
        _userContext = userContext;
        _clubRepository = clubRepository;
    }

    public async Task<Result<int>> Handle(RegisterSchedule request, CancellationToken cancellationToken)
    {
        if (!_userContext.IsAnyAdmin())
        {
            return Result.Failure<int>(UserErrors.Forbidden);
        }
        
        if (!_userContext.IsOnlySuperadmin())
        {
            var hasPermission = await _clubMemberQueries.IsAdminInClub(request.ClubId, _userContext.UserId, cancellationToken);

            if (!hasPermission)
            {
                return Result.Failure<int>(ClubErrors.Forbidden);
            }
        }
        
        var club = await _clubRepository.GetClubById(request.ClubId, cancellationToken);
        if (club is null)
        {
            return Result.Failure<int>(ClubErrors.NotFound(request.ClubId));
        }
        
        if (!Enum.TryParse<DayOfWeek>(request.DayOfWeek, ignoreCase: true, out var dayOfWeek))
        {
            return Result.Failure<int>(ScheduleErrors.InvalidType);
        }

        var scheduleResult = club.AddSchedule(dayOfWeek, request.OpeningTime, request.ClosingTime);
        if (scheduleResult.IsFailure)
        {
            return Result.Failure<int>(scheduleResult.Error);
        }
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(scheduleResult.Value.Id);
    }
}
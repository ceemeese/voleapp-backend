using Application.Abstractions.Extensions;
using Application.Abstractions.Interfaces;
using Domain.Club;
using Domain.User;
using MediatR;
using SharedKernel;

namespace Application.Schedules.Commands.Update;

internal sealed class UpdateScheduleHandler : IRequestHandler<UpdateSchedule, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClubRepository _clubRepository;
    private readonly IUserContext _userContext;
    private readonly IClubMemberQueries _clubMemberQueries;


    public UpdateScheduleHandler(IUnitOfWork unitOfWork, IClubRepository clubRepository,  IUserContext userContext,  IClubMemberQueries clubMemberQueries)
    {
        _unitOfWork = unitOfWork;
        _clubRepository = clubRepository;
        _userContext = userContext;
        _clubMemberQueries = clubMemberQueries;
    }

    public async Task<Result> Handle(UpdateSchedule request, CancellationToken cancellationToken)
    {
        if (!_userContext.IsAnyAdmin())
        {
            return Result.Failure(UserErrors.Forbidden);
        }
        
        if (!_userContext.IsOnlySuperadmin())
        {
            var hasPermission = await _clubMemberQueries.IsAdminInClub(request.ClubId, _userContext.UserId, cancellationToken);

            if (!hasPermission)
            {
                return Result.Failure(ClubErrors.Forbidden);
            }
        }
        
        var club = await _clubRepository.GetClubById(request.ClubId, cancellationToken);
        if (club is null)
        {
            return Result.Failure(ClubErrors.NotFound(request.ClubId));
        }

        var scheduleResult = club.UpdateScheduleTime(request.ScheduleId, request.OpeningTime, request.ClosingTime);
        if (scheduleResult.IsFailure)
        {
            return Result.Failure(scheduleResult.Error);
        }
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(Unit.Value);
    }
}
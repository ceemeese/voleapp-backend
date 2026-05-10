using Application.Abstractions.DTO.Schedule;
using Application.Abstractions.Extensions;
using Application.Abstractions.Interfaces;
using AutoMapper;
using Domain.Club;
using Domain.Club.Entities;
using Domain.User;
using MediatR;
using SharedKernel;
using DayOfWeek = Domain.Club.Enum.DayOfWeek;

namespace Application.Schedules.Commands.Register;

internal sealed class RegisterScheduleHandler : IRequestHandler<RegisterSchedule, Result<ScheduleResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClubMemberQueries _clubMemberQueries;
    private readonly IUserContext _userContext;
    private readonly IClubRepository _clubRepository;
    private readonly IMapper _mapper;

    public RegisterScheduleHandler(IUnitOfWork unitOfWork, IClubMemberQueries clubMemberQueries, IUserContext userContext,  IClubRepository clubRepository, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _clubMemberQueries = clubMemberQueries;
        _userContext = userContext;
        _clubRepository = clubRepository;
        _mapper = mapper;
    }

    public async Task<Result<ScheduleResponse>> Handle(RegisterSchedule request, CancellationToken cancellationToken)
    {
        if (!_userContext.IsAnyAdmin())
        {
            return Result.Failure<ScheduleResponse>(UserErrors.Forbidden);
        }
        
        if (!_userContext.IsOnlySuperadmin())
        {
            var hasPermission = await _clubMemberQueries.IsAdminInClub(request.ClubId, _userContext.UserId, cancellationToken);

            if (!hasPermission)
            {
                return Result.Failure<ScheduleResponse>(ClubErrors.Forbidden);
            }
        }
        
        var club = await _clubRepository.GetClubByIdAsync(request.ClubId, cancellationToken);
        if (club is null)
        {
            return Result.Failure<ScheduleResponse>(ClubErrors.NotFound(request.ClubId));
        }
        
        if (!Enum.TryParse<DayOfWeek>(request.DayOfWeek, ignoreCase: true, out var dayOfWeek))
        {
            return Result.Failure<ScheduleResponse>(ScheduleErrors.InvalidType);
        }

        var scheduleResult = club.AddSchedule(dayOfWeek, request.OpeningTime, request.ClosingTime);
        if (scheduleResult.IsFailure)
        {
            return Result.Failure<ScheduleResponse>(scheduleResult.Error);
        }
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        var scheduleMapped = _mapper.Map<ScheduleResponse>(scheduleResult.Value);
        return Result.Success(scheduleMapped);
    }
}
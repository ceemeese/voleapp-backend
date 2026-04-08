using Application.Abstractions.DTO.Schedule;
using Application.Abstractions.Extensions;
using Application.Abstractions.Interfaces;
using AutoMapper;
using Domain.Club;
using Domain.User;
using MediatR;
using SharedKernel;

namespace Application.Schedules.Commands.Update;

internal sealed class UpdateScheduleHandler : IRequestHandler<UpdateSchedule, Result<ScheduleResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClubRepository _clubRepository;
    private readonly IUserContext _userContext;
    private readonly IClubMemberQueries _clubMemberQueries;
    private readonly IMapper _mapper;


    public UpdateScheduleHandler(IUnitOfWork unitOfWork, IClubRepository clubRepository,  IUserContext userContext,  IClubMemberQueries clubMemberQueries, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _clubRepository = clubRepository;
        _userContext = userContext;
        _clubMemberQueries = clubMemberQueries;
        _mapper = mapper;
    }

    public async Task<Result<ScheduleResponse>> Handle(UpdateSchedule request, CancellationToken cancellationToken)
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
        
        var club = await _clubRepository.GetClubById(request.ClubId, cancellationToken);
        if (club is null)
        {
            return Result.Failure<ScheduleResponse>(ClubErrors.NotFound(request.ClubId));
        }

        var scheduleResult = club.UpdateScheduleTime(request.ScheduleId, request.OpeningTime, request.ClosingTime);
        if (scheduleResult.IsFailure)
        {
            return Result.Failure<ScheduleResponse>(scheduleResult.Error);
        }
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        var scheduleMapped = _mapper.Map<ScheduleResponse>(scheduleResult.Value);
        return Result.Success(scheduleMapped);
    }
}
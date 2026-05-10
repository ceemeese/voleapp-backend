using Application.Abstractions.Extensions;
using Application.Abstractions.Interfaces;
using Domain.Club;
using Domain.User;
using MediatR;
using SharedKernel;

namespace Application.Schedules.Commands.Toggle;

internal sealed class ToggleSheduleHandler : IRequestHandler<ToggleSchedule, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClubMemberQueries _clubMemberQueries;
    private readonly IUserContext _userContext;
    private readonly IClubRepository _clubRepository;

    public ToggleSheduleHandler(IUnitOfWork unitOfWork, IClubMemberQueries clubMemberQueries, IUserContext userContext, IClubRepository clubRepository)
    {
        _unitOfWork = unitOfWork;
        _clubMemberQueries = clubMemberQueries;
        _userContext = userContext;
        _clubRepository = clubRepository;
    }

    public async Task<Result> Handle(ToggleSchedule request, CancellationToken cancellationToken)
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
        
        var club = await _clubRepository.GetClubByIdAsync(request.ClubId, cancellationToken);
        if (club is null)
        {
            return Result.Failure(ClubErrors.NotFound(request.ClubId));
        }
        
        var scheduleResult = club.SwitchOpeningStatus(request.ScheduleId);
        if (scheduleResult.IsFailure)
        {
            return Result.Failure(scheduleResult.Error);
        }
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(Unit.Value);
    }
}
using Application.Abstractions.Extensions;
using Application.Abstractions.Interfaces;
using Domain.Club;
using Domain.Court;
using Domain.User;
using MediatR;
using SharedKernel;

namespace Application.CourtEvents.Commands.Update;

internal sealed class UpdateCourtEventHandler : IRequestHandler<UpdateCourtEvent, Result>
{
    private readonly ICourtRepository _courtRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClubMemberQueries _clubMemberQueries;
    private readonly IUserContext _userContext;

    public UpdateCourtEventHandler(ICourtRepository courtRepository, IUnitOfWork unitOfWork,
        IClubMemberQueries clubMemberQueries, IUserContext userContext)
    {
        _courtRepository = courtRepository;
        _unitOfWork = unitOfWork;
        _clubMemberQueries = clubMemberQueries;
        _userContext = userContext;
    }

    public async Task<Result> Handle(UpdateCourtEvent request, CancellationToken cancellationToken)
    {
        if (!_userContext.IsAnyAdmin())
        {
            return Result.Failure(UserErrors.Forbidden);
        }
        
        var court = await _courtRepository.GetCourtWithEventsByDateRangeAsync(request.CourtId,request.StartTime, cancellationToken);
        if (court is null)
        {
            return Result.Failure(CourtErrors.NotFound(request.CourtId));
        }
        
        if (!_userContext.IsOnlySuperadmin())
        {
            var hasPermission = await _clubMemberQueries.IsAdminInClub(court.ClubId, _userContext.UserId, cancellationToken);

            if (!hasPermission)
            {
                return Result.Failure(ClubErrors.Forbidden);
            }
        }
        
        var courtEventResult = court.UpdateEvent(request.EventId, request.StartTime, request.EndTime, request.EventName, request.Description);
        if (courtEventResult.IsFailure)
        {
            return Result.Failure(courtEventResult.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(Unit.Value);
    }
}
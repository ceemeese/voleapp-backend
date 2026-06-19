using Application.Abstractions.Extensions;
using Application.Abstractions.Interfaces;
using Domain.Club;
using Domain.Court;
using Domain.User;
using MediatR;
using SharedKernel;

namespace Application.CourtEvents.Commands.Delete;

internal sealed class DeleteCourtEventHandler : IRequestHandler<DeleteCourtEvent, Result>
{
    private readonly IClubMemberQueries _clubMemberQueries;
    private readonly IUserContext _userContext;
    private readonly ICourtRepository _courtRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCourtEventHandler(IClubMemberQueries clubMemberQueries,  IUserContext userContext,  ICourtRepository courtRepository,  IUnitOfWork unitOfWork)
    {
        _clubMemberQueries = clubMemberQueries;
        _userContext = userContext;
        _courtRepository = courtRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteCourtEvent request, CancellationToken cancellationToken)
    {
        if (!_userContext.IsAnyAdmin())
        {
            return Result.Failure(UserErrors.Forbidden);
        }

        var court = await _courtRepository.GetCourtWithEventByIdAsync(request.CourtId, request.EventId, cancellationToken);
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
        var deleteEventResult = court.DeleteEvent(request.EventId);
        if (deleteEventResult.IsFailure)
        {
            return Result.Failure(deleteEventResult.Error);
        }
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(Unit.Value);
    }
}
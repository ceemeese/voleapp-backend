using Application.Abstractions.Extensions;
using Application.Abstractions.Interfaces;
using Domain.Club.Entities;
using Domain.Court;
using Domain.User;
using MediatR;
using SharedKernel;

namespace Application.Courts.Commands.Deactivate;

internal sealed class DeactivateCourtHandler : IRequestHandler<DeactivateCourt, Result>
{
    private readonly ICourtRepository _courtRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly IClubMemberQueries _clubMemberQueries;
    
    public DeactivateCourtHandler(ICourtRepository courtRepository, IUnitOfWork unitOfWork, IUserContext userContext,  IClubMemberQueries clubMemberQueries)
    {
        _courtRepository = courtRepository;
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _clubMemberQueries = clubMemberQueries;
    }

    public async Task<Result> Handle(DeactivateCourt request, CancellationToken cancellationToken)
    {
        if (!_userContext.IsAnyAdmin())
        {
            return Result.Failure(UserErrors.Forbidden);
        }
        
        var court = await _courtRepository.GetCourtByIdAsync(request.CourtId, cancellationToken);
        
        if (court is null)
        {
            return Result.Failure(CourtErrors.NotFound(request.CourtId));    
        }
        
        if (!_userContext.IsOnlySuperadmin())
        {
            var hasPermission = await _clubMemberQueries.IsAdminInClub(court.ClubId, _userContext.UserId, cancellationToken);

            if (!hasPermission)
            {
                return Result.Failure(ClubMemberErrors.Forbidden);
            }
        }
        
        court.Deactivate();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(Unit.Value);
    }
}
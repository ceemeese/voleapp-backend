using Application.Abstractions.Extensions;
using Application.Abstractions.Interfaces;
using Domain.Club;
using Domain.Club.Entities;
using Domain.User;
using MediatR;
using SharedKernel;

namespace Application.ClubMember.Commands.Activate;

internal sealed class ActivateClubMemberHandler : IRequestHandler<ActivateClubMember, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClubRepository _clubRepository;
    private readonly IUserContext _userContext;
    private readonly IClubMemberQueries _clubMemberQueries;

    public ActivateClubMemberHandler(IUnitOfWork unitOfWork, IClubRepository clubRepository, IUserContext userContext, IClubMemberQueries clubMemberQueries)
    {
        _unitOfWork = unitOfWork;
        _clubRepository = clubRepository;
        _userContext = userContext;
        _clubMemberQueries = clubMemberQueries;
    }

    public async Task<Result> Handle(ActivateClubMember request, CancellationToken cancellationToken)
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
        
        var club = await _clubRepository.GetClubWithMembersAsync(request.ClubId, cancellationToken);
        if (club is null)
        {
            return Result.Failure(ClubErrors.NotFound(request.ClubId));
        }

        var clubMemberResult = club.ActivateMember(request.UserId);
        if (clubMemberResult.IsFailure)
        {
            return Result.Failure(clubMemberResult.Error);
        }
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(Unit.Value);
    }
}
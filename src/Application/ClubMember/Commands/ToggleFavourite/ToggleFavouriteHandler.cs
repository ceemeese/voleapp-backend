using Application.Abstractions.Extensions;
using Application.Abstractions.Interfaces;
using Domain.Club;
using Domain.User;
using MediatR;
using SharedKernel;

namespace Application.ClubMember.Commands.ToggleFavourite;

internal sealed class ToggleFavouriteHandler : IRequestHandler<ToggleFavourite, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClubRepository _clubRepository;
    private readonly IUserContext _userContext;


    public ToggleFavouriteHandler(IUnitOfWork unitOfWork, IClubRepository clubRepository, IUserContext userContext)
    {
        _unitOfWork = unitOfWork;
        _clubRepository = clubRepository;
        _userContext = userContext;
    }

    public async Task<Result> Handle(ToggleFavourite request, CancellationToken cancellationToken)
    {
        if (!_userContext.IsOwnerOrSuperadmin(request.UserId))
        {
            return Result.Failure(UserErrors.Forbidden);
        }
        
        var club = await _clubRepository.GetClubWithMembers(request.ClubId, cancellationToken);
        if (club is null)
        {
            return Result.Failure(ClubErrors.NotFound(request.ClubId));
        }

        var clubMemberResult = club.ToggleMemberFavourite(request.UserId);
        if (clubMemberResult.IsFailure)
        {
            return Result.Failure(clubMemberResult.Error);
        }
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
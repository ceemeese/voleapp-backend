using Application.Abstractions.Extensions;
using Application.Abstractions.Interfaces;
using MediatR;
using SharedKernel;
using Domain.Club;

namespace Application.Clubs.Commands.Activate;

internal sealed class ActivateClubHandler : IRequestHandler<ActivateClub, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClubRepository _clubRepository;
    private readonly IUserContext _userContext;

    public ActivateClubHandler(IUnitOfWork unitOfWork, IClubRepository clubRepository, IUserContext userContext)
    {
        _unitOfWork = unitOfWork;
        _clubRepository = clubRepository;
        _userContext = userContext;
    }

    public async Task<Result> Handle(ActivateClub request, CancellationToken cancellationToken)
    {
        if (!_userContext.IsOnlySuperadmin())
        {
            return Result.Failure(ClubErrors.Forbidden);     
        }

        var club = await _clubRepository.GetClubByIdAsync(request.ClubId, cancellationToken);
          
        if (club is null)
        {
            return Result.Failure(ClubErrors.NotFound(request.ClubId));
        }
          
        club.Activate();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
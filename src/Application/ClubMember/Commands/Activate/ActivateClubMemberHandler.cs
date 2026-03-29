using Application.Abstractions.Interfaces;
using Domain.Club;
using Domain.Club.Entities;
using MediatR;
using SharedKernel;

namespace Application.ClubMember.Commands.Activate;

internal sealed class ActivateClubMemberHandler : IRequestHandler<ActivateClubMember, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClubRepository _clubRepository;

    public ActivateClubMemberHandler(IUnitOfWork unitOfWork, IClubRepository clubRepository)
    {
        _unitOfWork = unitOfWork;
        _clubRepository = clubRepository;
    }

    public async Task<Result> Handle(ActivateClubMember request, CancellationToken cancellationToken)
    {
        var club = await _clubRepository.GetClubWithMembers(request.ClubId, cancellationToken);
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
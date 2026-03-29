using Application.Abstractions.Interfaces;
using Domain.Club;
using Domain.Club.Entities;
using Domain.Club.Enum;
using MediatR;
using SharedKernel;

namespace Application.ClubMember.Commands.Update;

internal sealed class UpdateClubMemberHandler : IRequestHandler<UpdateClubMember, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClubRepository _clubRepository;

    public UpdateClubMemberHandler(IUnitOfWork unitOfWork, IClubRepository clubRepository)
    {
        _unitOfWork = unitOfWork;
        _clubRepository = clubRepository;
    }

    public async Task<Result> Handle(UpdateClubMember request, CancellationToken cancellationToken)
    {
        var club = await _clubRepository.GetClubWithMembers(request.ClubId, cancellationToken);
        if (club is null)
        {
            return Result.Failure(ClubErrors.NotFound(request.ClubId));
        }
        
        if (!Enum.TryParse<MemberRole>(request.Role, ignoreCase: true, out var memberRole))
        {
            return Result.Failure<int>(ClubMemberErrors.InvalidType);
        }

        var memberResult = club.UpdateMember(request.UserId, memberRole, request.IsMember, request.MembershipNumber);
        if (memberResult.IsFailure)
        {
            return Result.Failure(memberResult.Error);
        }
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
using Application.Abstractions.Interfaces;
using Domain.Club;
using Domain.Club.Entities;
using Domain.Club.Enum;
using MediatR;
using SharedKernel;

namespace Application.ClubMember.Commands.Register;

internal sealed class RegisterClubMemberHandler : IRequestHandler<RegisterClubMember, Result<int>>
{
    private readonly IClubRepository _clubRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterClubMemberHandler(IClubRepository clubRepository, IUnitOfWork unitOfWork)
    {
        _clubRepository = clubRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<int>> Handle(RegisterClubMember request, CancellationToken cancellationToken)
    {
        var club = await _clubRepository.GetClubById(request.ClubId, cancellationToken);
        if (club is null)
        {
            return Result.Failure<int>(ClubMemberErrors.NotFound(request.ClubId));
        }
        
        if (!Enum.TryParse<MemberRole>(request.Role, ignoreCase: true, out var memberRole))
        {
            return Result.Failure<int>(ClubMemberErrors.InvalidType);
        }

        var clubMemberResult = club.AddMember(request.UserId, memberRole);
        if (clubMemberResult.IsFailure)
        {
            return Result.Failure<int>(clubMemberResult.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success<int>(clubMemberResult.Value.Id);
    }
}
using Application.Abstractions.DTO.ClubMember;
using MediatR;
using SharedKernel;

namespace Application.ClubMember.Commands.Register;

public sealed record RegisterClubMember(Guid ClubId, Guid UserId, string Role) : IRequest<Result<ClubMemberCompleteResponse>>
{
}
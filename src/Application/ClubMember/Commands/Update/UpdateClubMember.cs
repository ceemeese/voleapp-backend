using Application.Abstractions.DTO.ClubMember;
using MediatR;
using SharedKernel;

namespace Application.ClubMember.Commands.Update;

public sealed record UpdateClubMember(Guid ClubId, Guid UserId, string Role, string? MembershipNumber, bool IsMember) : IRequest<Result>
{
}
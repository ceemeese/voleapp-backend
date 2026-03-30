using Application.Abstractions.DTO.ClubMember;
using MediatR;
using SharedKernel;

namespace Application.ClubMember.Queries.GetAllMembers;

public sealed record GetAllClubMembers(Guid ClubId, string? Search) : IRequest<Result<List<ClubMemberCompleteResponse>>>
{
}
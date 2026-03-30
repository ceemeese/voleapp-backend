using Application.Abstractions.DTO.ClubMember;
using MediatR;
using SharedKernel;

namespace Application.ClubMember.Queries.GetMemberDetail;

public sealed record GetMemberDetail(Guid ClubId, Guid UserId) : IRequest<Result<ClubMemberCompleteResponse>>
{
}
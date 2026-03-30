using MediatR;
using SharedKernel;

namespace Application.ClubMember.Commands.Delete;

public sealed record DeleteClubMember(Guid ClubId, Guid UserId) : IRequest<Result>
{
}
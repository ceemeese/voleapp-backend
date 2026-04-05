using MediatR;
using SharedKernel;

namespace Application.ClubMember.Commands.Deactivate;

public sealed record DeactivateClubMember(Guid ClubId, Guid UserId) : IRequest<Result>
{
}
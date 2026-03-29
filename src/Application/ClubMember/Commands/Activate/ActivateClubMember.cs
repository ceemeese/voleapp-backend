using MediatR;
using SharedKernel;

namespace Application.ClubMember.Commands.Activate;

public record ActivateClubMember(Guid ClubId, Guid UserId) : IRequest<Result>
{
}
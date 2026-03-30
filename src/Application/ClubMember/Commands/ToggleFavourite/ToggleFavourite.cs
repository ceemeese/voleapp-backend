using MediatR;
using SharedKernel;

namespace Application.ClubMember.Commands.ToggleFavourite;

public sealed record ToggleFavourite(Guid ClubId, Guid UserId) : IRequest<Result>
{
}
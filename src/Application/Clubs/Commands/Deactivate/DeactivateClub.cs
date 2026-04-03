using MediatR;
using SharedKernel;

namespace Application.Clubs.Commands.Deactivate;

public sealed record DeactivateClub(Guid ClubId) : IRequest<Result>
{
}
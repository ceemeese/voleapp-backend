using MediatR;
using SharedKernel;

namespace Application.Clubs.Commands;

public sealed record ActivateClub(Guid ClubId) : IRequest<Result>
{
}
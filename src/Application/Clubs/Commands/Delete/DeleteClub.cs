using MediatR;
using SharedKernel;

namespace Application.Clubs.Commands.Delete;

public sealed record DeleteClub(Guid ClubId) : IRequest<Result>
{
}
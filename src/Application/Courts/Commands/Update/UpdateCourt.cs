using MediatR;
using SharedKernel;

namespace Application.Courts.Commands.Update;

public sealed record UpdateCourt(Guid Id, Guid ClubId, string Name, decimal BasePrice) : IRequest<Result>
{
}
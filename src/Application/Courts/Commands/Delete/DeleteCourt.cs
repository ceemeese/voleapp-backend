using MediatR;
using SharedKernel;

namespace Application.Courts.Commands.Delete;

public sealed record DeleteCourt(Guid CourtId) : IRequest<Result>
{
}
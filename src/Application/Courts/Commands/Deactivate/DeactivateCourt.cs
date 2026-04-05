using MediatR;
using SharedKernel;

namespace Application.Courts.Commands.Deactivate;

public sealed record DeactivateCourt(Guid CourtId) : IRequest<Result>
{
}
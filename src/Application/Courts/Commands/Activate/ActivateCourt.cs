using MediatR;
using SharedKernel;

namespace Application.Courts.Commands.Activate;

public sealed record ActivateCourt(Guid CourtId) : IRequest<Result>
{
}
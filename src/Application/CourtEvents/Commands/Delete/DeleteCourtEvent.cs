using MediatR;
using SharedKernel;

namespace Application.CourtEvents.Commands.Delete;

public sealed record DeleteCourtEvent(Guid CourtId, int EventId, DateTime Date) : IRequest<Result>
{
}
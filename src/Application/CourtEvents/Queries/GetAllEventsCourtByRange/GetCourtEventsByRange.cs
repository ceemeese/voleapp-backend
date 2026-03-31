using Application.Abstractions.DTO.CourtEvent;
using MediatR;
using SharedKernel;

namespace Application.CourtEvents.Queries.GetAllEventsCourtByRange;

public sealed record GetCourtEventsByRange(Guid CourtId, DateTime StartDate, DateTime? EndDate) : IRequest<Result<List<CourtEventResponse>>>
{
}
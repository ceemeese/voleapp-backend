using Application.Abstractions.DTO.Court;
using MediatR;
using SharedKernel;

namespace Application.Courts.Queries.Availability;

public record AvailabilityCourts(string City, DateTime RequestDateTime, int DurationMinutes) : IRequest<Result<List<CourtGroupedAvailabilityResponse>>>
{
}

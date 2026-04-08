using Application.Abstractions.DTO.CourtEvent;
using MediatR;
using SharedKernel;

namespace Application.CourtEvents.Commands.Update;

public sealed record UpdateCourtEvent(
    Guid CourtId, 
    int EventId, 
    DateTime StartTime, 
    DateTime EndTime, 
    string EventName, 
    string? Description) : IRequest<Result<CourtEventResponse>>;
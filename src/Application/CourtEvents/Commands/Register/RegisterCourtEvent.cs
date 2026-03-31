using MediatR;
using SharedKernel;

namespace Application.CourtEvents.Commands.Register;

public sealed record RegisterCourtEvent(
    Guid CourtId,
    DateTime StartTime,
    DateTime EndTime,
    string EventName,
    string? Description) : IRequest<Result<int>>{};
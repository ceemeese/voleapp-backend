using Application.Abstractions.DTO.CourtEvent;
using MediatR;
using SharedKernel;

namespace Application.CourtEvents.Queries.GetEventById;

public sealed record GetEventById(Guid CourtId, int EventId) : IRequest<Result<CourtEventCompleteResponse>>;
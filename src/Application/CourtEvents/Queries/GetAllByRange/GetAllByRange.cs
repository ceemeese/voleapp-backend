using Application.Abstractions.DTO.CourtEvent;
using MediatR;
using SharedKernel;

namespace Application.CourtEvents.Queries.GetAllByRange;

public sealed record GetAllByRange(Guid ClubId, DateTime StartDate, DateTime? EndDate) : IRequest<Result<List<CourtEventCompleteResponse>>>
{
}
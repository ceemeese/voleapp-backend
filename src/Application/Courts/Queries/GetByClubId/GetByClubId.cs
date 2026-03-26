using Application.Abstractions.DTO.Court;
using MediatR;
using SharedKernel;

namespace Application.Courts.Queries.GetByClubId;

public sealed record GetByClubId(Guid ClubId) : IRequest<Result<List<CourtSummaryResponse>>>
{
}
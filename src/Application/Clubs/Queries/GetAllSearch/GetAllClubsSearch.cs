using Application.Abstractions.DTO.Club;
using MediatR;
using SharedKernel;

namespace Application.Clubs.Queries.GetAllSearch;

public sealed record GetAllClubsSearch(string? Name = null) : IRequest<Result<List<ClubSummaryResponse>>>
{
}
using Application.Abstractions.DTO;
using MediatR;
using SharedKernel;

namespace Application.Clubs.Queries.GetAllSearch;

public sealed record GetAllClubsSearch(string? Name = null) : IRequest<Result<List<ClubSummaryResponse>>>
{
}
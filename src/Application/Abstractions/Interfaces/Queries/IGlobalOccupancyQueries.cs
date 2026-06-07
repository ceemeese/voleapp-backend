using Application.Abstractions.DTO.Dashboard;

namespace Application.Abstractions.Interfaces.Queries;

public interface IGlobalOccupancyQueries
{
    Task<GlobalOccupancyResponse> GetGlocalOccupancyStatsAsync(DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken);

}
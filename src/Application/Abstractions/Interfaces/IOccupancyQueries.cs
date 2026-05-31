using Application.Abstractions.DTO.Dashboard;

namespace Application.Abstractions.Interfaces;

public interface IOccupancyQueries
{
    Task<OccupancyResponse> GetOccupancyStatsAsync(Guid clubId, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken);
}
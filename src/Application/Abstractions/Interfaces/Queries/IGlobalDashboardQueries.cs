using Application.Abstractions.DTO.Dashboard;

namespace Application.Abstractions.Interfaces.Queries;

public interface IGlobalDashboardQueries
{
    Task<GlobalDashboardResponse> GetGlobalDashboardStatsAsync(CancellationToken cancellationToken);
}
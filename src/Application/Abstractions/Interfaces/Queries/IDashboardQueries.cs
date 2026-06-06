using Application.Abstractions.DTO.Dashboard;

namespace Application.Abstractions.Interfaces.Queries;

public interface IDashboardQueries
{
    Task<ClubDashboardResponse> GetDashboardStatsAsync(Guid clubId, CancellationToken cancellationToken);
}
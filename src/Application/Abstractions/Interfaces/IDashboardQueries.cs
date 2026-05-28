using Application.Abstractions.DTO.Dashboard;

namespace Application.Abstractions.Interfaces;

public interface IDashboardQueries
{
    Task<ClubDashboardResponse> GetDashboardStatsAsync(Guid clubId, CancellationToken cancellationToken);
}
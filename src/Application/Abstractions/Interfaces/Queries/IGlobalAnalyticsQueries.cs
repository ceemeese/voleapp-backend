using Application.Abstractions.DTO.Dashboard;

namespace Application.Abstractions.Interfaces.Queries;

public interface IGlobalAnalyticsQueries
{
    Task<GlobalAnalysisResponse> GetGlobalAnalyticsStatsAsync(DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken);
}
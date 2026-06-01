using Application.Abstractions.DTO.Dashboard;

namespace Application.Abstractions.Interfaces;

public interface IAnalyticsQueries
{
    Task<AnalysisResponse> GetAnalysisStatsAsync(Guid clubId, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken);
}
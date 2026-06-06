using Application.Abstractions.DTO.Dashboard;

namespace Application.Abstractions.Interfaces.Queries;

public interface IAnalyticsQueries
{
    Task<AnalysisResponse> GetAnalysisStatsAsync(Guid clubId, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken);
}
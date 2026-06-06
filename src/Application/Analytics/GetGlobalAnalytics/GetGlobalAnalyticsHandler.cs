using Application.Abstractions.DTO.Dashboard;
using Application.Abstractions.Extensions;
using Application.Abstractions.Interfaces;
using Application.Abstractions.Interfaces.Queries;
using Domain.User;
using MediatR;
using SharedKernel;

namespace Application.Analytics.GetGlobalAnalytics;

internal sealed class GetGlobalAnalyticsHandler : IRequestHandler<GetGlobalAnalytics, Result<GlobalAnalysisResponse>>
{
    private readonly IUserContext _userContext;
    private readonly IGlobalAnalyticsQueries _globalAnalyticsQueries;
 
    public GetGlobalAnalyticsHandler(IUserContext userContext,  IGlobalAnalyticsQueries globalAnalyticsQueries)
    {
        _userContext = userContext;
        _globalAnalyticsQueries = globalAnalyticsQueries;
    }

    public async Task<Result<GlobalAnalysisResponse>> Handle(GetGlobalAnalytics request, CancellationToken cancellationToken)
    {
        if (!_userContext.IsOnlySuperadmin())
        {
            return Result.Failure<GlobalAnalysisResponse>(UserErrors.Forbidden);
        }
        
        var startDate = new DateOnly(request.Year, request.Month, 1);
        var daysInMonth = DateTime.DaysInMonth(request.Year, request.Month);
        var endDate = new DateOnly(request.Year, request.Month, daysInMonth);
        
        var stats = await _globalAnalyticsQueries.GetGlobalAnalyticsStatsAsync(startDate, endDate, cancellationToken);

        return Result.Success(stats);        
    }
}
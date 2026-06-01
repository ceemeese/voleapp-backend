using Application.Abstractions.DTO.Dashboard;
using Application.Abstractions.Extensions;
using Application.Abstractions.Interfaces;
using Domain.Club;
using MediatR;
using SharedKernel;

namespace Application.Analytics.GetAnalytics;

internal sealed class GetAnalyticsHandler : IRequestHandler<GetAnalytics, Result<AnalysisResponse>>
{
    private readonly IUserContext _userContext;
    private readonly IAnalyticsQueries _analyticsQueries;
    private readonly IClubMemberQueries _clubMemberQueries;

    public GetAnalyticsHandler(IAnalyticsQueries analyticsQueries, IUserContext userContext,  IClubMemberQueries clubMemberQueries)
    {
        _analyticsQueries = analyticsQueries;
        _userContext = userContext;
        _clubMemberQueries = clubMemberQueries;
    }

    public async Task<Result<AnalysisResponse>> Handle(GetAnalytics request, CancellationToken cancellationToken)
    {
        if (!_userContext.IsOnlySuperadmin())
        {
            var hasPermission = await _clubMemberQueries.IsAdminInClub(request.ClubId, _userContext.UserId, cancellationToken);

            if (!hasPermission)
            {
                return Result.Failure<AnalysisResponse>(ClubErrors.Forbidden);
            }
        }
        
        var startDate = new DateOnly(request.Year, request.Month, 1);
        var daysInMonth = DateTime.DaysInMonth(request.Year, request.Month);
        var endDate = new DateOnly(request.Year, request.Month, daysInMonth);
        
        var stats = await _analyticsQueries.GetAnalysisStatsAsync(request.ClubId, startDate, endDate, cancellationToken);

        return Result.Success(stats);
    }
}
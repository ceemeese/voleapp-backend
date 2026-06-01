using Application.Abstractions.DTO.Dashboard;
using Application.Abstractions.Extensions;
using Application.Abstractions.Interfaces;
using Domain.Club;
using MediatR;
using SharedKernel;

namespace Application.Analytics.GetDashboard;

internal sealed class GetDashboardHandler : IRequestHandler<GetDashboard, Result<ClubDashboardResponse>>
{
    private readonly IDashboardQueries _dashboardQueries;
    private readonly IUserContext _userContext;
    private readonly IClubMemberQueries _clubMemberQueries;

    public GetDashboardHandler(IDashboardQueries dashboardQueries, IUserContext userContext,  IClubMemberQueries clubMemberQueries)
    {
        _dashboardQueries = dashboardQueries;
        _userContext = userContext;
        _clubMemberQueries = clubMemberQueries;
    }

    public async Task<Result<ClubDashboardResponse>> Handle(GetDashboard request, CancellationToken cancellationToken)
    {
        if (!_userContext.IsOnlySuperadmin())
        {
            var hasPermission = await _clubMemberQueries.IsAdminInClub(request.ClubId, _userContext.UserId, cancellationToken);

            if (!hasPermission)
            {
                return Result.Failure<ClubDashboardResponse>(ClubErrors.Forbidden);
            }
        }
        
        var stats = await _dashboardQueries.GetDashboardStatsAsync(request.ClubId, cancellationToken);

        return Result.Success(stats);
    }
}
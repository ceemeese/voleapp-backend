using Application.Abstractions.DTO.Dashboard;
using Application.Abstractions.Extensions;
using Application.Abstractions.Interfaces;
using Application.Abstractions.Interfaces.Queries;
using Domain.User;
using MediatR;
using SharedKernel;

namespace Application.Analytics.GetGlobalDashboard;

internal sealed class GetGlobalDashboardHandler : IRequestHandler<GetGlobalDashboard, Result<GlobalDashboardResponse>>
{
    private readonly IUserContext _userContext;
    private readonly IGlobalDashboardQueries _globalDashboardQueries;
    
    public GetGlobalDashboardHandler(IUserContext userContext, IGlobalDashboardQueries globalDashboardQueries)
    {
        _userContext = userContext;
        _globalDashboardQueries = globalDashboardQueries;
    }


    public async Task<Result<GlobalDashboardResponse>> Handle(GetGlobalDashboard request, CancellationToken cancellationToken)
    {
        if (!_userContext.IsOnlySuperadmin())
        {
            return Result.Failure<GlobalDashboardResponse>(UserErrors.Forbidden);
        }

        var stats = await _globalDashboardQueries.GetGlobalDashboardStatsAsync(cancellationToken);
        return Result.Success(stats);
    }
}
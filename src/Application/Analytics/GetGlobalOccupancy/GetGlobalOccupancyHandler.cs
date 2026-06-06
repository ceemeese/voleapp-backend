using Application.Abstractions.DTO.Dashboard;
using Application.Abstractions.Extensions;
using Application.Abstractions.Interfaces;
using Application.Abstractions.Interfaces.Queries;
using Domain.User;
using MediatR;
using SharedKernel;

namespace Application.Analytics.GetGlobalOccupancy;

internal sealed class GetGlobalOccupancyHandler : IRequestHandler<GetGlobalOccupancy, Result<GlobalOccupancyResponse>>
{
    private readonly IUserContext _userContext;
    private readonly IGlobalOccupancyQueries _globalOccupancyQueries;
    
    public GetGlobalOccupancyHandler(IUserContext userContext,  IGlobalOccupancyQueries globalOccupancyQueries)
    {
        _userContext = userContext;
        _globalOccupancyQueries = globalOccupancyQueries;
    }

    public async Task<Result<GlobalOccupancyResponse>> Handle(GetGlobalOccupancy request, CancellationToken cancellationToken)
    {
        if (!_userContext.IsOnlySuperadmin())
        {
            return Result.Failure<GlobalOccupancyResponse>(UserErrors.Forbidden);
        }
        
        var startDate = new DateOnly(request.Year, request.Month, 1);
        var daysInMonth = DateTime.DaysInMonth(request.Year, request.Month);
        var endDate = new DateOnly(request.Year, request.Month, daysInMonth);
        
        var stats = await _globalOccupancyQueries.GetGlocalOccupancyStatsAsync(startDate, endDate, cancellationToken);

        return Result.Success(stats);
    }
    
}
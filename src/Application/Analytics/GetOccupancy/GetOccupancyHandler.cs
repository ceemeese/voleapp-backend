using Application.Abstractions.DTO.Dashboard;
using Application.Abstractions.Extensions;
using Application.Abstractions.Interfaces;
using Domain.Club;
using MediatR;
using SharedKernel;

namespace Application.Analytics.GetOccupancy;

internal sealed class GetOccupancyHandler : IRequestHandler<GetOccupancy, Result<OccupancyResponse>>
{
    private readonly IUserContext _userContext;
    private readonly IClubMemberQueries _clubMemberQueries;
    private readonly IOccupancyQueries _occupancyQueries;

    public GetOccupancyHandler(IUserContext userContext,  IClubMemberQueries clubMemberQueries,  IOccupancyQueries occupancyQueries)
    {
        _userContext = userContext;
        _clubMemberQueries = clubMemberQueries;
        _occupancyQueries = occupancyQueries;
    }

    public async Task<Result<OccupancyResponse>> Handle(GetOccupancy request, CancellationToken cancellationToken)
    {
        if (!_userContext.IsOnlySuperadmin())
        {
            var hasPermission = await _clubMemberQueries.IsAdminInClub(request.ClubId, _userContext.UserId, cancellationToken);

            if (!hasPermission)
            {
                return Result.Failure<OccupancyResponse>(ClubErrors.Forbidden);
            }
        }
        
        var startDate = new DateOnly(request.Year, request.Month, 1);
        var daysInMonth = DateTime.DaysInMonth(request.Year, request.Month);
        var endDate = new DateOnly(request.Year, request.Month, daysInMonth);
        
        var stats = await _occupancyQueries.GetOccupancyStatsAsync(request.ClubId, startDate, endDate, cancellationToken);

        return Result.Success(stats);
    }
}
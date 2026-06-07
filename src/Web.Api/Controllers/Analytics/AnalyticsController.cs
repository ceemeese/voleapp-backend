using Application.Analytics.GetAnalytics;
using Application.Analytics.GetDashboard;
using Application.Analytics.GetGlobalAnalytics;
using Application.Analytics.GetGlobalDashboard;
using Application.Analytics.GetGlobalOccupancy;
using Application.Analytics.GetOccupancy;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Web.Api.Infrastructure;

namespace Web.Api.Controllers.Analytics;

[ApiController]
public class AnalyticsController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public AnalyticsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [AuthorizeAdmins]
    [HttpGet("api/clubs/{clubId:guid}/dashboard")]
    public async Task<IActionResult> GetDashboardStats([FromRoute] Guid clubId, CancellationToken cancellationToken)
    {
        var query =  new GetDashboard(clubId);
        
        var analyticsResult = await _mediator.Send(query, cancellationToken);
        
        return analyticsResult.IsSuccess 
            ? Ok(analyticsResult.Value) 
            : CustomResults.Problem(analyticsResult);
    }
    
    [AuthorizeAdmins]
    [HttpGet("api/clubs/{clubId:guid}/analytics")]
    public async Task<IActionResult> GetAnalyticsStats([FromRoute] Guid clubId, [FromQuery] int year, [FromQuery] int month, CancellationToken cancellationToken)
    {
        var query =  new GetAnalytics(clubId, year, month);
        
        var analyticsResult = await _mediator.Send(query, cancellationToken);
        
        return analyticsResult.IsSuccess 
            ? Ok(analyticsResult.Value) 
            : CustomResults.Problem(analyticsResult);
    }
    
    [AuthorizeAdmins]
    [HttpGet("api/clubs/{clubId:guid}/occupancy")]
    public async Task<IActionResult> GetOccupancyStats([FromRoute] Guid clubId, [FromQuery] int year, [FromQuery] int month, CancellationToken cancellationToken)
    {
        var query =  new GetOccupancy(clubId, year, month);
        
        var occupancyResult = await _mediator.Send(query, cancellationToken);
        
        return occupancyResult.IsSuccess 
            ? Ok(occupancyResult.Value) 
            : CustomResults.Problem(occupancyResult);
    }
    
    [AuthorizeSuperAdmin]
    [HttpGet("api/management/dashboard")]
    public async Task<IActionResult> GetGlobalStats(CancellationToken cancellationToken)
    {
        var globalResult = await _mediator.Send(new GetGlobalDashboard(), cancellationToken);
        
        return globalResult.IsSuccess 
            ? Ok(globalResult.Value) 
            : CustomResults.Problem(globalResult);
    }
    
    [AuthorizeSuperAdmin]
    [HttpGet("api/management/analytics")]
    public async Task<IActionResult> GetGlobalAnalysisStats([FromQuery] int year, [FromQuery] int month, CancellationToken cancellationToken)
    {
        var query =  new GetGlobalAnalytics(year, month);
        
        var globalAnalyticsResult = await _mediator.Send(query, cancellationToken);
        
        return globalAnalyticsResult.IsSuccess 
            ? Ok(globalAnalyticsResult.Value) 
            : CustomResults.Problem(globalAnalyticsResult);
    }
    
    [AuthorizeSuperAdmin]
    [HttpGet("api/management/occupancy")]
    public async Task<IActionResult> GetGlobalOccupancyStats([FromQuery] int year, [FromQuery] int month, CancellationToken cancellationToken)
    {
        var query =  new GetGlobalOccupancy(year, month);
        
        var globalOccupancyResult = await _mediator.Send(query, cancellationToken);
        
        return globalOccupancyResult.IsSuccess 
            ? Ok(globalOccupancyResult.Value) 
            : CustomResults.Problem(globalOccupancyResult);
    }
}
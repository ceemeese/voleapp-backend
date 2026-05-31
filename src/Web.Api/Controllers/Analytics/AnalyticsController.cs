using Application.Analytics.GetAnalytics;
using Application.Analytics.GetDashboard;
using Application.Analytics.GetOccupancy;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Web.Api.Infrastructure;

namespace Web.Api.Controllers.Analytics;

[Route("api/[controller]")]
[ApiController]
public class AnalyticsController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public AnalyticsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [AuthorizeAdmins]
    [HttpGet("dashboard/{clubId:guid}")]
    public async Task<IActionResult> GetDashboardStats([FromRoute] Guid clubId, CancellationToken cancellationToken)
    {
        var query =  new GetDashboard(clubId);
        
        var analyticsResult = await _mediator.Send(query, cancellationToken);
        
        return analyticsResult.IsSuccess 
            ? Ok(analyticsResult.Value) 
            : CustomResults.Problem(analyticsResult);
    }
    
    [AuthorizeAdmins]
    [HttpGet("analytics/{clubId:guid}/{year:int}/{month:int}")]
    public async Task<IActionResult> GetAnalyticsStats([FromRoute] Guid clubId, [FromRoute] int year, [FromRoute] int month, CancellationToken cancellationToken)
    {
        var query =  new GetAnalytics(clubId, year, month);
        
        var analyticsResult = await _mediator.Send(query, cancellationToken);
        
        return analyticsResult.IsSuccess 
            ? Ok(analyticsResult.Value) 
            : CustomResults.Problem(analyticsResult);
    }
    
    [AuthorizeAdmins]
    [HttpGet("occupancy/{clubId:guid}/{year:int}/{month:int}")]
    public async Task<IActionResult> GetOccupancyStats([FromRoute] Guid clubId, [FromRoute] int year, [FromRoute] int month, CancellationToken cancellationToken)
    {
        var query =  new GetOccupancy(clubId, year, month);
        
        var occupancyResult = await _mediator.Send(query, cancellationToken);
        
        return occupancyResult.IsSuccess 
            ? Ok(occupancyResult.Value) 
            : CustomResults.Problem(occupancyResult);
    }
}
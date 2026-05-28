using Application.Analytics.GetDashboard;
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
    public async Task<IActionResult> GetAdminContext([FromRoute] Guid clubId, CancellationToken cancellationToken)
    {
        var query =  new GetDashboard(clubId);
        
        var analyticsResult = await _mediator.Send(query, cancellationToken);
        
        return analyticsResult.IsSuccess 
            ? Ok(analyticsResult.Value) 
            : CustomResults.Problem(analyticsResult);
    }
}
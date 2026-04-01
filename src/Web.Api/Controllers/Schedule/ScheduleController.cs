using Application.Schedules.Queries.GetAll;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Api.Infrastructure;

namespace Web.Api.Controllers.Schedule;

[ApiController]
public sealed class ScheduleController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public ScheduleController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [Authorize]
    [HttpGet("api/clubs/{clubId:guid}/schedules")]
    public async Task<IActionResult> GetAll([FromRoute] Guid clubId, CancellationToken cancellationToken)
    {
        var scheduleResult = await _mediator.Send(new GetAllSchedule(clubId), cancellationToken);
        
        return scheduleResult.IsSuccess
            ? Ok(scheduleResult.Value) 
            : CustomResults.Problem(scheduleResult);
    }
}
using Application.Schedules.Commands.Register;
using Application.Schedules.Commands.Toggle;
using Application.Schedules.Commands.Update;
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
    
    [AuthorizeAdmins]
    [HttpPost("api/clubs/{clubId:guid}/schedules")]
    public async Task<IActionResult> Register([FromRoute] Guid clubId, [FromBody] RegisterScheduleRequest request, CancellationToken cancellationToken)
    {
        var command = new RegisterSchedule(clubId, request.DayOfWeek, request.OpeningTime, request.ClosingTime );
        var scheduleResult = await _mediator.Send(command, cancellationToken);
        
        return scheduleResult.IsSuccess
            ? Ok(scheduleResult.Value) 
            : CustomResults.Problem(scheduleResult);
    }
    
    [AuthorizeAdmins]
    [HttpPut("api/clubs/{clubId:guid}/schedules/{scheduleId:int}")]
    public async Task<IActionResult> Register([FromRoute] Guid clubId, [FromRoute] int scheduleId, [FromBody] UpdateScheduleRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateSchedule(clubId, scheduleId, request.OpeningTime, request.ClosingTime );
        var scheduleResult = await _mediator.Send(command, cancellationToken);
        
        return scheduleResult.IsSuccess
            ? NoContent()
            : CustomResults.Problem(scheduleResult);
    }
    
    [AuthorizeAdmins]
    [HttpPut("api/clubs/{clubId:guid}/schedules/{scheduleId:int}/toggle")]
    public async Task<IActionResult> ToggleSchedule([FromRoute] Guid clubId, [FromRoute] int scheduleId, CancellationToken cancellationToken)
    {
        var command = new ToggleSchedule(clubId, scheduleId);
        var scheduleResult = await _mediator.Send(command, cancellationToken);
        
        return scheduleResult.IsSuccess
            ? NoContent()
            : CustomResults.Problem(scheduleResult);
    }
}
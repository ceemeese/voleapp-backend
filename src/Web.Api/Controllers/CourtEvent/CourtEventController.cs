using Application.ClubMember.Queries.GetAllMembers;
using Application.CourtEvents.Commands.Delete;
using Application.CourtEvents.Commands.Register;
using Application.CourtEvents.Commands.Update;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Web.Api.Infrastructure;

namespace Web.Api.Controllers.CourtEvent;

[ApiController]
public sealed class CourtEventController : ControllerBase
{
    private readonly IMediator _mediator;

    public CourtEventController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [AuthorizeAdmins]
    [HttpPost("api/courts/{courtId:guid}/events")]
    public async Task<IActionResult> Register([FromRoute] Guid courtId, [FromBody] RegisterCourtEventRequest request, CancellationToken cancellationToken)
    {
        var command = new RegisterCourtEvent(
            courtId,
            request.StartTime,
            request.EndTime,
            request.EventName,
            request.Description
        );
        
        var eventResult = await _mediator.Send(command, cancellationToken);
        
        return eventResult.IsSuccess 
            ? Ok(eventResult.Value) 
            : CustomResults.Problem(eventResult);
    }
    
    [AuthorizeAdmins]
    [HttpPut("api/courts/{courtId:guid}/events/{eventId:int}")]
    public async Task<IActionResult> Update([FromRoute] Guid courtId, [FromRoute] int eventId, [FromBody] UpdateCourtEventRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateCourtEvent(
            courtId,
            eventId,
            request.StartTime,
            request.EndTime,
            request.EventName,
            request.Description
        );
        
        var eventResult = await _mediator.Send(command, cancellationToken);
        
        return eventResult.IsSuccess 
            ? NoContent() 
            : CustomResults.Problem(eventResult);
    }
    
    [AuthorizeAdmins]
    [HttpDelete("api/courts/{courtId:guid}/events/{eventId:int}")]
    public async Task<IActionResult> Delete([FromRoute] Guid courtId, [FromRoute] int eventId, [FromBody] DateTime date, CancellationToken cancellationToken)
    {
        var command = new DeleteCourtEvent(courtId, eventId, date);
        var eventResult = await _mediator.Send(command, cancellationToken);
        
        return eventResult.IsSuccess 
            ? NoContent() 
            : CustomResults.Problem(eventResult);
    }
}
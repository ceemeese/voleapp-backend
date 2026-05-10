using Application.Courts.Commands.Activate;
using Application.Courts.Commands.Deactivate;
using Application.Courts.Commands.Register;
using Application.Courts.Commands.Update;
using Application.Courts.Queries.Availability;
using Application.Courts.Queries.GetAll;
using Application.Courts.Queries.GetByClubId;
using Application.Courts.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Api.Infrastructure;

namespace Web.Api.Controllers.Court;

[ApiController]
public class CourtsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CourtsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [AllowAnonymous]
    [HttpGet("api/courts")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var courtResult = await _mediator.Send(new GetAllCourts(), cancellationToken);
        
        return courtResult.IsSuccess
            ? Ok(courtResult.Value)
            : CustomResults.Problem(courtResult);
    }
    
    [AllowAnonymous]
    [HttpGet("api/clubs/{clubId:guid}/courts")]
    public async Task<IActionResult> GetByClubId([FromRoute] Guid clubId, CancellationToken cancellationToken)
    {
        var courtResult = await _mediator.Send(new GetByClubId(clubId), cancellationToken);
        
        return courtResult.IsSuccess
            ? Ok(courtResult.Value)
            : CustomResults.Problem(courtResult);
    }
    
    [AllowAnonymous]
    [HttpGet("api/courts/{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var courtResult = await _mediator.Send(new GetCourtById(id), cancellationToken);
        
        return courtResult.IsSuccess
            ? Ok(courtResult.Value)
            : CustomResults.Problem(courtResult);
    }
    
    
    [AuthorizeAdmins]
    [HttpPost("api/clubs/{clubId}/courts")]
    public async Task<IActionResult>Register([FromRoute] Guid clubId, [FromBody] RegisterCourtRequest request)
    {
        var command = new RegisterCourt(
            clubId,
            request.Name,
            request.Type,
            request.BasePrice,
            request.IsActive
        );
        var courtResult = await _mediator.Send(command);

        return courtResult.IsSuccess 
            ? Ok(courtResult.Value) 
            : CustomResults.Problem(courtResult);
    }
    
    [AuthorizeAdmins]
    [HttpPut("api/courts/{id:guid}")]
    public async Task<IActionResult>Update([FromRoute] Guid id, [FromBody] UpdateCourtRequest request)
    {
        var command = new UpdateCourt(
            id,
            request.Name,
            request.BasePrice
        );
        var courtResult = await _mediator.Send(command);

        return courtResult.IsSuccess 
            ? NoContent()
            : CustomResults.Problem(courtResult);
    }
    
    [AuthorizeAdmins]
    [HttpPatch("api/courts/{id:guid}/deactivate")]
    public async Task<IActionResult>Deactivate([FromRoute] Guid id)
    {
        var courtResult = await _mediator.Send(new DeactivateCourt(id));

        return courtResult.IsSuccess 
            ? NoContent()
            : CustomResults.Problem(courtResult);
    }
    
    [AuthorizeAdmins]
    [HttpPatch("api/courts/{id:guid}/activate")]
    public async Task<IActionResult>Activate([FromRoute] Guid id)
    {
        var courtResult = await _mediator.Send(new ActivateCourt(id));

        return courtResult.IsSuccess 
            ? NoContent()
            : CustomResults.Problem(courtResult);
    }

    [Authorize]
    [HttpGet("api/courts/availability")]
    public async Task<IActionResult> GetAvailability([FromQuery] string city, [FromQuery] DateTime dateFilter, [FromQuery] int durationMinutes,
        CancellationToken cancellationToken)
    {
        var query = new AvailabilityCourts(city, dateFilter, durationMinutes);
        
        var courtResult = await _mediator.Send(query);
        return courtResult.IsSuccess
            ? Ok(courtResult.Value)
            : CustomResults.Problem(courtResult);
    }
    
}
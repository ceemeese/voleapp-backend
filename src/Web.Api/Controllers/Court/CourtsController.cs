using Application.Courts.Commands.Activate;
using Application.Courts.Commands.Delete;
using Application.Courts.Commands.Register;
using Application.Courts.Commands.Update;
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
    public async Task<IActionResult> GetByClubId(Guid clubId, CancellationToken cancellationToken)
    {
        var courtResult = await _mediator.Send(new GetByClubId(clubId), cancellationToken);
        
        return courtResult.IsSuccess
            ? Ok(courtResult.Value)
            : CustomResults.Problem(courtResult);
    }
    
    [AllowAnonymous]
    [HttpGet("api/courts/{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var courtResult = await _mediator.Send(new GetCourtById(id), cancellationToken);
        
        return courtResult.IsSuccess
            ? Ok(courtResult.Value)
            : CustomResults.Problem(courtResult);
    }
    
    
    [AuthorizeAdmins]
    [HttpPost("api/clubs/{clubId}/courts")]
    public async Task<IActionResult>Register(Guid clubId, [FromBody] RegisterCourtRequest request)
    {
        var command = new RegisterCourt(
            clubId,
            request.Name,
            request.CourtType,
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
    public async Task<IActionResult>Update(Guid id, [FromBody] UpdateCourtRequest request)
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
    [HttpDelete("api/courts/{id:guid}")]
    public async Task<IActionResult>Delete(Guid id)
    {
        var courtResult = await _mediator.Send(new DeleteCourt(id));

        return courtResult.IsSuccess 
            ? NoContent()
            : CustomResults.Problem(courtResult);
    }
    
    [AuthorizeAdmins]
    [HttpPatch("api/courts/{id:guid}/activate")]
    public async Task<IActionResult>Activate(Guid id)
    {
        var courtResult = await _mediator.Send(new ActivateCourt(id));

        return courtResult.IsSuccess 
            ? NoContent()
            : CustomResults.Problem(courtResult);
    }
    
}
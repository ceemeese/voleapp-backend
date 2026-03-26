using Application.Courts.Commands.Register;
using Application.Courts.Commands.Update;
using Application.Courts.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Api.Infrastructure;

namespace Web.Api.Controllers.Court;

[Route("api/[controller]")]
[ApiController]
public class CourtsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CourtsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var courtResult = await _mediator.Send(new GetAllCourts(), cancellationToken);
        
        return courtResult.IsSuccess
            ? Ok(courtResult.Value)
            : CustomResults.Problem(courtResult);
    }
    
    
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult>Register([FromBody] RegisterCourtRequest request)
    {
        var command = new RegisterCourt(
            request.ClubId,
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
    
    [AllowAnonymous]
    [HttpPut("/api/clubs/{clubId:guid}/courts/{id:guid}")]
    public async Task<IActionResult>Update(Guid id, Guid clubId,[FromBody] UpdateCourtRequest request)
    {
        var command = new UpdateCourt(
            id,
            clubId,
            request.Name,
            request.BasePrice
        );
        var courtResult = await _mediator.Send(command);

        return courtResult.IsSuccess 
            ? NoContent()
            : CustomResults.Problem(courtResult);
    }
    
}
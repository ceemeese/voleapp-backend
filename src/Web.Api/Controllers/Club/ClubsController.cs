using Application.Abstractions.Interfaces;
using Application.Clubs.Commands;
using Application.Clubs.Commands.Deactivate;
using Application.Clubs.Commands.Register;
using Application.Clubs.Commands.Update;
using Application.Clubs.Queries.GetAdminClubContext;
using Application.Clubs.Queries.GetAll;
using Application.Clubs.Queries.GetAllSearch;
using Application.Clubs.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Api.Infrastructure;

namespace Web.Api.Controllers.Club;

[Route("api/[controller]")]
[ApiController]
public class ClubsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ClubsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [AuthorizeAdmins]
    [HttpGet("admin-context")]
    public async Task<IActionResult> GetAdminContext([FromServices] IClubContext clubContext, CancellationToken cancellationToken)
    {
        var clubResult = await _mediator.Send(new GetAdminClubContext(), cancellationToken);
        
        return clubResult.IsSuccess 
            ? Ok(clubResult.Value) 
            : CustomResults.Problem(clubResult);
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var clubResult = await _mediator.Send(new GetAllClubs(), cancellationToken);
        
        return clubResult.IsSuccess 
            ? Ok(clubResult.Value) 
            : CustomResults.Problem(clubResult);
    }

    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var clubResult = await _mediator.Send(new GetClubById(id), cancellationToken);
        
        return clubResult.IsSuccess
            ? Ok(clubResult.Value)
            : CustomResults.Problem(clubResult);
    }
    
    [AllowAnonymous]
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string? name)
    {
        var clubResult = await _mediator.Send(new GetAllClubsSearch(name));

        return clubResult.IsSuccess 
            ? Ok(clubResult.Value) 
            : CustomResults.Problem(clubResult);
    }
    
    [AuthorizeSuperAdmin]
    [HttpPost]
    public async Task<IActionResult>Register([FromBody] RegisterClubRequest request)
    {
        var command = new RegisterClub(
            request.Name,
            request.Cif,
            request.Street,
            request.City,
            request.ZipCode,
            request.Country,
            request.PhoneNumber,
            request.Email
        );
        var clubResult = await _mediator.Send(command);

        return clubResult.IsSuccess 
            ? Ok(clubResult.Value) 
            : CustomResults.Problem(clubResult);
    }
    
    [AuthorizeAdmins]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult>Update([FromRoute] Guid id, [FromBody] UpdateClubRequest request)
    {
        var command = new UpdateClub(
            id,
            request.Name,
            request.Cif,
            request.Street,
            request.City,
            request.ZipCode,
            request.Country,
            request.PhoneNumber,
            request.Email
        );
        
        var clubResult = await _mediator.Send(command);

        return clubResult.IsSuccess 
            ? NoContent()
            : CustomResults.Problem(clubResult);
    }
    
    [AuthorizeSuperAdmin]
    [HttpPatch("{id:guid}/deactivate")]
    public async Task<IActionResult>Deactivate([FromRoute] Guid id)
    {
        var clubResult = await _mediator.Send(new DeactivateClub(id));

        return clubResult.IsSuccess 
            ? NoContent()
            : CustomResults.Problem(clubResult);
    }
    
    [AuthorizeSuperAdmin]
    [HttpPatch("{id:guid}/activate")]
    public async Task<IActionResult>Activate([FromRoute] Guid id)
    {
        var clubResult = await _mediator.Send(new ActivateClub(id));

        return clubResult.IsSuccess 
            ? NoContent()
            : CustomResults.Problem(clubResult);
    }
}
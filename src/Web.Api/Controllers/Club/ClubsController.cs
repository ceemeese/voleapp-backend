using Application.Clubs.Queries.GetAll;
using Application.Clubs.Queries.GetAllSearch;
using Application.Clubs.Queries.GetById;
using MediatR;
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

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var clubResult = await _mediator.Send(new GetAllClubs(), cancellationToken);
        
        return clubResult.IsSuccess 
            ? Ok(clubResult.Value) 
            : CustomResults.Problem(clubResult);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var clubResult = await _mediator.Send(new GetClubById(id), cancellationToken);
        
        return clubResult.IsSuccess
            ? Ok(clubResult.Value)
            : CustomResults.Problem(clubResult);
    }
    
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string? name)
    {
        var clubResult = await _mediator.Send(new GetAllClubsSearch(name));

        return clubResult.IsSuccess 
            ? Ok(clubResult.Value) 
            : CustomResults.Problem(clubResult);
    }
}
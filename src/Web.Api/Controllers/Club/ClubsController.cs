using Application.Clubs.Queries.GetAll;
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
    public async Task<IActionResult> GetAll()
    {
        var clubResult = await _mediator.Send(new GetAllClubs());
        
        return clubResult.IsSuccess 
            ? Ok(clubResult.Value) 
            : CustomResults.Problem(clubResult);
    }
}
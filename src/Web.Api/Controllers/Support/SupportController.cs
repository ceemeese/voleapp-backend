using Application.Supports.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Api.Infrastructure;

namespace Web.Api.Controllers.Support;

[ApiController]
[Route("api/[controller]")]
public class SupportController : ControllerBase
{
    private readonly IMediator _mediator;

    public SupportController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [AllowAnonymous]
    [HttpPost("contact")]
    public async Task<IActionResult> SendContactEmail([FromBody] SendContactRequest request, CancellationToken cancellationToken)
    {
        var command = new SendContact(request.Name, request.Email, request.Message);
        var contactResult = await _mediator.Send(command, cancellationToken);

        return contactResult.IsSuccess
            ? NoContent()
            : CustomResults.Problem(contactResult);
    }
}
using Application.Users.Commands.Login;
using Application.Users.Queries.GetToken;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Api.Infrastructure;

namespace Web.Api.Controllers.Auth;

public class AuthsController : ControllerBase
{
  private readonly IMediator _mediator;
  
  public AuthsController(IMediator mediator)
  {
    _mediator = mediator;
  }
  
  [AllowAnonymous]
  [HttpPost("login")]
  public async Task<IActionResult>Login([FromBody] LoginUserRequest request)
  {
    var command = new LoginUser(
      request.Username,
      request.Password
    );
    var authResult = await _mediator.Send(command);

    return authResult.IsSuccess 
      ? Ok(authResult.Value) 
      : CustomResults.Problem(authResult);
  }

  [AllowAnonymous]
  [HttpGet("Refresh/{refreshToken}")]
  public async Task<IActionResult> Refresh(Guid refreshToken)
  {
    var authResult = await _mediator.Send(new GetToken(refreshToken));

    return authResult.IsSuccess 
      ? Ok(authResult.Value) 
      : CustomResults.Problem(authResult);
  }
}
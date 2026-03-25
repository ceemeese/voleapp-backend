using Application.Users.Commands.ChangePassword;
using Application.Users.Commands.Forgot;
using Application.Users.Commands.Login;
using Application.Users.Commands.Reset;
using Application.Users.Queries.GetToken;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Api.Infrastructure;

namespace Web.Api.Controllers.Auth;

[Route("api/[controller]")]
[ApiController]
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
  
  [AllowAnonymous]
  [HttpPost("ForgotPassword")]
  public async Task<IActionResult> Reset([FromBody] ForgotPasswordRequest request)
  {
    var command = new ForgotPasswordUser(
      request.Email
    );
    var authResult = await _mediator.Send(command);

    return authResult.IsSuccess 
      ? Ok(authResult.Value) 
      : CustomResults.Problem(authResult);
  }
  
  [AllowAnonymous]
  [HttpPost("ResetPassword")]
  public async Task<IActionResult> Reset([FromBody] ResetPasswordRequest request)
  {
    var command = new ResetPasswordUser(
      request.Email,
      request.Token,
      request.NewPassword
    );
    var authResult = await _mediator.Send(command);

    return authResult.IsSuccess 
      ? NoContent()
      : CustomResults.Problem(authResult);
  }
  
  [Authorize]
  [HttpPost("ChangePassword")]
  public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
  {
    var command = new ChangeUserPassword(
      request.OldPassword,
      request.NewPassword
    );
    var authResult = await _mediator.Send(command);

    return authResult.IsSuccess 
      ? NoContent()
      : CustomResults.Problem(authResult);
  }
}
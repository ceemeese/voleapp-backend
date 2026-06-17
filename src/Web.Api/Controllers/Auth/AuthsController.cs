using Application.Users.Commands.ChangePassword;
using Application.Users.Commands.ConfirmEmail;
using Application.Users.Commands.Forgot;
using Application.Users.Commands.Login;
using Application.Users.Commands.RefreshToken;
using Application.Users.Commands.ResendConfirmation;
using Application.Users.Commands.Reset;
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
  [HttpPost("refresh")]
  public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
  {
    var authResult = await _mediator.Send(new RefreshToken(request.RefreshToken));

    return authResult.IsSuccess 
      ? Ok(authResult.Value) 
      : CustomResults.Problem(authResult);
  }
  
  [AllowAnonymous]
  [HttpPost("forgot-password")]
  public async Task<IActionResult> Reset([FromBody] ForgotPasswordRequest request)
  {
    var command = new ForgotPasswordUser(
      request.Email
    );
    var authResult = await _mediator.Send(command);

    return authResult.IsSuccess 
      ? NoContent() 
      : CustomResults.Problem(authResult);
  }
  
  [AllowAnonymous]
  [HttpPost("reset-password")]
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
  [HttpPost("change-password")]
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
  
  [AllowAnonymous]
  [HttpGet("confirm-email")]
  public async Task<IActionResult> ConfirmEmail([FromQuery] ConfirmEmailRequest request)
  {
    var command = new ConfirmEmail(
      request.Token,
      request.Email
    );
    var authResult = await _mediator.Send(command);

    return authResult.IsSuccess 
      ? NoContent()
      : CustomResults.Problem(authResult);
  }
  
  [AllowAnonymous]
  [HttpPost("resend-confirmation")]
  public async Task<IActionResult> ResendConfirmation([FromBody] ResendConfirmationRequest request)
  {
    var command = new ResendConfirmation(request.Email);
    var authResult = await _mediator.Send(command);

    return authResult.IsSuccess 
      ? NoContent()
      : CustomResults.Problem(authResult);
  }
  
}
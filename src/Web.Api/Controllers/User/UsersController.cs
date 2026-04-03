using Application.Users.Commands.Activate;
using Application.Users.Commands.Deactivate;
using Application.Users.Commands.Register;
using Application.Users.Commands.Update;
using Application.Users.Queries.GetAll;
using Application.Users.Queries.GetByEmail;
using Application.Users.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Api.Infrastructure;

namespace Web.Api.Controllers.User;

[Route("api/[controller]")]
[ApiController]

public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [AuthorizeSuperAdmin]
    [HttpGet]
    public async Task<IActionResult>GetAll()
    {
        var userResult = await _mediator.Send(new GetAllUsers());

        return userResult.IsSuccess 
            ? Ok(userResult.Value) 
            : CustomResults.Problem(userResult);
    }
    
    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult>GetById(Guid id)
    {
        var userResult = await _mediator.Send(new GetUserById(id));

        return userResult.IsSuccess 
            ? Ok(userResult.Value) 
            : CustomResults.Problem(userResult);
    }
    
    [AuthorizeAdmins]
    [HttpGet("search")]
    public async Task<IActionResult> GetByEmail([FromQuery] string email)
    {
        var userResult = await _mediator.Send(new GetUserByEmail(email));

        return userResult.IsSuccess 
            ? Ok(userResult.Value) 
            : CustomResults.Problem(userResult);
    }
    
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult>Register([FromBody] RegisterUserRequest request)
    {
        var command = new RegisterUser(
            request.Dni,
            request.Name,
            request.LastName,
            request.Username,
            request.Email,
            request.PhoneNumber,
            request.Password
        );
        var userResult = await _mediator.Send(command);

        return userResult.IsSuccess 
            ? Ok(userResult.Value) 
            : CustomResults.Problem(userResult);
    }
    
    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult>Update(Guid id, [FromBody] UpdateUserRequest request)
    {
        var command = new UpdateUser(
            id,
            request.Username,
            request.Email,
            request.PhoneNumber
        );
        
        var userResult = await _mediator.Send(command);

        return userResult.IsSuccess 
            ? NoContent()
            : CustomResults.Problem(userResult);
    }
    
    [AuthorizeSuperAdmin]
    [HttpPatch("{id:guid}/deactivate")]
    public async Task<IActionResult>Deactivate(Guid id)
    {
        var userResult = await _mediator.Send(new DeactivateUser(id));

        return userResult.IsSuccess 
            ? NoContent()
            : CustomResults.Problem(userResult);
    }
    
    [AuthorizeSuperAdmin]
    [HttpDelete("{id:guid}/activate")]
    public async Task<IActionResult>Activate(Guid id)
    {
        var userResult = await _mediator.Send(new ActivateUser(id));

        return userResult.IsSuccess 
            ? NoContent()
            : CustomResults.Problem(userResult);
    }
}
using Application.Users.Commands.Delete;
using Application.Users.Commands.Register;
using Application.Users.Commands.Update;
using Application.Users.Queries.GetAll;
using Application.Users.Queries.GetByEmail;
using Application.Users.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Web.Api.Controllers.User;
using Web.Api.Infrastructure;

namespace Web.Api.Controllers;

[Route("api/[controller]")]
[ApiController]

public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    
    [HttpGet]
    public async Task<IActionResult>GetAll()
    {
        var userResult = await _mediator.Send(new GetAllUsers());

        return userResult.IsSuccess 
            ? Ok(userResult.Value) 
            : CustomResults.Problem(userResult);
    }
    
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult>GetById(Guid id)
    {
        var userResult = await _mediator.Send(new GetUserById(id));

        return userResult.IsSuccess 
            ? Ok(userResult.Value) 
            : CustomResults.Problem(userResult);
    }
    
    
    [HttpGet("search")]
    public async Task<IActionResult> GetByEmail([FromQuery] string email)
    {
        var userResult = await _mediator.Send(new GetUserByEmail(email));

        return userResult.IsSuccess 
            ? Ok(userResult.Value) 
            : CustomResults.Problem(userResult);
    }
    
    
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
            ? Ok(userResult.Value) 
            : CustomResults.Problem(userResult);
    }
    
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult>Delete(Guid id)
    {
        var userResult = await _mediator.Send(new DeleteUser(id));

        return userResult.IsSuccess 
            ? Ok() 
            : CustomResults.Problem(userResult);
    }
}
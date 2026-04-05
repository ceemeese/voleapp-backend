using Application.ClubMember.Commands.Activate;
using Application.ClubMember.Commands.Deactivate;
using Application.ClubMember.Commands.Register;
using Application.ClubMember.Commands.ToggleFavourite;
using Application.ClubMember.Commands.Update;
using Application.ClubMember.Queries.GetAllMembers;
using Application.ClubMember.Queries.GetMemberDetail;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Api.Infrastructure;

namespace Web.Api.Controllers.ClubMember;

[ApiController]
public class ClubMemberController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public ClubMemberController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [AuthorizeAdmins]
    [HttpGet("api/clubs/{clubId:guid}/members")]
    public async Task<IActionResult> GetAll([FromRoute] Guid clubId, [FromQuery] string? search, CancellationToken cancellationToken)
    {
        var command = new GetAllClubMembers(clubId, search);
        
        var memberResult = await _mediator.Send(command, cancellationToken);
        
        return memberResult.IsSuccess 
            ? Ok(memberResult.Value) 
            : CustomResults.Problem(memberResult);
    }
    
    [AuthorizeAdmins]
    [HttpGet("api/clubs/{clubId:guid}/members/{userId:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid clubId,[FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var command = new GetMemberDetail(clubId,  userId);
        
        var memberResult = await _mediator.Send(command, cancellationToken);
        
        return memberResult.IsSuccess 
            ? Ok(memberResult.Value) 
            : CustomResults.Problem(memberResult);
    }

    [AuthorizeAdmins]
    [HttpPost("api/clubs/{clubId:guid}/members")]
    public async Task<IActionResult> Register([FromRoute] Guid clubId, [FromBody] ClubMemberRequest request, CancellationToken cancellationToken)
    {
        var command = new RegisterClubMember(
            clubId,
            request.UserId,
            request.Role
        );
        
        var memberResult = await _mediator.Send(command, cancellationToken);
        
        return memberResult.IsSuccess 
            ? Ok(memberResult.Value) 
            : CustomResults.Problem(memberResult);
    }
    
    [AuthorizeAdmins]
    [HttpPut("api/clubs/{clubId:guid}/members/{userId:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid clubId, [FromRoute] Guid userId, [FromBody] UpdateClubMemberRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateClubMember(
            clubId,
            userId,
            request.Role,
            request.MembershipNumber,
            request.IsMember
        );
        
        var memberResult = await _mediator.Send(command, cancellationToken);
        
        return memberResult.IsSuccess 
            ? NoContent()
            : CustomResults.Problem(memberResult);
    }
    
    [AuthorizeAdmins]
    [HttpPatch("api/clubs/{clubId:guid}/members/{userId:guid}/deactivate")]
    public async Task<IActionResult> Deactivate([FromRoute] Guid clubId, [FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var command = new DeactivateClubMember(
            clubId,
            userId
        );
        
        var memberResult = await _mediator.Send(command, cancellationToken);
        
        return memberResult.IsSuccess 
            ? NoContent()
            : CustomResults.Problem(memberResult);
    }
    
    [AuthorizeAdmins]
    [HttpPatch("api/clubs/{clubId:guid}/members/{userId:guid}/activate")]
    public async Task<IActionResult> Activate([FromRoute] Guid clubId, [FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var command = new ActivateClubMember(
            clubId,
            userId
        );
        
        var memberResult = await _mediator.Send(command, cancellationToken);
        
        return memberResult.IsSuccess 
            ? NoContent()
            : CustomResults.Problem(memberResult);
    }
    
    [Authorize]
    [HttpPatch("api/clubs/{clubId:guid}/members/{userId:guid}/favourite")]
    public async Task<IActionResult> ToggleFavourite([FromRoute] Guid clubId, [FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var command = new ToggleFavourite(
            clubId,
            userId
        );
        
        var memberResult = await _mediator.Send(command, cancellationToken);
        
        return memberResult.IsSuccess 
            ? NoContent()
            : CustomResults.Problem(memberResult);
    }
    
}
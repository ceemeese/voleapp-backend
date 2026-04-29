using Application.Reservations.Commands.Cancel;
using Application.Reservations.Commands.Register;
using Application.Reservations.Commands.UpdateStatus;
using Application.Reservations.Queries.GetAll;
using Application.Reservations.Queries.GetById;
using Application.Reservations.Queries.GetClubReservations;
using Application.Reservations.Queries.GetUserReservations;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Api.Infrastructure;
namespace Web.Api.Controllers.Reservation;

[ApiController]
[Route("api")]
public class ReservationsController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public ReservationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [HttpPost("reservations")]
    public async Task<IActionResult> Register([FromBody] RegisterReservationRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RegisterReservation(
            request.CourtId,
            request.Date,
            request.StartTime,
            request.EndTime,
            request.Notes);
        
        var reservationResult = await _mediator.Send(command, cancellationToken);

        return reservationResult.IsSuccess
            ? Ok(reservationResult.Value)
            : CustomResults.Problem(reservationResult);
    }
    
    [AuthorizeAdmins]
    [HttpPatch("reservations/{id:int}/status")]
    public async Task<IActionResult> UpdateStatus([FromRoute]int id, [FromBody] UpdateStatusReservationRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateStatusReservation(id, request.NewStatus);
        
        var reservationResult = await _mediator.Send(command, cancellationToken);

        return reservationResult.IsSuccess
            ? NoContent()
            : CustomResults.Problem(reservationResult);
    }
    
    [Authorize]
    [HttpPatch("reservations/{id:int}/cancel")]
    public async Task<IActionResult> Cancel([FromRoute]int id, CancellationToken cancellationToken)
    {
        var command = new CancelReservation(id);
        
        var reservationResult = await _mediator.Send(command, cancellationToken);

        return reservationResult.IsSuccess
            ? NoContent()
            : CustomResults.Problem(reservationResult);
    }
    
    [AuthorizeSuperAdmin]
    [HttpGet("reservations")]
    public async Task<IActionResult> GetAll([FromQuery] ReservationFilters filters, CancellationToken cancellationToken)
    {
        var query = new GetAllReservations(filters.UserId, filters.ClubId, filters.StartDateRange, filters.EndDateRange);
        
        var reservationResult = await _mediator.Send(query, cancellationToken);

        return reservationResult.IsSuccess
            ? Ok(reservationResult.Value)
            : CustomResults.Problem(reservationResult);
    }
    
    [Authorize]
     [HttpGet("users/{userId:guid}/reservations")]
     public async Task<IActionResult> GetUserReservations([FromRoute] Guid userId, [FromQuery] DateOnly? startDate, DateOnly? endDate, CancellationToken cancellationToken)
     {
         var query = new GetUserReservations(userId,  startDate, endDate);
         
         var reservationResult = await _mediator.Send(query, cancellationToken);
 
         return reservationResult.IsSuccess
             ? Ok(reservationResult.Value)
             : CustomResults.Problem(reservationResult);
     }
     
    [Authorize]
    [HttpGet("clubs/{clubId:guid}/reservations")]
    public async Task<IActionResult> GetClubReservations([FromRoute] Guid clubId, [FromQuery] DateOnly? startDate, DateOnly? endDate, CancellationToken cancellationToken)
    {
        var query = new GetClubReservations(clubId,  startDate, endDate);
         
        var reservationResult = await _mediator.Send(query, cancellationToken);
 
        return reservationResult.IsSuccess
            ? Ok(reservationResult.Value)
            : CustomResults.Problem(reservationResult);
    }
    
    [Authorize]
    [HttpGet("reservations/{reservationId:int}")]
    public async Task<IActionResult> GetById([FromRoute] int reservationId,
        CancellationToken cancellationToken)
    {
        var query = new GetReservationById(reservationId);
        
        var reservationResult = await _mediator.Send(query, cancellationToken);
        
        return reservationResult.IsSuccess
            ? Ok(reservationResult.Value)
            : CustomResults.Problem(reservationResult);
    }
}
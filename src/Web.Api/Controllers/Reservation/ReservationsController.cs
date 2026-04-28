using Application.Reservations.Commands;
using Application.Reservations.Commands.Register;
using Application.Reservations.Commands.UpdateStatus;
using Application.Reservations.Queries;
using Application.Reservations.Queries.GetAll;
using Application.Reservations.Queries.GetById;
using Application.Reservations.Queries.GetUserReservations;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Api.Infrastructure;
namespace Web.Api.Controllers.Reservation;

[ApiController]
public class ReservationsController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public ReservationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [HttpPost("api/reservations")]
    public async Task<IActionResult> Register([FromBody] RegisterReservationRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RegisterReservation(
            request.CourtId,
            request.Date,
            request.StartTime,
            request.EndTime,
            request.Notes);
        
        var reservationResult = await _mediator.Send(command);

        return reservationResult.IsSuccess
            ? Ok(reservationResult.Value)
            : CustomResults.Problem(reservationResult);
    }
    
    [Authorize]
    [HttpPatch("api/reservations/{id:int}/status")]
    public async Task<IActionResult> UpdateStatus([FromRoute]int id, [FromBody] UpdateStatusReservationRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateStatusReservation(id, request.NewStatus);
        
        var reservationResult = await _mediator.Send(command, cancellationToken);

        return reservationResult.IsSuccess
            ? NoContent()
            : CustomResults.Problem(reservationResult);
    }
    
    [AuthorizeSuperAdmin]
    [HttpGet("api/reservations")]
    public async Task<IActionResult> GetAll([FromQuery] ReservationFilters filters, CancellationToken cancellationToken)
    {
        var command = new GetAllReservations(filters.UserId, filters.ClubId, filters.StartDateRange, filters.EndDateRange);
        
        var reservationResult = await _mediator.Send(command, cancellationToken);

        return reservationResult.IsSuccess
            ? Ok(reservationResult.Value)
            : CustomResults.Problem(reservationResult);
    }
    
    [Authorize]
    [HttpGet("api/users/${userId:Guid}/reservations")]
    public async Task<IActionResult> GetUserReservations([FromRoute] Guid userId, [FromQuery] DateOnly? startDate, DateOnly? endDate, CancellationToken cancellationToken)
    {
        var command = new GetUserReservations(userId,  startDate, endDate);
        
        var reservationResult = await _mediator.Send(command, cancellationToken);

        return reservationResult.IsSuccess
            ? Ok(reservationResult.Value)
            : CustomResults.Problem(reservationResult);
    }
    
    [Authorize]
    [HttpGet("api/bookings/${reservationId:int}")]
    public async Task<IActionResult> GetAll([FromRoute] int reservationId,
        CancellationToken cancellationToken)
    {
        var command = new GetReservationById(reservationId);
        
        var reservationResult = await _mediator.Send(command, cancellationToken);
        
        return reservationResult.IsSuccess
            ? Ok(reservationResult.Value)
            : CustomResults.Problem(reservationResult);
    }
}
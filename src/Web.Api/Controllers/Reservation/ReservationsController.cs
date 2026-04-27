using Application.Reservations.Commands;
using Application.Reservations.Commands.Register;
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
    [HttpPost("api/bookings")]
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
    
}
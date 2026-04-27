using Application.Abstractions.DTO.Reservation;
using MediatR;
using SharedKernel;

namespace Application.Reservations.Commands.Register;

public sealed record RegisterReservation(
    Guid CourtId,
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime,
    string? Notes
): IRequest<Result<ReservationResponse>>;
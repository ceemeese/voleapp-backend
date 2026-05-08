using Application.Abstractions.DTO.Reservation;
using MediatR;
using SharedKernel;

namespace Application.Reservations.Queries.GetById;

public sealed record GetReservationById(int ReservationId) : IRequest<Result<ReservationResponse>>
{
}
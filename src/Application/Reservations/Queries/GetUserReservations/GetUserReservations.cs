using Application.Abstractions.DTO.Reservation;
using Domain.Reservation;
using MediatR;
using SharedKernel;

namespace Application.Reservations.Queries.GetUserReservations;

public sealed record GetUserReservations(Guid UserId, DateOnly? StartDate, DateOnly? EndDate) : IRequest<Result<List<ReservationResponse>>>;
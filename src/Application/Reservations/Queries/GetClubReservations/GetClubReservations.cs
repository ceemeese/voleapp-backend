using Application.Abstractions.DTO.Reservation;
using Domain.Reservation;
using MediatR;
using SharedKernel;

namespace Application.Reservations.Queries.GetClubReservations;

public sealed record GetClubReservations(Guid ClubId, DateOnly? StartDate, DateOnly? EndDate) : IRequest<Result<List<ReservationResponse>>>;
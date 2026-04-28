using Application.Abstractions.DTO.Reservation;
using MediatR;
using SharedKernel;

namespace Application.Reservations.Queries.GetAll;

public sealed record GetAllReservations(Guid? UserId, Guid? ClubId, DateOnly? StartDateRange, DateOnly? EndDateRange) : IRequest<Result<List<ReservationResponse>>>;
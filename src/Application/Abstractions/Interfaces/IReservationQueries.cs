using Application.Abstractions.DTO.Reservation;

namespace Application.Abstractions.Interfaces;

public interface IReservationQueries
{
    Task<List<ReservationCompleteResponse>> GetAllReservationsAsync(Guid? userId, Guid? clubId, DateOnly? startDateRange, DateOnly? endDateRange, CancellationToken cancellationToken);
    Task<ReservationCompleteResponse?> GetReservationCompleteByIdAsync(int reservationId, CancellationToken cancellationToken);

}
namespace Domain.Reservation;

public interface IReservationRepository
{
    void Add(Reservation reservation);
    Task<Reservation?> GetReservationByIdAsync(int reservationId, CancellationToken cancellationToken);
    Task<List<Reservation>> GetAllReservationsAsync(Guid? userId, Guid? clubId, DateOnly? startDateRange, DateOnly? endDateRange, CancellationToken cancellationToken);
}
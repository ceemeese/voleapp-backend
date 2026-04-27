namespace Domain.Reservation;

public interface IReservationRepository
{
    void Add(Reservation reservation);
    Task<Reservation?> GetReservationById(int reservationId, CancellationToken cancellationToken);
}
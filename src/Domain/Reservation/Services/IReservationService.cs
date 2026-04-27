using SharedKernel;

namespace Domain.Reservation.Services;

public interface IReservationService
{
    Result<Reservation> BookCourt(
        Guid userId,
        Court.Court courtId,
        DateOnly date,
        TimeOnly start,
        TimeOnly end,
        string? notes
    );
}

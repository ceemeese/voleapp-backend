using Domain.Reservation;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories;

internal sealed class ReservationRepository : IReservationRepository
{
    private readonly ApplicationDbContext _context;
    
    public ReservationRepository(ApplicationDbContext dbContext)
    {
        _context = dbContext;
    }

    public void Add(Reservation reservation)
    {
        _context.Reservations.Add(reservation);
    }
}
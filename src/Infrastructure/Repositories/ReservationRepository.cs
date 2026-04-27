using Domain.Reservation;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

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

    public async Task<Reservation?> GetReservationById(int reservationId, CancellationToken cancellationToken)
    {
        return await _context.Reservations
            .Where(r => r.Id == reservationId)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
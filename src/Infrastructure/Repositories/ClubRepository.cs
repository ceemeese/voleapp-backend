using Domain.Club;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

internal sealed class ClubRepository : IClubRepository
{
    private readonly ApplicationDbContext _context;
    
    public ClubRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<Club>> GetAll(CancellationToken cancellationToken)
    {
        return await _context.Clubs.ToListAsync(cancellationToken);
    }
}
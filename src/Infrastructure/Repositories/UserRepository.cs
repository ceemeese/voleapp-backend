using Domain.User;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

internal sealed class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<User>> GetAll(CancellationToken cancellationToken)
    {
        return await _context.Users.IgnoreQueryFilters().ToListAsync(cancellationToken);
    }
    
    public async Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken)   
    {                                      
        return await _context.Users
            .Where(u => u.Id == userId)
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<User?> GetByEmailActiveAsync(string email, CancellationToken cancellationToken)
    {
        return await _context.Users
            .Where(u => u.Email == email)
            .FirstOrDefaultAsync(cancellationToken);
    }
    
    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await _context.Users
            .IgnoreQueryFilters()
            .Where(u => u.Email == email)
            .FirstOrDefaultAsync(cancellationToken);
    }
    
    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        return await _context.Users
            .Where(u => u.Username == username)
            .FirstOrDefaultAsync(cancellationToken);
    }
    
    public async Task<bool> ExistByDniAsync(string dni, CancellationToken cancellationToken)
    {
        return await _context.Users
            .IgnoreQueryFilters()
            .AnyAsync(u => u.Dni.Trim() == dni.Trim(), cancellationToken);
    }
    
    public async Task<bool> ExistByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await _context.Users.AnyAsync(u => u.Email == email, cancellationToken);
    }
    
    public async Task<bool> ExistByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        return await _context.Users.AnyAsync(u => u.Username == username, cancellationToken);
    }

    public async Task<bool> ExistAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _context.Users.AnyAsync(u => u.Id == userId, cancellationToken);
    }

    public void Add(User user)
    {
        _context.Users.Add(user);
    }

    public void Delete(User user)
    {
        _context.Users.Remove(user);
    }
}
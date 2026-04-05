using Application.Abstractions.Interfaces;
using Domain.Club.Enum;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Authentication;

internal sealed class ClubContext : IClubContext
{
    private sealed class ClubContextUnavailableException : Exception
    {
        public ClubContextUnavailableException() : base("Club context no disponible")
        {
        }
    }

    private readonly IUserContext _userContext;
    private readonly ApplicationDbContext _context;
    
    private Guid? _cacheClubId;

    public ClubContext(IUserContext userContext, ApplicationDbContext context)
    {
        _userContext = userContext;
        _context = context;
    }
    
    public async Task<Guid> GetClubIdAsync()
    {
        if (_cacheClubId.HasValue) return _cacheClubId.Value;
        
        if (_userContext.UserId == Guid.Empty) return Guid.Empty;

        _cacheClubId = await _context.ClubMembers
            .Where(m => m.UserId == _userContext.UserId && m.Role == MemberRole.Admin)
            .Select(m => m.ClubId)
            .FirstOrDefaultAsync();

        return _cacheClubId ?? Guid.Empty;
    }
    
    public async Task<bool> HasClubAsync() => await GetClubIdAsync() != Guid.Empty;
}
    
    
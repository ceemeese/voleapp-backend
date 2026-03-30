using Application.Abstractions.DTO.ClubMember;
using Application.Abstractions.Interfaces;
using Domain.Club.Entities;
using Domain.Club.Enum;
using Domain.User;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.ServiceQueries;

internal sealed class ClubMemberQueries : IClubMemberQueries
{
    private readonly ApplicationDbContext _context;
    
    public ClubMemberQueries(ApplicationDbContext dbContext)
    {
        _context = dbContext;
    }

    public async Task<List<ClubMemberCompleteResponse>> GetMembersByClubId(Guid clubId, string? searchTerm, CancellationToken cancellationToken)
    {
        var query = _context.ClubMembers
            .AsNoTracking()
            .Where(c => c.ClubId == clubId)
            .Join(_context.Users,
                member => member.UserId,
                user => user.Id,
                (member, user) => new { member, user });

        if (!string.IsNullOrEmpty(searchTerm))
        {
            string search = searchTerm.ToLower();
            
            query = query.Where(d => 
                d.user.Name.ToLower().Contains(search) ||
                d.user.LastName.ToLower().Contains(search) ||
                d.user.Email.ToLower().Contains(search));
        }
        
        var data = await query.ToListAsync(cancellationToken);
        
        return data.Select(d => MapToResponse(d.member, d.user)).ToList();
    }

    public async Task<ClubMemberCompleteResponse?> GetMembersDetail(Guid clubId, Guid userId, CancellationToken cancellationToken)
    {
        var data = await _context.ClubMembers
            .AsNoTracking()
            .Where(c => c.ClubId == clubId && c.UserId == userId)
            .Join(
                _context.Users, 
                m=> m.UserId,
                user => user.Id, 
                (member, user) => new {member, user}
                )
            .FirstOrDefaultAsync(cancellationToken);

        if (data is null)
        {
            return null;
        }

        return MapToResponse(data.member, data.user);
    }

    private static ClubMemberCompleteResponse MapToResponse(ClubMember member, User user)
    {
        return new ClubMemberCompleteResponse(
            member.UserId,
            user.Name,
            user.LastName,
            user.Email,
            member.Role,
            member.IsMember,
            member.MembershipNumber,
            member.IsFavourite,
            member.IsActive
        );
    }

    public async Task<bool> IsAdminInClub(Guid clubId, Guid userId, CancellationToken cancellationToken)
    {
        var rolesPermission = new[] {MemberRole.Admin, MemberRole.Owner};
        return await _context.ClubMembers
            .AsNoTracking()
            .AnyAsync(c => c.ClubId == clubId && c.UserId == userId && rolesPermission.Contains(c.Role), cancellationToken);
    }

    public async Task<bool> IsManagerInOtherClubs(Guid userId, Guid currentClubId, CancellationToken cancellationToken)
    {
        var managementRoles = new[] {MemberRole.Admin, MemberRole.Owner, MemberRole.Coach};
        return await _context.ClubMembers
            .AnyAsync(m => m.UserId == userId && 
                           m.ClubId != currentClubId &&
                           managementRoles.Contains(m.Role) &&
                           m.IsActive, cancellationToken);
    }
}
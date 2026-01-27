using Domain.Club.Enum;
using Domain.Common;

namespace Domain.Club.Entities;

public sealed class ClubMember : Entity<int>
{
    internal ClubMember(Guid clubId, Guid userId, MemberRole role)
    {
        ClubId = clubId;
        UserId = userId;
        Role = role;
        MembershipNumber = null;
        RegisteredOn = DateOnly.FromDateTime(DateTime.UtcNow);
        IsFavourite = false;
        IsMember = false;
        IsActive = true;
    }
    
    private ClubMember()
    {
    }
    
    public Guid ClubId { get; private set; }
    public Guid UserId { get; private set; }
    public MemberRole Role { get; private set; }
    public string? MembershipNumber { get; private set; }
    public DateOnly RegisteredOn { get; private set; }
    public bool IsFavourite { get; private set; }
    public bool IsMember { get; private set; }
    public bool IsActive { get; private set; }
    
}
using Domain.Club.Enum;
using Domain.Common;
using SharedKernel;

namespace Domain.Club.Entities;

public sealed class ClubMember : Entity<int>
{
    public Guid ClubId { get; private set; }
    public Guid UserId { get; private set; }
    public MemberRole Role { get; private set; }
    public string? MembershipNumber { get; private set; }
    public DateOnly RegisteredOn { get; private set; }
    public bool IsFavourite { get; private set; }
    public bool IsMember { get; private set; }
    public bool IsActive { get; private set; }
    
    private ClubMember(Guid clubId, Guid userId, MemberRole role)
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
    
    internal static ClubMember Create(Guid clubId, Guid userId, MemberRole role)
    {
        return new ClubMember(clubId, userId, role);
    }

    internal Result UpdateMembership(MemberRole role, bool isMember, string? membershipNumber)
    {
        if (isMember && string.IsNullOrWhiteSpace(membershipNumber))
        {
            return Result.Failure(ClubMemberErrors.NotEmptymembershipNumber);
        }

        Role = role;
        IsMember = isMember;
        MembershipNumber = isMember ? membershipNumber : null;
        
        return Result.Success();
    }

    internal void Deactivate()
    {
        IsActive = false;
    }
    
    internal void Activate()
    {
        IsActive = true;
    }

    internal void ToggleFavourite()
    {
        IsFavourite = !IsFavourite;
    }
    
}
namespace Application.Abstractions.DTO.ClubMember;

public sealed record ClubMemberCompleteResponse(
    Guid UserId,
    string Name,
    string LastName,
    string Email,
    RoleResponse Role,
    bool IsMember,
    string? MembershipNumber,
    bool IsFavourite,
    bool IsActive
);
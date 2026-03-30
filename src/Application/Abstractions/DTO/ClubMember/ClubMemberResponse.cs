namespace Application.Abstractions.DTO.ClubMember;

public sealed record ClubMemberResponse(
    int Id,
    Guid ClubId,
    Guid UserId,
    RoleResponse Role,
    string? MembershipNumber,
    DateOnly RegisteredOn,
    bool IsFavourite,
    bool IsMember,
    bool IsActive
);
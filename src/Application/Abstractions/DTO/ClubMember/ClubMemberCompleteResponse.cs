using Domain.Club.Enum;

namespace Application.Abstractions.DTO.ClubMember;

public sealed record ClubMemberCompleteResponse(
    Guid UserId,
    string Name,
    string LastName,
    string Email,
    MemberRole Role,
    bool IsMember,
    string? MembershipNumber,
    bool IsFavourite,
    bool IsActive
);
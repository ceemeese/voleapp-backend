namespace Application.Abstractions.DTO.ClubMember;

public sealed record UserClubResponse(Guid ClubId, string ClubName, RoleResponse Role, bool IsFavourite, bool IsMember, string? MembershipNumber, DateOnly RegisteredOn);
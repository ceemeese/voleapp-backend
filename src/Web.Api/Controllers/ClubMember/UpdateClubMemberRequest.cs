namespace Web.Api.Controllers.ClubMember;

public sealed record UpdateClubMemberRequest(
    string Role,
    string? MembershipNumber, 
    bool IsMember
);
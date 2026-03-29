namespace Web.Api.Controllers.ClubMember;

public sealed record ClubMemberRequest(Guid UserId, string Role);
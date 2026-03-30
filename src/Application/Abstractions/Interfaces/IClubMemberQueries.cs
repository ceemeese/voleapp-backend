using Application.Abstractions.DTO.ClubMember;

namespace Application.Abstractions.Interfaces;

public interface IClubMemberQueries
{
    Task<List<ClubMemberCompleteResponse>> GetMembersByClubId(Guid clubId, string? searchTerm, CancellationToken cancellationToken);
    Task<ClubMemberCompleteResponse?> GetMembersDetail(Guid clubId, Guid userId, CancellationToken cancellationToken);
    Task<bool> IsAdminInClub(Guid clubId, Guid userId, CancellationToken cancellationToken);
    Task<bool> IsManagerInOtherClubs(Guid userId, Guid currentClubId, CancellationToken cancellationToken);

}
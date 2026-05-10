namespace Domain.Club;

public interface IClubRepository
{
    Task<List<Club>> GetAllAsync(CancellationToken cancellationToken);
    Task<Club?> GetClubByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<Club>> GetAllSearchAsync(string? name, CancellationToken cancellationToken);
    Task<List<Club>> GetAllSearchActiveAsync(string? name, CancellationToken cancellationToken);
    Task<Club?> GetClubWithMembersAsync(Guid clubId, CancellationToken cancellationToken);
    void Add(Club club);
    Task<List<Club>> GetActiveClubsByCityWithSchedulesAsync(string city, CancellationToken cancellationToken);
}
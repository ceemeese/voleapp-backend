namespace Domain.Club;

public interface IClubRepository
{
    Task<List<Club>> GetAll(CancellationToken cancellationToken);
    Task<Club?> GetClubById(Guid id, CancellationToken cancellationToken);
    Task<List<Club>> GetAllSearch(string? name, CancellationToken cancellationToken);
}
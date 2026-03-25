namespace Domain.Club;

public interface IClubRepository
{
    Task<List<Club>> GetAll(CancellationToken cancellationToken);
}
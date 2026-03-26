namespace Domain.Court;

public interface ICourtRepository
{
    Task<List<Court>> GetAll(CancellationToken cancellationToken);
    Task<Court?> GetCourtById(Guid id, CancellationToken cancellationToken);
    //Task<List<Court>> GetByClubId(Guid clubId, CancellationToken cancellationToken);
    Task<bool> ExistsByNameInClub(Guid clubId, string name, CancellationToken cancellationToken);
    void Add(Court court);
}
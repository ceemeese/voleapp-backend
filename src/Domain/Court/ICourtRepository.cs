namespace Domain.Court;

public interface ICourtRepository
{
    Task<List<Court>> GetAll(CancellationToken cancellationToken);
    Task<Court?> GetCourtById(Guid id, CancellationToken cancellationToken);

    Task<Court?> GetCourtWithEventsByDateRangeAsync(Guid courtId, DateTime startDate, CancellationToken cancellationToken, DateTime? endDate = null);
    Task<List<Court>> GetCourtsByClubId(Guid clubId, CancellationToken cancellationToken);
    Task<bool> ExistsByNameInClub(Guid clubId, string name, CancellationToken cancellationToken);
    Task<bool> ExistsByNameInClubExcludeId(Guid clubId, string name, Guid id, CancellationToken cancellationToken);

    void Add(Court court);
}
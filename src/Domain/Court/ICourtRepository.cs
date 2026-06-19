namespace Domain.Court;

public interface ICourtRepository
{
    Task<List<Court>> GetAll(CancellationToken cancellationToken);
    Task<Court?> GetCourtByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Court?> GetCourtWithEventsByDateRangeAsync(Guid courtId, DateTime startDate, CancellationToken cancellationToken, DateTime? endDate = null);
    Task<Court?> GetCourtWithEventByIdAsync(Guid courtId, int eventId, CancellationToken cancellationToken);
    Task<List<Court>> GetCourtsByClubIdAsync(Guid clubId, CancellationToken cancellationToken);
    Task<bool> ExistsByNameInClubAsync(Guid clubId, string name, CancellationToken cancellationToken);
    Task<bool> ExistsByNameInClubExcludeIdAsync(Guid clubId, string name, Guid id, CancellationToken cancellationToken);
    void Add(Court court);
    Task<List<Court>> GetCourtsByClubIdWithFilterEventsAsync(List<Guid> clubId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken);
}
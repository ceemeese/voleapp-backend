using Application.Abstractions.DTO.CourtEvent;

namespace Application.Abstractions.Interfaces;

public interface ICourtEventQueries
{
    Task<List<CourtEventCompleteResponse>> GetByClubIdAsync(Guid clubId, DateTime startDate, DateTime? endDate, CancellationToken cancellationToken);
    Task<CourtEventCompleteResponse?> GetByIdAsync(Guid courtId, int eventId, CancellationToken cancellationToken);

}
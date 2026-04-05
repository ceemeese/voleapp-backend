namespace Application.Abstractions.Interfaces;

public interface IClubContext
{
    Task<Guid> GetClubIdAsync();
    Task<bool> HasClubAsync();
}
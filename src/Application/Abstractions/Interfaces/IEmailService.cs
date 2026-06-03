namespace Application.Abstractions.Interfaces;

public interface IEmailService
{
    Task SendReservationEmailAsync(string toEmail, string playerName, string date, string time, string courtName, string clubName);
    Task SendContactEmailAsync(string name, string email, string message);
}
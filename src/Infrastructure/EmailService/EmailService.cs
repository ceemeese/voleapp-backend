using Application.Abstractions.Interfaces;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Logging;

namespace Infrastructure.EmailService;

public class EmailService: IEmailService
{
    private readonly string _apiKey;
    private readonly string _fromEmail;
    private readonly string _fromName;
    private readonly string _templateId;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration,  ILogger<EmailService> logger)
    {
        _apiKey = configuration["SendGrid:ApiKey"]!;
        _fromEmail = configuration["SendGrid:FromEmail"]!;
        _fromName = configuration["SendGrid:FromName"]!;
        _templateId = configuration["SendGrid:TemplateId"]!;
        _logger = logger;
    }
    
    public async Task SendReservationEmailAsync(string toEmail, string playerName, string date, string time, string courtName, string clubName)
    {
        try
        {
            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress(_fromEmail, _fromName);
            var to = new EmailAddress(toEmail, playerName);

            var msg = new SendGridMessage();
            msg.SetFrom(from);
            msg.AddTo(to);
            msg.SetTemplateId(_templateId);

            var templateData = new
            {
                playerName = playerName,
                date = date,
                time = time,
                courtName = courtName,
                clubName = clubName
            };
            msg.SetTemplateData(templateData);

            var response = await client.SendEmailAsync(msg);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("SendGrid devolvió código de error: {StatusCode}. No se pudo enviar el correo a {Email}.", response.StatusCode,
                    toEmail);
            }
            else
            {
                _logger.LogInformation("Email de reserva enviado correctamente a {Email} para la fecha {date} y hora {time}.", toEmail, date, time);
            }
            
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error crítico e inesperado al intentar enviar el email de reserva a {Email}.", toEmail);
        }
       
    }
}
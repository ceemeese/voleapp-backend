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
    private readonly string _fromSendEmail;
    private readonly string _fromName;
    private readonly string _templateIdSend;
    private readonly string _templateIdForgot;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration,  ILogger<EmailService> logger)
    {
        _apiKey = configuration["SendGrid:ApiKey"]!;
        _fromEmail = configuration["SendGrid:FromEmail"]!;
        _fromSendEmail = configuration["SendGrid:FromSendEmail"]!;
        _fromName = configuration["SendGrid:FromName"]!;
        _templateIdSend = configuration["SendGrid:TemplateIdSend"]!;
        _templateIdForgot = configuration["SendGrid:TemplateIdForgot"]!;
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
            msg.SetTemplateId(_templateIdSend);

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


    public async Task SendContactEmailAsync(string name, string email, string message)
    {
        try
        {
            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress(_fromEmail, _fromName);
            var to = new EmailAddress(_fromSendEmail, "Equipo VoleApp");

            var msg = new SendGridMessage();
            msg.SetFrom(from);
            msg.AddTo(to);
            msg.SetSubject($"Nuevo Mensaje de Contacto: {name}");
            msg.SetReplyTo(new EmailAddress(email, name));
            
            var htmlContent = $@"
            <div style='font-family: sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #f1f5f9; border-radius: 12px;'>
                <h2 style='color: #0f172a; font-size: 20px; font-weight: 800; margin-bottom: 20px;'>Nuevo mensaje recibido</h2>
                <hr style='border: 0; border-top: 1px solid #e2e8f0; margin-bottom: 20px;' />
                <p><strong>De:</strong> {name}</p>
                <p><strong>Email de contacto:</strong> <a href='mailto:{email}'>{email}</a></p>
                <p><strong>Mensaje:</strong></p>
                <div style='background-color: #f8fafc; padding: 15px; border-radius: 8px; color: #334155; line-height: 1.6;'>
                    {message}
                </div>
            </div>";

            msg.AddContent(MimeType.Html, htmlContent);
            var response = await client.SendEmailAsync(msg);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("SendGrid devolvió código de error: {StatusCode}. No se pudo enviar el email de contacto de {Name}.", response.StatusCode,
                    name);
            }
            else
            {
                _logger.LogInformation("Email de contacto de{Name} enviado correctamente", name);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error crítico e inesperado al intentar enviar el email de contacto");
        }
        
    }
    
    public async Task SendResetPasswordEmailAsync(string toEmail, string resetUrl)
    {
        try
        {
            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress(_fromEmail, _fromName);
            var to = new EmailAddress(toEmail);

            var msg = new SendGridMessage();
            msg.SetFrom(from);
            msg.AddTo(to);
            msg.SetTemplateId(_templateIdForgot);

            var templateData = new
            {
                resetUrl = resetUrl,
            };
            
            msg.SetTemplateData(templateData);
            var response = await client.SendEmailAsync(msg);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("SendGrid devolvió código de error: {StatusCode}. No se pudo enviar el email de contacto de {toEmail}.", response.StatusCode,
                    toEmail);
            }
            else
            {
                _logger.LogInformation("Email de contacto a {toEmail} enviado correctamente", toEmail);
            }
            
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar email de reset a {Email}.", toEmail);
        }
    }
}
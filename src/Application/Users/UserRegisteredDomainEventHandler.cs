using System.Web;
using Application.Abstractions.Interfaces;
using Application.Abstractions.Options;
using Domain.User.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application.Users;

internal sealed class UserRegisteredDomainEventHandler : INotificationHandler<UserRegisteredDomainEvent>
{
    private readonly ILogger<UserRegisteredDomainEventHandler> _logger;
    private readonly IEmailService _emailService;
    private readonly IIdentityService _identityService;
    private readonly UrlOptions _webOptions;

    public UserRegisteredDomainEventHandler(ILogger<UserRegisteredDomainEventHandler> logger, IEmailService emailService, IIdentityService identityService, IOptions<UrlOptions> webOptions)
    {
        _logger = logger;
        _emailService = emailService;
        _identityService = identityService;
        _webOptions = webOptions.Value;
    }
    
    
    public async Task Handle(UserRegisteredDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Procesando envío de email para usuario registrado" +
            "Usuario: {Username}, Email: {Email}",
            notification.User.Username, 
            notification.User.Email
            );
        var user = notification.User;
        
        var identityData = await _identityService.GenerateEmailConfirmationTokenAsync(user.Id);
        if (identityData is null)
        {
            return;
        }
        
        var baseUrl = _webOptions.FrontendUrl;
        var encodedToken = HttpUtility.UrlEncode(identityData.Token);
        var confirmationUrl = $"{baseUrl}/confirm-email?token={encodedToken}&email={identityData.Email}";
        
        await _emailService.SendConfirmationEmailAsync(user.Email,  confirmationUrl);
    }
}
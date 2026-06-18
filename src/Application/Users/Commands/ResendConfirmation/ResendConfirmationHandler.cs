using System.Web;
using Application.Abstractions.Interfaces;
using Application.Abstractions.Options;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedKernel;

namespace Application.Users.Commands.ResendConfirmation;

internal sealed class ResendConfirmationHandler : IRequestHandler<ResendConfirmation, Result>
{
    private readonly IIdentityService _identityService;
    private readonly IEmailService _emailService;
    private readonly UrlOptions _webOptions;
    private readonly ILogger<ResendConfirmationHandler> _logger;
    
    public  ResendConfirmationHandler(IIdentityService identityService, IEmailService emailService,  IOptions<UrlOptions> webOptions,  ILogger<ResendConfirmationHandler> logger)
    {
        _identityService = identityService;
        _emailService = emailService;
        _webOptions = webOptions.Value;
        _logger = logger;
    }

    public async Task<Result> Handle(ResendConfirmation request, CancellationToken cancellationToken)
    {
        var identityData = await _identityService.ResendConfirmationEmailAsync(request.Email);
        if (identityData is null || identityData.Token is null)
        {
            _logger.LogWarning("ResendConfirmation: no se encontró usuario o ya está confirmado para {Email}", request.Email); 
            return Result.Success();
        }
        
        var baseUrl = _webOptions.FrontendUrl;
        var encodedToken = HttpUtility.UrlEncode(identityData.Token);
        var encodedEmail = HttpUtility.UrlEncode(identityData.Email);
        var confirmationUrl = $"{baseUrl}/auth/confirm-email?token={encodedToken}&email={encodedEmail}";
        
        await _emailService.SendConfirmationEmailAsync(request.Email, confirmationUrl);
        return Result.Success();
    }
}
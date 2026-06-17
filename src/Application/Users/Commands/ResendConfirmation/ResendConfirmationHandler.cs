using System.Web;
using Application.Abstractions.Interfaces;
using Application.Abstractions.Options;
using MediatR;
using Microsoft.Extensions.Options;
using SharedKernel;

namespace Application.Users.Commands.ResendConfirmation;

internal sealed class ResendConfirmationHandler : IRequestHandler<ResendConfirmation, Result>
{
    private readonly IIdentityService _identityService;
    private readonly IEmailService _emailService;
    private readonly UrlOptions _webOptions;
    
    public  ResendConfirmationHandler(IIdentityService identityService, IEmailService emailService,  IOptions<UrlOptions> webOptions)
    {
        _identityService = identityService;
        _emailService = emailService;
        _webOptions = webOptions.Value;
    }

    public async Task<Result> Handle(ResendConfirmation request, CancellationToken cancellationToken)
    {
        var identityData = await _identityService.ResendConfirmationEmailAsync(request.Email);
        if (identityData is null || identityData.Token is null)
        {
            return Result.Success();
        }
        
        var baseUrl = _webOptions.FrontendUrl;
        var encodedToken = HttpUtility.UrlEncode(identityData.Token);
        var confirmationUrl = $"{baseUrl}/confirm-email?token={encodedToken}&email={identityData.Email}";
        
        await _emailService.SendConfirmationEmailAsync(request.Email, confirmationUrl);
        return Result.Success();
    }
}
using System.Web;
using Application.Abstractions.Interfaces;
using Application.Abstractions.Options;
using MediatR;
using Microsoft.Extensions.Options;
using SharedKernel;

namespace Application.Users.Commands.Forgot;

internal sealed class ForgotPasswordUserHandler : IRequestHandler<ForgotPasswordUser, Result>
{
    private readonly IIdentityService _identityService;
    private readonly IEmailService _emailService;
    private readonly UrlOptions _webOptions;

    public ForgotPasswordUserHandler(IIdentityService identityService,  IEmailService emailService, IOptions<UrlOptions> webOptions)
    {
        _identityService = identityService;
        _emailService = emailService;
        _webOptions = webOptions.Value;
    }

    public async Task<Result> Handle(ForgotPasswordUser request, CancellationToken cancellationToken)
    {
        var identityData = await _identityService.ForgotPasswordAsync(request.Email);

        if (identityData is null)
        {
            return Result.Success();
        }

        var baseUrl = _webOptions.FrontendUrl;
        var encodedToken = HttpUtility.UrlEncode(identityData.Token);
        var resetUrl = $"{baseUrl}/reset-password?token={encodedToken}&email={identityData.Email}";
        
        //envio de mail
      
        await _emailService.SendResetPasswordEmailAsync(identityData.Email, resetUrl);
        return Result.Success();
    }
}
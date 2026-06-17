using Application.Abstractions.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Application.Users.Commands.ConfirmEmail;

internal sealed class ConfirmEmailHandler : IRequestHandler<ConfirmEmail, Result>
{
    private readonly IIdentityService _identityService;
    private readonly ILogger<ConfirmEmailHandler> _logger;
    
    public ConfirmEmailHandler(IIdentityService identityService,  ILogger<ConfirmEmailHandler> logger)
    {
        _identityService = identityService;
        _logger = logger;
    }

    public async Task<Result> Handle(ConfirmEmail request, CancellationToken cancellationToken)
    {
        var identityResult = await _identityService.ConfirmEmailAsync(request.Email, request.Token);
        
        if (identityResult.IsFailure)
        {
            return Result.Failure(identityResult.Error);
        }
        
        _logger.LogInformation("Usuario con email {Email} confirmado", request.Email);
        
        return Result.Success();
    }
}
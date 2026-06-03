using Application.Abstractions.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Application.Supports.Commands;

internal sealed class SendContactHandler : IRequestHandler<SendContact, Result>
{

    private readonly IEmailService _emailService;
    private readonly ILogger<SendContactHandler> _logger;
    
    public SendContactHandler(IEmailService emailService, ILogger<SendContactHandler> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<Result> Handle(SendContact request, CancellationToken cancellationToken)
    {
        await _emailService.SendContactEmailAsync(
            request.Name,
            request.Email,
            request.Message
        );
        
        _logger.LogInformation("Handler: Email de contacto de {Name} enviado con éxito", request.Name);

        return Result.Success();
    }
}
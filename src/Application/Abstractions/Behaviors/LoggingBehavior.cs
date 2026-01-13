using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using SharedKernel;

namespace Application.Abstractions.Behaviors;

internal sealed class LoggingBehavior<TRequest, TResponse> (
    ILogger<LoggingBehavior<TRequest, TResponse>> logger) 
    : IPipelineBehavior<TRequest, TResponse> 
    where TRequest : class 
    where TResponse : Result
{

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        string requestName = typeof(TRequest).Name;
        
        logger.LogInformation("Iniciando petición {requestName}", requestName);

        //delegado
        TResponse response = await next();

        if (response.IsSuccess)
        {
            logger.LogInformation("Petición completada {requestName}", requestName);
        }
        else
        {
            using (LogContext.PushProperty("Error", response.Error, true))
            {
                logger.LogError("Petición completada con errores {requestName}", requestName);
            }
        }
        
        return response;
    }
}
using System.Reflection;
using MediatR;
using FluentValidation;
using FluentValidation.Results;
using SharedKernel;

namespace Application.Abstractions.Behaviors;

internal sealed class ValidationBehavior<TRequest, TResponse>(
    //inyeccion lista de validadores
    IEnumerable<IValidator<TRequest>> validators) 
    : IPipelineBehavior<TRequest, TResponse> 
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {

        ValidationFailure[] validationFailures = await ValidateAsync(request, validators);

        if (validationFailures.Length == 0)
        {
            return await next();
        }

        if (typeof(TResponse) == typeof(Result))
        {
            return (TResponse)(object)Result.Failure(CreateValidationError(validationFailures));
        }
        
        //no permite llamar a métodos estáticos de un tipo genérico (TResponse) directamente
        Type responseType = typeof(TResponse).GetGenericArguments()[0];
        MethodInfo? method = typeof(Result<>)
            .MakeGenericType(responseType)
            .GetMethod(nameof(Result<object>.ValidationFailure));

        if (method is not null)
        {
            return (TResponse)method.Invoke(
                null, 
                new object[] { CreateValidationError(validationFailures) })!;
        }
        
        throw new ValidationException(validationFailures);
    }

    private static async Task<ValidationFailure[]> ValidateAsync<TRequest>(
        TRequest request,
        IEnumerable<IValidator<TRequest>> validators)
    {
        if (!validators.Any())
        {
            return [];
        }
        
        var context = new ValidationContext<TRequest>(request);
        
        ValidationResult[] validationResults = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(context)));

        ValidationFailure[] validationFailures = validationResults
            .Where(validationResult => !validationResult.IsValid)
            .SelectMany(validationResult => validationResult.Errors)
            .ToArray();
        
        return validationFailures;
    }
    
    
    private static ValidationError CreateValidationError(ValidationFailure[] validationFailures) =>
        new(validationFailures.Select(f => Error.Problem(f.ErrorCode, f.ErrorMessage)).ToArray());
}
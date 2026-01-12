namespace SharedKernel;

public sealed record ValidationError : Error
{
    public ValidationError(Error[] errors) : base("Validation.General", "Se han producido uno o más errores de validación", ErrorType.Validation)
    {
        Errors = errors;
    }
    
    public Error[] Errors { get; }

    //De los resultados de validacion, sólo me quedo con los errores para construir el objeto ValidationError
    public static ValidationError FromFailures(IEnumerable<Result> results) =>
        new(results.Where(r => r.IsFailure).Select(r => r.Error).ToArray());
}
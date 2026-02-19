using SharedKernel;

namespace Application.Abstractions.Errors;

public static class IdentityErrors
{
    public static Error NotFound(Guid userId) => Error.NotFound(
        "IdentityUser.NotFound",
        $"Usuario con Id = '{userId}' no fue encontrado");

    public static readonly Error EmailNotUnique = Error.Conflict(
        "IdentityUser.EmailNotUnique",
        "El email ya existe");
    
    public static readonly Error UserNameNotUnique = Error.Conflict(
        "IdentityUser.UserNameNotUnique",
        "El nombre de usuario ya existe");

    public static readonly Error RegistrationFailed = Error.Problem(
        "RegistrationFailed",
        "No se pudo completar el registro del usuario");

    public static readonly Error InvalidCredentials = Error.Problem(
        "InvalidCredentials",
        "Usuario o contraseña incorrectos");
    
    public static readonly Error CannotUpdateSuperAdmin = Error.Problem(
        "IdentityUser.CannotUpdateSuperAdmin",
        "No se permite modificar el perfil de superadmin principal");

    public static readonly Error UpdateFailed = Error.Problem(
        "IdentityUser.UpdateFailed",
        "Ha ocurrido un error al actualizar los datos de seguridad en Identity");
    
    public static readonly Error SessionExpired = Error.Problem(
        "IdentityUser.SessionExpired",
        "La sessión del usuario ha expirado");
    
    public static readonly Error InvalidToken = Error.Problem(
        "IdentityUser.InvalidToken",
        "El refresh token no pertenece a ningún usuario");
}
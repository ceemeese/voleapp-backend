using SharedKernel;

namespace Domain.User;

public static class UserErrors
{
    public static Error NotFound(Guid userId) => Error.NotFound(
        "User.NotFound",
        $"El usuario con id {userId} no se encontró");
    
    public static readonly Error NotAuthorized = Error.NotFound(
        "User.NotAuthorized",
        "Usuario no autorizado");

    public static readonly Error EmailDuplicated = Error.Conflict(
        "User.EmailDuplicated",
    "El email ya existe");
    
    public static readonly Error NotFoundByEmail = Error.NotFound(
        "User.NotFoundByEmail",
    "No se ha encontrado ningún usuario con ese email");
    
    public static readonly Error NotFoundByUsername = Error.NotFound(
        "User.NotFoundByUsername",
        "No se ha encontrado ningún usuario con ese nombre de usuario");
    
    public static readonly Error DniDuplicated = Error.Conflict(
        "User.DniDuplicated",
        "El dni ya está registrado");
    
}
        
        
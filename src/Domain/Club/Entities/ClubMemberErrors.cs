using SharedKernel;

namespace Domain.Club.Entities;

public static class ClubMemberErrors
{
    public static Error NotFound(Guid clubId) => Error.NotFound(
        "ClubMember.NotFound",
        $"El club con id {clubId} no se encontró");
    
    public static Error MemberNotFound(Guid userId) => Error.NotFound(
        "ClubMember.MemberNotFound",
        $"El usuario con id {userId} no se encontró");
    
    public static readonly Error InvalidType = Error.Validation(
        "ClubMember.InvalidType",
        $"El rol no es válido");
    
    public static readonly Error MemberDuplicated = Error.Conflict(
        "ClubMember.MemberDuplicated",
        "El usuario ya está registrado en este club");
    
    public static readonly Error MinAdmin = Error.Validation(
        "ClubMember.MinAdmin",
        "El club debe tener al menos un administrador");
    
    public static readonly Error NotEmptymembershipNumber = Error.Validation(
        "ClubMember.NotEmptymembershipNumbe",
        "El número de socio es obligatorio");
}
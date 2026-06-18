using SharedKernel;

namespace Domain.Club;

public static class ClubErrors
{
    public static Error NotFound(Guid clubId) => Error.NotFound(
        "Club.NotFound",
        $"El club con id {clubId} no se encontró");
    
    public static readonly Error Forbidden = Error.Forbidden(
        "Club.Forbidden",
        "Usuario sin permisos");
    
    public static readonly Error ScheduleExistOverlap = Error.Conflict(
        "Club.ScheduleAlreadyExist",
        "El horario se solapa con otro existente ese día");
    
    public static Error ScheduleNotFound(int scheduleId) => Error.NotFound(
        "Club.ScheduleNotFound",
        $"El horario con id {scheduleId} no se encontró");
    
    public static Error AdminContextNotFound = Error.NotFound(
        "Club.AdminContextNotFound",
        "El usuario no tiene un club asignado como administrador");
    
    public static readonly Error ClubClosed = Error.Validation(
        "Reservation.ClubClosed",
        "El club está cerrado");
}
using SharedKernel;

namespace Domain.Reservation;

public static class ReservationErrors
{
    public static readonly Error Unauthenticated = Error.Unauthorized(
        "Reservation.Unauthenticated",
        "No se puede crear una reserva sin estar registrado");
    
    public static readonly Error InvalidType = Error.Validation(
        "Reservation.InvalidType",
        $"El estado de la reserva no es válido");
    
    public static Error NotFound(int reservationId) => Error.NotFound(
        "Reservation.ClubNotFound",
        $"No se encuentra la reserva con Id {reservationId}");
    
    public static readonly Error InvalidTotalPrice = Error.Validation(
        "Reservation.InvalidTotalPrice",
        "El precio total de la reserva debe ser mayor que cero");
    
    public static readonly Error InvalidHours = Error.Validation(
        "Reservation.InvalidHours",
        "La hora de inicio debe ser anterior a la hora de finalización");
    
    public static readonly Error PastDate = Error.Validation(
        "Reservation.PastDate",
        "No se puede hacer una reserva anterior a hoy");
    
    public static readonly Error PastStartTime = Error.Validation(
        "Reservation.PastStartTime",
        "No se puede reservar en una hora que ya ha pasado");
    
    public static readonly Error TooFarInFuture = Error.Validation(
        "Reservation.PastStartTime",
        "No se puede reservar con más de 30 días de antelación");
    
    public static Error ClubNotFound(Guid clubId) => Error.NotFound(
        "Reservation.ClubNotFound",
        $"El club con id {clubId} no está activo");
    
    public static readonly Error CourtNotFound = Error.NotFound(
        "Reservation.CourtNotFound",
        "La pista seleccionada no se encontró");
    
    public static readonly Error CourtNotActive = Error.NotFound(
        "Reservation.CourtNotActive",
        "La pista seleccionada no está activa");
    
    public static readonly Error AlreadyFinalized = Error.Conflict(
        "Reservation.AlreadyFinalized",
        "La reserva ya está finalizada");
    
    public static readonly Error CannotMoveBackToPending = Error.Conflict(
        "Reservation.CannotMoveBackToPending",
        "La reserva ya no puede pasar a estado pendiente");
    
}
using SharedKernel;

namespace Domain.Reservation.Events;

public sealed record ReservationCreatedDomainEvent( Reservation Reservation ) : IDomainEvent;
using SharedKernel;

namespace Domain.Reservation.Events;

public sealed record ReservationConfirmedDomainEvent( Reservation Reservation ) : IDomainEvent;
using SharedKernel;

namespace Domain.Reservation.Events;

public sealed record ReservationCreatedDomainEvent() : IDomainEvent;
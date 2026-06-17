using SharedKernel;

namespace Domain.User.Events;

public sealed record UserRegisteredDomainEvent( User User ) : IDomainEvent;
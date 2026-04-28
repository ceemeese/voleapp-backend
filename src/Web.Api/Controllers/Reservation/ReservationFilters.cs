namespace Web.Api.Controllers.Reservation;

public sealed record ReservationFilters(Guid? UserId, Guid? ClubId, DateOnly? StartDateRange, DateOnly? EndDateRange );
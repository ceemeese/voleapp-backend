using MediatR;
using SharedKernel;

namespace Application.Reservations.Commands.ConfirmPayment;

public sealed record ConfirmPayment(int ReservationId, string SessionId) : IRequest<Result>;

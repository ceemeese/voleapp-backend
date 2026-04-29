using MediatR;
using SharedKernel;

namespace Application.Reservations.Commands.Cancel;

public sealed record CancelReservation(int Id) : IRequest<Result>;
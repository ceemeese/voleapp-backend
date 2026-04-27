using MediatR;
using SharedKernel;

namespace Application.Reservations.Commands.UpdateStatus;

public sealed record UpdateStatusReservation(int Id, int NewStatus) : IRequest<Result>;
using MediatR;
using SharedKernel;

namespace Application.Schedules.Commands.Toggle;

public sealed record ToggleSchedule(Guid ClubId, int ScheduleId) : IRequest<Result>
{
}
using MediatR;
using SharedKernel;

namespace Application.Schedules.Commands.Update;

public sealed record UpdateSchedule(Guid ClubId, int ScheduleId, TimeOnly OpeningTime, TimeOnly ClosingTime) : IRequest<Result>
{
}
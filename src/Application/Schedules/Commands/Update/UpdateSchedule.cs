using Application.Abstractions.DTO.Schedule;
using MediatR;
using SharedKernel;

namespace Application.Schedules.Commands.Update;

public sealed record UpdateSchedule(Guid ClubId, int ScheduleId, TimeOnly OpeningTime, TimeOnly ClosingTime) : IRequest<Result<ScheduleResponse>>
{
}
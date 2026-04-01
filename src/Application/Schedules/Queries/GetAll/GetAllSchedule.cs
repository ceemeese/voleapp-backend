using Application.Abstractions.DTO.Schedule;
using MediatR;
using SharedKernel;

namespace Application.Schedules.Queries.GetAll;

public sealed record GetAllSchedule(Guid ClubId) : IRequest<Result<List<ScheduleResponse>>>
{
}
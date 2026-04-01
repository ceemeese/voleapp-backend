using MediatR;
using SharedKernel;

namespace Application.Schedules.Commands.Register;

public sealed record RegisterSchedule(Guid ClubId, string DayOfWeek, TimeOnly OpeningTime, TimeOnly ClosingTime) : IRequest<Result<int>>
{
}
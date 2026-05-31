using Application.Abstractions.DTO.Dashboard;
using MediatR;
using SharedKernel;

namespace Application.Analytics.GetOccupancy;

public sealed record GetOccupancy(Guid ClubId, int Year, int Month) : IRequest<Result<OccupancyResponse>>;
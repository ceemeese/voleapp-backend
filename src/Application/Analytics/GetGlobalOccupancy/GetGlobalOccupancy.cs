using Application.Abstractions.DTO.Dashboard;
using MediatR;
using SharedKernel;

namespace Application.Analytics.GetGlobalOccupancy;

public sealed record GetGlobalOccupancy(int Year, int Month) : IRequest<Result<GlobalOccupancyResponse>>;
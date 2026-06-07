using Application.Abstractions.DTO.Dashboard;
using MediatR;
using SharedKernel;

namespace Application.Analytics.GetGlobalDashboard;

public sealed record GetGlobalDashboard() : IRequest<Result<GlobalDashboardResponse>>;
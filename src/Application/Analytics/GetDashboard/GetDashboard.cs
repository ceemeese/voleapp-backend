using Application.Abstractions.DTO.Dashboard;
using MediatR;
using SharedKernel;

namespace Application.Analytics.GetDashboard;

public sealed record GetDashboard(Guid ClubId) : IRequest<Result<ClubDashboardResponse>>
{
    
}
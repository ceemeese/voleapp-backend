using Application.Abstractions.DTO.Dashboard;
using MediatR;
using SharedKernel;

namespace Application.Analytics.GetAnalytics;

public sealed record GetAnalytics(Guid ClubId, int Year, int Month) : IRequest<Result<AnalysisResponse>>;
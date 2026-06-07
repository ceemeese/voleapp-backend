using Application.Abstractions.DTO.Dashboard;
using MediatR;
using SharedKernel;

namespace Application.Analytics.GetGlobalAnalytics;

public sealed record GetGlobalAnalytics(int Year, int Month) : IRequest<Result<GlobalAnalysisResponse>>;
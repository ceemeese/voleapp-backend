using Application.Abstractions.DTO.Court;
using MediatR;
using SharedKernel;

namespace Application.Courts.Queries.GetAll;

public record GetAllCourts() : IRequest<Result<List<CourtSummaryResponse>>>
{
}
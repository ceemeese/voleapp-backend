using Application.Abstractions.DTO;
using MediatR;
using SharedKernel;

namespace Application.Courts.Queries;

public record GetAllCourts() : IRequest<Result<List<CourtResponse>>>
{
}
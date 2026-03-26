using Application.Abstractions.DTO.Court;
using MediatR;
using SharedKernel;

namespace Application.Courts.Queries.GetById;

public record GetCourtById(Guid CourtId) : IRequest<Result<CourtResponse>>
{
}
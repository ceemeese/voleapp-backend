using Application.Abstractions.DTO.Club;
using MediatR;
using SharedKernel;

namespace Application.Clubs.Queries.GetAll;

public sealed record GetAllClubs() : IRequest<Result<List<ClubResponse>>>
{
}
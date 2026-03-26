using Application.Abstractions.DTO.Club;
using MediatR;
using SharedKernel;

namespace Application.Clubs.Queries.GetById;

public sealed record GetClubById(Guid ClubId) : IRequest<Result<ClubResponse>>
{
}
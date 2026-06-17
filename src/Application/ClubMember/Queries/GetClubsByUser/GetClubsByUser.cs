using Application.Abstractions.DTO.ClubMember;
using MediatR;
using SharedKernel;

namespace Application.ClubMember.Queries.GetClubsByUser;

public sealed record GetClubsByUser(Guid UserId) : IRequest<Result<List<UserClubResponse>>>;
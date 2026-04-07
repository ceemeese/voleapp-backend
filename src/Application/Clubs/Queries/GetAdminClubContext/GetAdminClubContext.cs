using MediatR;
using SharedKernel;

namespace Application.Clubs.Queries.GetAdminClubContext;

public sealed record GetAdminClubContext : IRequest<Result<Guid>>
{
}
using MediatR;
using SharedKernel;

namespace Application.Users.Commands.Delete;

public sealed record DeleteUser(Guid UserId) : IRequest<Result>
{
}
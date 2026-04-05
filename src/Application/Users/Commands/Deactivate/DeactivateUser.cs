using MediatR;
using SharedKernel;

namespace Application.Users.Commands.Deactivate;

public sealed record DeactivateUser(Guid UserId) : IRequest<Result>
{
}
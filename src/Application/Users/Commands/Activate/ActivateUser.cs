using MediatR;
using SharedKernel;

namespace Application.Users.Commands.Activate;

public sealed record ActivateUser(Guid UserId) : IRequest<Result>
{
}
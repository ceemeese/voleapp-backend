using MediatR;
using SharedKernel;

namespace Application.Users.Commands.Update;

public sealed record UpdateUser(Guid Id, string Username, string Email, string PhoneNumber) : IRequest<Result<Unit>>
{
}

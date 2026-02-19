using MediatR;
using SharedKernel;

namespace Application.Users.Commands.Reset;

public sealed record ResetPasswordUser(string Email, string Token, string NewPassword) : IRequest<Result>;
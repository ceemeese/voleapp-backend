using MediatR;
using SharedKernel;

namespace Application.Users.Commands.ChangePassword;

public sealed record ChangeUserPassword(string OldPassword, string NewPassword) : IRequest<Result>;
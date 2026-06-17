using MediatR;
using SharedKernel;

namespace Application.Users.Commands.ConfirmEmail;

public sealed record ConfirmEmail(string Token, string Email) : IRequest<Result>;
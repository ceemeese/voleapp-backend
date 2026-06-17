using MediatR;
using SharedKernel;

namespace Application.Users.Commands.ResendConfirmation;

public sealed record ResendConfirmation(string Email) : IRequest<Result>;
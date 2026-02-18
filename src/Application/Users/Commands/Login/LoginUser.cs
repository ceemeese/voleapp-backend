using Application.Abstractions.DTO;
using MediatR;
using SharedKernel;

namespace Application.Users.Commands.Login;

public sealed record LoginUser(string Username, string Password) : IRequest<Result<LoginResponse>>
{
}
using Application.Abstractions.DTO.Auth;
using MediatR;
using SharedKernel;

namespace Application.Users.Commands.Login;

public sealed record LoginUser(string Username, string Password) : IRequest<Result<LoginResponse>>
{
}
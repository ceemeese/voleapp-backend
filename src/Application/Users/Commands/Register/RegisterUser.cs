using Application.Abstractions.DTO;
using Application.Abstractions.DTO.User;
using MediatR;
using SharedKernel;

namespace Application.Users.Commands.Register;

public sealed record RegisterUser(string Name, string LastName, string Username, string Email, string PhoneNumber, string Password) : IRequest<Result<UserResponse>>
{
}
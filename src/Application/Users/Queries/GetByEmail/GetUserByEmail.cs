using Application.Abstractions.DTO;
using Application.Abstractions.DTO.User;
using MediatR;
using SharedKernel;

namespace Application.Users.Queries.GetByEmail;

public sealed record GetUserByEmail(string Email) : IRequest<Result<UserResponse>>
{
}
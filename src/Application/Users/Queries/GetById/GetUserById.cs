using Application.Abstractions.DTO.User;
using MediatR;
using SharedKernel;

namespace Application.Users.Queries.GetById;

public sealed record GetUserById(Guid UserId) : IRequest<Result<UserResponse>>
{
}
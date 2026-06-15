using Application.Abstractions.DTO.User;
using MediatR;
using SharedKernel;

namespace Application.Users.Queries.GetAll;

public sealed record GetAllUsers() : IRequest<Result<List<UserResponse>>>
{
}
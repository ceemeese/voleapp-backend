using Application.Abstractions.DTO.Auth;
using MediatR;
using SharedKernel;

namespace Application.Users.Queries.GetToken;

public sealed record GetToken(Guid RefreshToken): IRequest<Result<LoginResponse>>;
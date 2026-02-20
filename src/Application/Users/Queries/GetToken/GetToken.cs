using Application.Abstractions.DTO;
using MediatR;
using SharedKernel;

namespace Application.Users.Queries.GetToken;

public sealed record GetToken(Guid RefreshToken): IRequest<Result<LoginResponse>>;
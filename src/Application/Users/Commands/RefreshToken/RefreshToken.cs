using Application.Abstractions.DTO.Auth;
using MediatR;
using SharedKernel;

namespace Application.Users.Commands.RefreshToken;

public sealed record RefreshToken(string Token): IRequest<Result<LoginResponse>>;
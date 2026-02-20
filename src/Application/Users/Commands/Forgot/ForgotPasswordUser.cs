using Application.Abstractions.DTO;
using MediatR;
using SharedKernel;

namespace Application.Users.Commands.Forgot;

public record ForgotPasswordUser(string Email) : IRequest<Result<ForgotResponse>>;
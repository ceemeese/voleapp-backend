using Domain.Court.Enum;
using MediatR;
using SharedKernel;

namespace Application.Courts.Commands.Register;

public sealed record RegisterCourt(Guid ClubId, string Name, string Type, decimal BasePrice, bool IsActive) : IRequest<Result<Guid>>;
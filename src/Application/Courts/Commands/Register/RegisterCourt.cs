using Application.Abstractions.DTO.Court;
using Domain.Court.Enum;
using MediatR;
using SharedKernel;

namespace Application.Courts.Commands.Register;

public sealed record RegisterCourt(Guid ClubId, string Name, string CourtType, decimal BasePrice, bool IsActive) : IRequest<Result<CourtResponse>>;
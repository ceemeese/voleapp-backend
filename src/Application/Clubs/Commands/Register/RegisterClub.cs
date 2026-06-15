using Application.Abstractions.DTO.Club;
using MediatR;
using SharedKernel;

namespace Application.Clubs.Commands.Register;

public sealed record RegisterClub(string Name, string Street, string City, string ZipCode, string Country, string PhoneNumber, string Email)
    : IRequest<Result<ClubResponse>>
{
}
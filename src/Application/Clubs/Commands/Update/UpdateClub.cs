using Application.Abstractions.DTO.Club;
using MediatR;
using SharedKernel;

namespace Application.Clubs.Commands.Update;

public sealed record UpdateClub(
    Guid Id,
    string Name,
    string Street,
    string City,
    string ZipCode,
    string Country,
    string PhoneNumber,
    string Email) : IRequest<Result<ClubResponse>>
{
}
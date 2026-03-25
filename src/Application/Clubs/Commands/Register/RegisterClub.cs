using MediatR;
using SharedKernel;

namespace Application.Clubs.Commands.Register;

public sealed record RegisterClub(string Name, string Cif, string Street, string City, string ZipCode, string Country, string PhoneNumber, string Email)
    : IRequest<Result<Guid>>
{
}
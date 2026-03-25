namespace Web.Api.Controllers.Club;

public sealed record RegisterClubRequest(
    string Name,
    string Cif,
    string Street,
    string City,
    string ZipCode,
    string Country,
    string PhoneNumber,
    string Email
);
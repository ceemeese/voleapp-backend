using SharedKernel;

namespace Domain.Common.ValueObjects;

public record Address
{
    public string Street { get; init; }
    public string City { get; init; }
    public string ZipCode { get; init; }
    public string Country { get; init; }
    
    
    private Address(string street, string city, string zipCode, string country)
    {
        Street = street;
        City = city;
        ZipCode = zipCode;
        Country = country;
    }

    public static Result<Address> Create(string street, string city, string zipCode, string country)
    {
        if (string.IsNullOrWhiteSpace(street) || string.IsNullOrWhiteSpace(city) ||
            string.IsNullOrWhiteSpace(zipCode) || string.IsNullOrWhiteSpace(country))
        {
            return Result.Failure<Address>(new Error("Address.Empty", "Los campos de la dirección no pueden estar vacíos", ErrorType.Validation));
        }

        return Result.Success(new Address(street, city, zipCode, country));
    }
    
    public override string ToString()
    {
        return $"{Street}, {City}, {ZipCode}, {Country}";
    }
    
};
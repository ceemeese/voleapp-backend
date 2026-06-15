namespace Application.Abstractions.DTO.Club;

public sealed record ClubResponse(
    Guid Id,
    string Name,
    AddressResponse Address,
    string PhoneNumber,
    string Email,
    bool IsActive,
    DateTime CreatedAt
);

public sealed record AddressResponse(
    string Street, 
    string City, 
    string Country, 
    string ZipCode
);
            
    
        

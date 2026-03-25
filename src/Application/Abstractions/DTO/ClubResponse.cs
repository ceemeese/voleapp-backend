namespace Application.Abstractions.DTO;

public sealed record ClubResponse(
    Guid Id,
    string Name,
    string Cif,
    string Address,
    string PhoneNumber,
    string Email,
    bool IsActive,
    DateTime CreatedAt
);

    
            
    
        

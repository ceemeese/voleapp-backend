namespace Application.Abstractions.DTO;

public sealed record UserResponse(
    Guid Id,
    string Dni,
    string Name,
    string LastName,
    string Username,
    string PhoneNumber,
    string Email,
    bool IsActive
    );
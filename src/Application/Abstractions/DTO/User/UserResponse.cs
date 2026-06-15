namespace Application.Abstractions.DTO.User;

public sealed record UserResponse(
    Guid Id,
    string Name,
    string LastName,
    string Username,
    string PhoneNumber,
    string Email,
    bool IsActive,
    DateTime CreatedAt
    );
using Domain.Common;

namespace Domain.User;

public sealed class User : AggregateRoot<Guid>
{
    public User( Guid id, string dni, string name, string lastName, string username, string email, string phoneNumber) : base(id)
    {
        Dni = dni;
        Name = name;
        LastName = lastName;
        Username = username;
        Email = email;
        PhoneNumber = phoneNumber;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }
    
    private User()
    {
    }
    
    public string Dni { get; private set; }
    public string Name { get; private set; }
    public string LastName { get; private set; }
    public string Username { get; private set; }
    public string Email { get; private set; }
    public string PhoneNumber { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    
    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;


    public void UpdateProfile(string username, string phoneNumber, string email)
    {
        Username = username;
        PhoneNumber = phoneNumber;
        Email = email;
    }
}
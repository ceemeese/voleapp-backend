using Domain.Club.Entities;
using Domain.Common;
using Domain.Common.ValueObjects;

namespace Domain.Club;

public sealed class Club : AggregateRoot<Guid>
{
    private Club(Guid id, string name, string cif, Address address, string phoneNumber, string email) : base(id)
    {
        Name = name;
        Cif = cif;
        Address = address;
        PhoneNumber = phoneNumber;
        Email = email;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    private Club()
    {
    }

    public static Club Create(string name, string cif, Address address, string phoneNumber, string email)
    {
        return new Club(
            Guid.NewGuid(), 
            name, 
            cif, 
            address, 
            phoneNumber, 
            email);
    }

    public void UpdateProfile(string name, string cif, Address address, string phoneNumber, string email)
    {
        Name = name;
        Cif = cif;
        Address = address;
        PhoneNumber = phoneNumber;
        Email = email;
    }
    
    public void Deactivate()
    {
        IsActive = false;
    }
    
    public void Activate()
    {
        if (IsActive) return;
        IsActive = false;
    }
    
    
    public string Name { get; private set; }
    public string Cif { get; private set; }
    public Address Address { get; private set; }
    public string PhoneNumber { get; private set; }
    public string Email { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    private readonly List<ClubMember> _members = new();
    public IReadOnlyCollection<ClubMember> Members => _members.AsReadOnly();
    
    private readonly List<Court> _courts = new();
    public IReadOnlyCollection<Court> Courts => _courts.AsReadOnly();

    private readonly List<Schedule> _schedules = new();
    public IReadOnlyCollection<Schedule> Schedules => _schedules.AsReadOnly();
}
using Domain.Club.Entities;
using Domain.Club.Enum;
using Domain.Club.Extensions;
using Domain.Common;
using Domain.Common.ValueObjects;
using SharedKernel;
using DayOfWeek = Domain.Club.Enum.DayOfWeek;

namespace Domain.Club;

public sealed class Club : AggregateRoot<Guid>
{
    public string Name { get; private set; }
    public string Cif { get; private set; }
    public Address Address { get; private set; }
    public string PhoneNumber { get; private set; }
    public string Email { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    private readonly List<ClubMember> _members = new();
    public IReadOnlyCollection<ClubMember> Members => _members.AsReadOnly();

    private readonly List<Schedule> _schedules = new();
    public IReadOnlyCollection<Schedule> Schedules => _schedules.AsReadOnly();
    public PricingConfig PricingConfig { get; private set; }
    
    
    
    private Club(Guid id, string name, string cif, Address address, string phoneNumber, string email) : base(id)
    {
        Name = name;
        Cif = cif;
        Address = address;
        PhoneNumber = phoneNumber;
        Email = email;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        PricingConfig = PricingConfig.CreateForClub(id);
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
    
    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;

    
    public Result<ClubMember> AddMember(Guid userId, MemberRole role)
    {
        if (_members.Any(m => m.UserId == userId))
        {
            return Result.Failure<ClubMember>(ClubMemberErrors.MemberDuplicated);
        }

        var newMember = ClubMember.Create(this.Id, userId, role);
        _members.Add(newMember);
        return Result.Success(newMember);
    }

    public Result<ClubMember> UpdateMember(Guid userId, MemberRole role, bool isMember, string? membershipNumber)
    {
        var member = GetMember(userId);
        if (member is null)
        {
            return Result.Failure<ClubMember>(ClubMemberErrors.MemberNotFound(userId));
        }

        if (role != MemberRole.Admin && IsLastActiveAdmin(member))
        {
            return Result.Failure<ClubMember>(ClubMemberErrors.MinAdmin);
        }
        
        var memberResult = member.UpdateMembership(role, isMember, membershipNumber);
        if (memberResult.IsFailure)
        {
            return Result.Failure<ClubMember>(memberResult.Error);
        }

        return Result.Success(member);
    }


    public Result DeactivateMember(Guid userId)
    {
        var member = GetMember(userId);
        if (member is null)
        {
            return Result.Failure(ClubMemberErrors.MemberNotFound(userId));
        }

        if (IsLastActiveAdmin(member))
        {
            return Result.Failure(ClubMemberErrors.MinAdmin);
        }
        
        member.Deactivate();
        return Result.Success();
    }
    
    public Result ActivateMember(Guid userId)
    {
        var member = GetMember(userId);
        if (member is null)
        {
            return Result.Failure(ClubMemberErrors.MemberNotFound(userId));
        }
        
        member.Activate();
        return Result.Success();
    }

    private bool IsLastActiveAdmin(ClubMember member)
    {
        return member.Role == MemberRole.Admin && 
               _members.Count(m => m.Role == MemberRole.Admin && m.IsActive) <= 1;
    }

    public Result ToggleMemberFavourite(Guid userId)
    {
        var member = GetMember(userId);
        if (member is null)
        {
            return Result.Failure(ClubMemberErrors.MemberNotFound(userId));
        }

        member.ToggleFavourite();
        return Result.Success();
    }
    
    private ClubMember? GetMember(Guid userId) => _members.FirstOrDefault(m => m.UserId == userId);
    
    
    public Result<Schedule> AddSchedule(DayOfWeek day, TimeOnly startTime, TimeOnly endTime)
    {
        var overlaps = _schedules.Any(s => s.DayOfWeek == day && startTime < s.ClosingTime && endTime > s.OpeningTime);

        if (overlaps)
        {
            return Result.Failure<Schedule>(ClubErrors.ScheduleExistOverlap);
        }
        
        var scheduleResult = Schedule.Create(this.Id, day, startTime, endTime);
        if (scheduleResult.IsFailure)
        {
            return Result.Failure<Schedule>(scheduleResult.Error);
        }
        
        _schedules.Add(scheduleResult.Value);
        return Result.Success(scheduleResult.Value);
    }
    
    
    public Result<Schedule> UpdateScheduleTime(int scheduleId, TimeOnly openingTime, TimeOnly closingTime)
    {
        var schedule = _schedules.FirstOrDefault(s => s.Id == scheduleId);
        if (schedule is null)
        {
            return Result.Failure<Schedule>(ClubErrors.ScheduleNotFound(scheduleId));
        }
        
        var overlaps = _schedules.Any(s => s.Id != schedule.Id && s.DayOfWeek == schedule.DayOfWeek && openingTime < s.ClosingTime && closingTime > s.OpeningTime);

        if (overlaps)
        {
            return Result.Failure<Schedule>(ClubErrors.ScheduleExistOverlap);
        }

        var scheduleResult = schedule.UpdateHours(openingTime, closingTime);
        if (scheduleResult.IsFailure)
        {
            return Result.Failure<Schedule>(scheduleResult.Error);
        }
        
        return Result.Success(scheduleResult.Value);
    }
    
    
    public Result SwitchOpeningStatus(int scheduleId)
    {
        var schedule = _schedules.FirstOrDefault(s => s.Id == scheduleId);
        if (schedule is null)
        {
            return Result.Failure(ClubErrors.ScheduleNotFound(scheduleId));
        }

        if (schedule.IsClosed)
        {
            schedule.MaskAsOpen();
        }
        else
        {
            schedule.MaskAsClosed();
        }
        
        return Result.Success();
    }
    
    public Result EnsureMembership(Guid userId)
    {
        var alreadyMember = _members.Any(m => m.UserId == userId);
        if (alreadyMember) return Result.Success();

        var memberResult = AddMember(userId, MemberRole.Player);
        return memberResult.IsFailure 
            ? Result.Failure(memberResult.Error) 
            : Result.Success();
    }
    
    public Result UpdatePricing( decimal rain, double windT, decimal windD, double heatT, decimal heatD, double coldT, decimal coldD)
    {
        var pricingResult = PricingConfig.Update(rain, windT, windD, heatT, heatD, coldT, coldD);
        if (pricingResult.IsFailure)
        {
            return Result.Failure(pricingResult.Error);
        }
        return Result.Success();
    }
    
    public bool IsOpen(DateOnly date, TimeOnly start, TimeOnly end)
    {
        var domainDayOfWeek = date.DayOfWeek.ToDomainDay();
        return Schedules
            .Where(s => s.DayOfWeek == domainDayOfWeek && !s.IsClosed)
            .Any(s => start >= s.OpeningTime && end <= s.ClosingTime);
    }
    
    public bool IsOpen(DateTime date, TimeOnly start, TimeOnly end)
    {
        return IsOpen(DateOnly.FromDateTime(date), start, end);
    }
}
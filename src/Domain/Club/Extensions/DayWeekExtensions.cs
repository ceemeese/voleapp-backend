namespace Domain.Club.Extensions;

public static class DayWeekExtensions
{
    public static Domain.Club.Enum.DayOfWeek ToDomainDay(this DayOfWeek systemDay)
    {
        return systemDay switch
        {
            DayOfWeek.Monday => Enum.DayOfWeek.Monday,
            DayOfWeek.Tuesday => Enum.DayOfWeek.Tuesday,
            DayOfWeek.Wednesday => Enum.DayOfWeek.Wednesday,
            DayOfWeek.Thursday => Enum.DayOfWeek.Thursday,
            DayOfWeek.Friday => Enum.DayOfWeek.Friday,
            DayOfWeek.Saturday => Enum.DayOfWeek.Saturday,
            DayOfWeek.Sunday => Enum.DayOfWeek.Sunday,
            _ => Enum.DayOfWeek.Monday
        };
    }
    
    
    public static System.DayOfWeek ToDotNetDay(this Domain.Club.Enum.DayOfWeek domainDay)
    {
        return domainDay switch
        {
            Domain.Club.Enum.DayOfWeek.Monday => System.DayOfWeek.Monday,
            Domain.Club.Enum.DayOfWeek.Tuesday => System.DayOfWeek.Tuesday,
            Domain.Club.Enum.DayOfWeek.Wednesday => System.DayOfWeek.Wednesday,
            Domain.Club.Enum.DayOfWeek.Thursday => System.DayOfWeek.Thursday,
            Domain.Club.Enum.DayOfWeek.Friday => System.DayOfWeek.Friday,
            Domain.Club.Enum.DayOfWeek.Saturday => System.DayOfWeek.Saturday,
            Domain.Club.Enum.DayOfWeek.Sunday => System.DayOfWeek.Sunday,
            _ => System.DayOfWeek.Monday
        };
    }
}
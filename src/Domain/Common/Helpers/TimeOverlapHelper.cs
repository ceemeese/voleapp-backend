namespace Domain.Common.Helpers;

public static class TimeOverlapHelper
{
    public static bool TimesOverlap(TimeOnly start1, TimeOnly end1, TimeOnly start2, TimeOnly end2)
    {
        return start1 < end2 && end1 > start2;
    }

    public static bool DateTimeOverlaps(DateTime start1, DateTime end1, DateTime start2, DateTime end2)
    {
        return start1 < end2 && end1 > start2;
    }
}
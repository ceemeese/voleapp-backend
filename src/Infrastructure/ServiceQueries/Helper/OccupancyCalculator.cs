namespace Infrastructure.ServiceQueries.Helper;


internal static class OccupancyCalculator
{
    public static readonly Dictionary<DayOfWeek, string> DayNames = new() {
        { DayOfWeek.Monday, "Lunes" }, { DayOfWeek.Tuesday, "Martes" }, { DayOfWeek.Wednesday, "Miércoles" },
        { DayOfWeek.Thursday, "Jueves" }, { DayOfWeek.Friday, "Viernes" }, { DayOfWeek.Saturday, "Sábado" }, { DayOfWeek.Sunday, "Domingo" }
    };

    public static Dictionary<DayOfWeek, int> CountDaysOfWeekInPeriod(DateOnly start, DateOnly end)
    {
        var dic = new Dictionary<DayOfWeek, int>();
        for (DateOnly date = start; date <= end; date = date.AddDays(1))
        {
            var day = date.DayOfWeek;
            if (dic.ContainsKey(day)) dic[day]++;
            else dic[day] = 1;
        }
        return dic;
    }

    public static double CalculateRate(double occupiedHours, double availableHours)
    {
        if (availableHours <= 0) return 0;
        return Math.Round((occupiedHours / availableHours) * 100, 2);
    }
}

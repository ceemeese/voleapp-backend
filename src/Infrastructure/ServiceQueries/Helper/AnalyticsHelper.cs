using System.Globalization;

namespace Infrastructure.ServiceQueries.Helper;

internal static class AnalyticsHelper
{
    private static readonly CultureInfo Culture = new("es-ES");

    public static string GetCapitalizedMonthName(int monthNumber)
    {
        string monthName = Culture.DateTimeFormat.GetMonthName(monthNumber);
        return char.ToUpper(monthName[0]) + monthName.Substring(1);
    }
}
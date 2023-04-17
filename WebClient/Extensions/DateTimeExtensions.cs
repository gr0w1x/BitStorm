using System.Globalization;

namespace WebClient.Extensions;

public static class DateTimeExtensions
{
    public static string DefaultFormat(this DateTimeOffset dateTime) => DefaultFormat(dateTime.ToLocalTime().DateTime);
    public static string DefaultFormat(this DateTime dateTime)
    {
        DateTime today = DateTime.UtcNow.Date;
        if (dateTime.Date == today)
        {
            return $"Today, {dateTime.ToString("t", CultureInfo.InvariantCulture)}";
        }
        else if (dateTime.Date == today.AddDays(-1))
        {
            return $"Yesterday, {dateTime.ToString("t", CultureInfo.InvariantCulture)}";
        }

        DateTime startOfWeek = today.AddDays(-(int)today.DayOfWeek);
        if (dateTime >= startOfWeek && dateTime < startOfWeek.AddDays(7))
        {
            return dateTime.ToString("dddd, HH:mm", CultureInfo.InvariantCulture);
        }

        DateTime startOfYear = new(today.Year, 1, 1);
        if (dateTime >= startOfYear && dateTime < startOfYear.AddYears(1))
        {
            return dateTime.ToString("MMMM d, HH:mm", CultureInfo.InvariantCulture);
        }
        else
        {
            return dateTime.ToString("MMMM d yyyy, HH:mm", CultureInfo.InvariantCulture);
        }
    }
}

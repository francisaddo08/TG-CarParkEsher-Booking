namespace TG.CarParkEsher.Booking
{
    internal static class DateTimeExtensions
    {
        internal static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int d = dt.DayOfWeek - startOfWeek;
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }
    }
}


using System.Globalization;

namespace TG.CarParkEsher.Booking
{
    internal record EsherCarParkDayInfo( string DayName, int DayNumber, DateTime DateValue);
    public static class DateTimeExtensions
    {
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int d = dt.DayOfWeek - startOfWeek;
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }
    }
    internal sealed class CalenderWorkerService : BackgroundService
    {
        private readonly ILogger<CalenderWorkerService> _logger;
       
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public CalenderWorkerService(ILogger<CalenderWorkerService> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        }
        protected async override Task ExecuteAsync(CancellationToken cancellationToken)
        {
          await Task.Yield();
          while (!cancellationToken.IsCancellationRequested)
            {
              var dayss = WeekDays();   

                await  Task.Delay(3000000, cancellationToken);
            }



        }
        public List<EsherCarParkDayInfo> WeekDays()
        {
            var todaysDate = DateTime.UtcNow.ToShortDateString();
            DateTime Monday = DateTime.Now.StartOfWeek(DayOfWeek.Monday);
            DateTime Tuesday = Monday.AddDays(1);
            DateTime Wednesday = Monday.AddDays(2);
            DateTime Thursday = Monday.AddDays(3);
            DateTime Friday = Monday.AddDays(4);
            DateTime Saturday = Monday.AddDays(5);
            DateTime Sunday = Monday.AddDays(6);

            List<EsherCarParkDayInfo> WeekDates = new List<EsherCarParkDayInfo>()
                {
                  new EsherCarParkDayInfo(DayOfWeek.Monday.ToString(), 1,Monday),
                  new EsherCarParkDayInfo(DayOfWeek.Tuesday.ToString(),2, Tuesday),
                  new EsherCarParkDayInfo(DayOfWeek.Wednesday.ToString(),3, Wednesday),
                  new EsherCarParkDayInfo(DayOfWeek.Thursday.ToString(), 4,Thursday),
                  new EsherCarParkDayInfo(DayOfWeek.Friday.ToString(), 5,Friday),
                  new EsherCarParkDayInfo(DayOfWeek.Saturday.ToString(),6, Saturday),
                  new EsherCarParkDayInfo(DayOfWeek.Sunday.ToString(), 7,Sunday)
                };
            CultureInfo cinfo = new CultureInfo("en-uk");
            CalendarWeekRule wr = cinfo.DateTimeFormat.CalendarWeekRule;
            DayOfWeek firstDayOfWeek = DayOfWeek.Monday;
            //get calendar from culture info
            Calendar c = cinfo.Calendar;
            int currentWeekNumber = c.GetWeekOfYear(DateTime.Now, wr, firstDayOfWeek);
            return WeekDates;
        }
    }
}


using System.Globalization;

namespace TG.CarParkEsher.Booking
{
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
              //using(var scope = _serviceScopeFactory.CreateScope())
              //  {
              //      var calenderRepository = scope.ServiceProvider.GetRequiredService<CalenderRepository>();
              //      var result = await calenderRepository.GetDaysOfWeek(cancellationToken);
              //      if (result.IsSuccess)
              //      {
              //          var days = result.Value;
              //          foreach (var day in days)
              //          {
              //              _logger.LogInformation("Day: {DayName}, DayNumber: {DayNumber}, DateValue: {DateValue}", day.DayName, day.DayNumber, day.DateValue.ToString("yyyy-MM-dd"));
              //          }
              //      }
              //      else
              //      {
              //          _logger.LogError("Failed to retrieve days of the week: {Error}", result.Error);
              //      }
              //  }
                var dayss = WeekDays();   

                await  Task.Delay(3000000, cancellationToken);
            }



        }
        public List<EsherCarParkDayInfo> WeekDays()
        {
            
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
           
            return WeekDates;
        }
    }
}

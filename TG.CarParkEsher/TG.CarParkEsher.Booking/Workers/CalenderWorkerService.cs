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
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var calenderService = scope.ServiceProvider.GetRequiredService<ICalenderService>();
                    var result = await calenderService.UpdateWeekDaysAsync(cancellationToken);
                    if (result.IsSuccess)
                    {
                        //var days = result.Value;
                        //foreach (var day in days)
                        //{
                        //    _logger.LogInformation("Day: {DayName}, DayNumber: {DayNumber}, DateValue: {DateValue}", day.DayName, day.DayNumber, day.DateValue.ToString("yyyy-MM-dd"));
                        //}
                    }
                    else
                    {
                        _logger.LogError("Failed to retrieve days of the week: {Error}", result.Error);
                    }
                }
                

                await Task.Delay(3000000, cancellationToken);
            }



        }
   
    }
}

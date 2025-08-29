namespace TG.CarParkEsher.Booking
{
    public sealed class CalenderWorkerService : BackgroundService
    {
        private readonly ILogger<CalenderWorkerService> _logger;

        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IHostEnvironment _hostEnvironment;
        public CalenderWorkerService(ILogger<CalenderWorkerService> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
            _hostEnvironment = serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IHostEnvironment>();
        }
        protected async override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await Task.Yield();
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var calenderService = scope.ServiceProvider.GetRequiredService<ICalenderService>();
                var result = await calenderService.SeedDaysOfWeekTable(cancellationToken);
                if (result.IsSuccess)
                {

                }
                else
                {
                    _logger.LogError("Failed to retrieve days of the week: {Error}", result.Error);
                }
            }
            while (!cancellationToken.IsCancellationRequested)
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var calenderService = scope.ServiceProvider.GetRequiredService<ICalenderService>();
                    var result = await calenderService.UpdateWeekDaysAsync(cancellationToken);
                    if (result.IsSuccess)
                    {
      
                    }
                    else
                    {
                        _logger.LogError("Failed to retrieve days of the week: {Error}", result.Error);
                    }
                }

                int daysUntilMonday = ((int)DayOfWeek.Monday - (int)DateTime.Today.DayOfWeek + 7) % 7;
                if (daysUntilMonday == 0)
                {
                    // Today is Monday
                    daysUntilMonday = 7;
                }
                if (_hostEnvironment.IsDevelopment())
                {
                    // In development, wait for 1 minute
                    await Task.Delay(TimeSpan.FromMinutes(5), cancellationToken);
                }
                else
                {
                    // In production, wait until next Monday
                    await Task.Delay(TimeSpan.FromDays(daysUntilMonday), cancellationToken);
                }
                    
            }



        }
   
    }
}

namespace TG.CarParkEsher.Booking
{
    public sealed class CalenderWorkerService : BackgroundService
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


                await Task.Delay(3000000, cancellationToken);
            }



        }
   
    }
}

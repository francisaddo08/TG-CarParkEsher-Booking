

namespace TG.CarParkEsher.Booking
{
    public class LoggingService : ILoggingService
    {
    private readonly ILogger _logger;
        public LoggingService(ILogger<LoggingService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public void LogError(string message, Exception ex)
        {
            _logger.LogError(ex, message);
        }
        public void LogInfo(string message, params object[] parameters)
        {
            _logger.LogInformation(message, parameters);
        }
        public void LogWarning(string message, params object[] parameters)
        {
            _logger.LogWarning(message, parameters);
        }
    }
}

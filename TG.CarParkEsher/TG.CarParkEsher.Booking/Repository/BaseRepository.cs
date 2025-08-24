using Microsoft.Extensions.Options;

namespace TG.CarParkEsher.Booking.Repository
{
    internal abstract class BaseRepository
    {
        private readonly ILogger<BaseRepository> _logger;
        private readonly IOptionsMonitor<ConnectionOption> _connectionOption;
     
        protected BaseRepository(ILogger<BaseRepository> logger, IOptionsMonitor<ConnectionOption> connectionOption)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _connectionOption = connectionOption ?? throw new ArgumentNullException(nameof(connectionOption));
        }
    }
}

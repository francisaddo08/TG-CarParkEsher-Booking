using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace TG.CarParkEsher.Booking
{
    public abstract class BaseRepository
    {
        protected readonly ILogger<BaseRepository> _logger;
        private readonly IOptionsMonitor<ConnectionOption> _connectionOption;

        protected BaseRepository(ILogger<BaseRepository> logger, IOptionsMonitor<ConnectionOption> connectionOption)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _connectionOption = connectionOption ?? throw new ArgumentNullException(nameof(connectionOption));
        }
        protected SqliteConnection GetConnection()
        {
            var connectionString = _connectionOption.CurrentValue.ConnectionString;
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                _logger.LogError("Connection string is not configured.");
                throw new InvalidOperationException("Connection string is not configured.");
                
            }
            var connection = new SqliteConnection(connectionString);
          
            return connection;
        }
    }
}

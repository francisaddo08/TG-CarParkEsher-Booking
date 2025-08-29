using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;


namespace TG.CarParkEsher.Booking
{
    public abstract class BaseRepository
    {
        protected readonly ILogger<BaseRepository> _logger;
        private readonly IOptionsMonitor<ConnectionOption> _connectionOption;
        private readonly IWebHostEnvironment _env;

        protected BaseRepository(ILogger<BaseRepository> logger, IOptionsMonitor<ConnectionOption> connectionOption, IWebHostEnvironment webHost)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _connectionOption = connectionOption ?? throw new ArgumentNullException(nameof(connectionOption));
            _env = webHost ?? throw new ArgumentNullException(nameof(webHost));
        }
        protected SqliteConnection GetConnection()
        {


            string dbFilePath = Path.Combine(_env.ContentRootPath, "CarParkEsher.db");

            // Build the connection string with the absolute path
            var connectionString = $"Data Source={dbFilePath};Mode=ReadWriteCreate;";

            // Use the connection string to create the SqliteConnection
            


            if (string.IsNullOrWhiteSpace(connectionString))
            {
                _logger.LogError("Connection string is not configured.");
                throw new InvalidOperationException("Connection string is not configured.");
                
            }
            
            var connection = new SqliteConnection(connectionString);
          
            return connection;
        }
        static string GetRootDirectory()
        {
            var d = Directory.GetCurrentDirectory();
            while (!File.Exists(Path.Combine(d, "CarParkEsher.db")))
            {
                d = Path.GetDirectoryName(d);
            }

            return d;
        }
        private  string GetFilePath(IWebHostEnvironment env, string fileName)
        {
            // Combine the content root path with the file name
            return Path.Combine(env.ContentRootPath, fileName);
        }
    }
}

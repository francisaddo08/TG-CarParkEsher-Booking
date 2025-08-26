using CSharpFunctionalExtensions;
using Microsoft.Extensions.Options;

namespace TG.CarParkEsher.Booking
{
    internal sealed class CalenderRepository : BaseRepository, ICalenderRepository
    {
        public CalenderRepository(ILogger<BaseRepository> logger, IOptionsMonitor<ConnectionOption> connectionOption) : base(logger, connectionOption)
        {
        }
        public async Task<Result<List<EsherCarParkDayInfo>>> GetDaysOfWeek(CancellationToken cancellationToken)
        {
            List<EsherCarParkDayInfo> daysOfWeek = new List<EsherCarParkDayInfo>();
            try
            {
                using (var connection = GetConnection())
                {
                    await connection.OpenAsync(cancellationToken);
                    var command = connection.CreateCommand();
                    command.CommandText = @"SELECT id, dayname, daynumber, datevalue FROM daysofweek ORDER BY daynumber";
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            var id = reader.GetInt32(0);
                            var dayName = reader.GetString(1);
                            var dayNumber = reader.GetInt32(2);
                            var dateValue = reader.GetDateTime(3);
                            daysOfWeek.Add(new EsherCarParkDayInfo(dayName, dayNumber, dateValue));
                        }
                    }

                }

            }
            catch (Exception ex)
            {

                return Result.Failure<List<EsherCarParkDayInfo>>($"An error occurred while retrieving days of the week.{ex.Message} {ex.InnerException?.Message}");
            }
            return Result.Success<List<EsherCarParkDayInfo>>(daysOfWeek);
        }

    }
}

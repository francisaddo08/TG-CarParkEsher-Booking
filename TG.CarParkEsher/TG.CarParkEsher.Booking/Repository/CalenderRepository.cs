using CSharpFunctionalExtensions;
using Microsoft.Extensions.Options;
using TG.CarParkEsher.Booking.Domain.Primitives;

namespace TG.CarParkEsher.Booking
{
    public sealed class CalenderRepository : BaseRepository, ICalenderRepository
    {
        public CalenderRepository(ILoggingService logger, IOptionsMonitor<ConnectionOption> connectionOption, IWebHostEnvironment webHost) : base(logger, connectionOption, webHost)
        {
        }
        public async Task<Result<bool>> SeedDaysOfWeekTable(HashSet<CarParkEsherDayInfo> esherCarParkDayInfos, CancellationToken cancellationToken)
        {
            bool isDayOfWeekTableSeeded = false;
            try
            {
                using (var connection = GetConnection())
                {
                    await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                    using (var transaction = connection.BeginTransaction())
                    {


                        var command = connection.CreateCommand();
                        command.CommandText = @"SELECT id FROM daysofweek LIMIT 1";
                        await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isDayOfWeekTableSeeded = true;
                            }
                        }
                        if (!isDayOfWeekTableSeeded)
                        {


                            command.CommandText = @"INSERT INTO daysofweek (dayname, daynumber, datevalue) VALUES ($dayname, $daynumber, $datevalue)";
                            var dayNameParam = command.CreateParameter();
                            dayNameParam.ParameterName = "$dayname";
                            command.Parameters.Add(dayNameParam);
                            var dayNumberParam = command.CreateParameter();
                            dayNumberParam.ParameterName = "$daynumber";
                            command.Parameters.Add(dayNumberParam);
                            var dateValueParam = command.CreateParameter();
                            dateValueParam.ParameterName = "$datevalue";
                            command.Parameters.Add(dateValueParam);
                            foreach (var dayInfo in esherCarParkDayInfos)
                            {
                                dayNameParam.Value = dayInfo.DayName;
                                dayNumberParam.Value = dayInfo.DayNumber;
                                dateValueParam.Value = dayInfo.DateValue;
                                await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                            }
                        }
                        command.Parameters.Clear();

                        bool isWeeklyParkingSpaceTableSeeded = false;
                        command.CommandText = @"SELECT parkingspace_id FROM weeklyparkingspace LIMIT 1";
                        await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isDayOfWeekTableSeeded = true;
                            }
                        }
                        if (isWeeklyParkingSpaceTableSeeded)
                        {

                            command.Parameters.Clear();
                            command.CommandText = @"SELECT parkingspaceid  FROM parkingspace";
                            List<int> parkingSpaceIds = new List<int>();
                            using (var reader = command.ExecuteReader())
                            {

                                while (reader.Read())
                                {
                                    parkingSpaceIds.Add(reader.GetInt32(0));

                                }

                            }
                            command.Parameters.Clear();
                            command.CommandText = @"INSERT INTO  weeklyparkingspace (parkingspace_id, dayofweek_id) VALUES ($parkingspaceid, $weekdayid)";
                            var parkingSpaceIdParam = command.CreateParameter();
                            parkingSpaceIdParam.ParameterName = "$parkingspaceid";
                            command.Parameters.Add(parkingSpaceIdParam);

                            var weekDayIdParam = command.CreateParameter();
                            weekDayIdParam.ParameterName = "$weekdayid";
                            command.Parameters.Add(weekDayIdParam);

                            foreach (var parkingSpaceId in parkingSpaceIds)
                            {
                                foreach (var dayInfo in esherCarParkDayInfos)
                                {
                                    parkingSpaceIdParam.Value = parkingSpaceId;
                                    weekDayIdParam.Value = dayInfo.DayNumber;
                                    await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                                }
                            }
                        }
                        transaction.Commit();

                    }
                }

            }
            catch (Exception ex)
            {

                return Result.Failure<bool>($"An error occurred while retrieving days of the week.{ex.Message} {ex.InnerException?.Message}");
            }
            return Result.Success<bool>(true);

        }
        public async Task<Result<bool>> UpdateDaysOfWeek(HashSet<CarParkEsherDayInfo> esherCarParkDayInfos, CancellationToken cancellationToken)
        {

            try
            {
                using (var connection = GetConnection())
                {
                    await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                    using (var transaction = connection.BeginTransaction())
                    {

                        var command = connection.CreateCommand();

                        await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);

                        command.CommandText = @"UPDATE daysofweek SET datevalue = $datevalue";


                        var dateValueParam = command.CreateParameter();
                        dateValueParam.ParameterName = "$datevalue";
                        command.Parameters.Add(dateValueParam);

                        var dayofweekidParam = command.CreateParameter();
                        dayofweekidParam.ParameterName = "$dayofweekid";
                        command.Parameters.Add(dayofweekidParam);

                        foreach (var dayInfo in esherCarParkDayInfos)
                        {
                            command.CommandText = @"UPDATE daysofweek SET datevalue = $datevalue WHERE id =$dayofweekid";

                            dateValueParam.Value = dayInfo.DateValue;
                            dayofweekidParam.Value = dayInfo.DayNumber;
                            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                        }





                        transaction.Commit();

                    }
                }

            }
            catch (Exception ex)
            {

                return Result.Failure<bool>($"An error occurred while retrieving days of the week.{ex.Message} {ex.InnerException?.Message}");
            }
            return Result.Success<bool>(true);
        }
        public async Task<Result<HashSet<CarParkEsherDayInfo>>> GetDaysOfWeek(CancellationToken cancellationToken)
        {
            HashSet<CarParkEsherDayInfo> daysOfWeek = new HashSet<CarParkEsherDayInfo>();
            try
            {
                using (var connection = GetConnection())
                {
                    await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
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
                            daysOfWeek.Add(new CarParkEsherDayInfo(dayName, dayNumber, dateValue));
                        }
                    }

                }

            }
            catch (Exception ex)
            {

                return Result.Failure<HashSet<CarParkEsherDayInfo>>($"An error occurred while retrieving days of the week.{ex.Message} {ex.InnerException?.Message}");
            }
            return Result.Success<HashSet<CarParkEsherDayInfo>>(daysOfWeek);
        }

    }
}

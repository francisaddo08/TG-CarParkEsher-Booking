using CSharpFunctionalExtensions;
using Microsoft.Extensions.Options;

namespace TG.CarParkEsher.Booking.HostingExtensions
{
    internal sealed class BookingRepository : BaseRepository, IBookingRepository
    {
        public BookingRepository(ILogger<BaseRepository> logger, IOptionsMonitor<ConnectionOption> connectionOption) : base(logger, connectionOption)
        {
        }
       internal async Task<bool>  UpdateDaysOfWeek()
        {
            try
            {
                using (var connection = GetConnection())
                {
                    await connection.OpenAsync().ConfigureAwait(false);
                    var command = connection.CreateCommand();
                    command.CommandText = "UPDATE days_of_week SET is_active = 1 WHERE day_name IN ('Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday')";
                    var rowsAffected = await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating days of week.");
                return false;
            }
        }
        public async Task<Result<List<EsherCarParkAvailableSlotResponse>>> GetAvailableSlotsAsync(string carParkId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken)
        {
            List<EsherCarParkAvailableSlotResponse>? availableSlots = new List<EsherCarParkAvailableSlotResponse>();
            try
            {
                using (var connection = GetConnection())
                {
                    await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                    var command = connection.CreateCommand();
                    command.CommandText = "SELECT slot_id, car_park_id, start_time, end_time FROM available_slots WHERE car_park_id = @carParkId AND start_time >= @startTime AND end_time <= @endTime";
                    command.Parameters.AddWithValue("@carParkId", carParkId);
                    command.Parameters.AddWithValue("@startTime", startTime);
                    command.Parameters.AddWithValue("@endTime", endTime);
                    using (var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
                    {
                        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                        {
                            var slotId = reader.GetString(0);
                            var carParkIdFromDb = reader.GetString(1);
                            var slotStartTime = reader.GetDateTime(2);
                            var slotEndTime = reader.GetDateTime(3);
                            availableSlots.Add(EsherCarParkAvailableSlotResponse.Create(slotId, carParkIdFromDb, slotStartTime, slotEndTime));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Result.Failure<List<EsherCarParkAvailableSlotResponse>>($"An error occurred while retrieving available slots: {ex.Message}");
            }
            return Result.Success(availableSlots);
        }
    }
}
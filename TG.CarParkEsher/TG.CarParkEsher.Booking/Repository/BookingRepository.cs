using CSharpFunctionalExtensions;
using Microsoft.Extensions.Options;

namespace TG.CarParkEsher.Booking
{
    public sealed class BookingRepository : BaseRepository, IBookingRepository
    {
        public BookingRepository(ILogger<BaseRepository> logger, IOptionsMonitor<ConnectionOption> connectionOption) : base(logger, connectionOption)
        {
        }
        public async Task<Result<CarParkEsherBooking?>> CreateBookingAsync(CarParkEsherBooking bookingForCreate, CancellationToken cancellationToken)
        {
            CarParkEsherBooking? carParkEsherBooking = null;
            try
            {
                using (var connection = GetConnection())
                {
                    await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                    using (var transaction = connection.BeginTransaction())
                    {

                        var command = connection.CreateCommand();


                        command.CommandText = @"INSERT INTO booking (bookee_id, dateofbooking, dayofweek_id, parkingspace_id, parkingstructure_id) 
                                                VALUES ($bookeeid, $dateofbooking, $dayofweekid, $parkingspaceid, $parkingstructureid)";
                        var bookeeIdParam = command.CreateParameter();
                        bookeeIdParam.ParameterName = "$bookeeid";
                        command.Parameters.Add(bookeeIdParam);

                        var dateOfBookingParam = command.CreateParameter();
                        dateOfBookingParam.ParameterName = "$dateofbooking";
                        command.Parameters.Add(dateOfBookingParam);

                        var dateBookedIdParam = command.CreateParameter();
                        dateBookedIdParam.ParameterName = "$dayofweekid";
                        command.Parameters.Add(dateBookedIdParam);

                        var parkingSpaceIdParam = command.CreateParameter();
                        parkingSpaceIdParam.ParameterName = "$parkingspaceid";
                        command.Parameters.Add(parkingSpaceIdParam);
                        var parkingStructureIdParam = command.CreateParameter();
                        parkingStructureIdParam.ParameterName = "$parkingstructureid";
                        command.Parameters.Add(parkingStructureIdParam);

                        bookeeIdParam.Value = bookingForCreate.BookeeId;
                        dateOfBookingParam.Value = DateTime.UtcNow;
                        dateBookedIdParam.Value = bookingForCreate.DateBookedId;
                        parkingSpaceIdParam.Value = bookingForCreate.ParkingSpaceId;
                        parkingStructureIdParam.Value = bookingForCreate.ParkingStructureId;
                        await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);

                        command.CommandText = @"SELECT BookingId, EmployeeId,ContactId,FirstName,LastName,ParkingSpace,DayName,DateValue
                                               FROM  v_employee_contact_booking
                                               WHERE ContactId = $ContactId
                                                ORDER BY BookingId DESC
                                                LIMIT 1";
                        var contactIdParam = command.CreateParameter();
                        contactIdParam.ParameterName = "$ContactId";
                        contactIdParam.Value = bookingForCreate.BookeeId;
                        command.Parameters.Add(contactIdParam);


                        using (var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                            {
                                var bookingId = reader.GetInt32(reader.GetOrdinal("BookingId"));
                                var employeeId = reader.GetString(reader.GetOrdinal("EmployeeId"));
                                var contactId = reader.GetInt32(reader.GetOrdinal("ContactId"));
                                var firstName = reader.GetString(reader.GetOrdinal("FirstName"));
                                var lastName = reader.GetString(reader.GetOrdinal("LastName"));
                                var parkingSpace = reader.GetInt32(reader.GetOrdinal("ParkingSpace"));
                                var dayName = reader.GetString(reader.GetOrdinal("DayName"));
                                var dateValue = reader.GetDateTime(reader.GetOrdinal("DateValue"));
                                carParkEsherBooking = new CarParkEsherBooking(bookingId, contactId, dateValue, dayName, parkingSpace, bookingForCreate.ParkingStructureId);

                            }
                        }

                        transaction.Commit();

                    }
                }

            }
            catch (Exception ex)
            {

                return Result.Failure<CarParkEsherBooking?>($"An error occurred while retrieving days of the week.{ex.Message} {ex.InnerException?.Message}");
            }
            if (carParkEsherBooking == null)
            {
                return Result.Failure<CarParkEsherBooking?>("Booking could not be created.");
            }
            return Result.Success<CarParkEsherBooking?>(carParkEsherBooking);

        }
        public async Task<bool> UpdateDaysOfWeek()
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
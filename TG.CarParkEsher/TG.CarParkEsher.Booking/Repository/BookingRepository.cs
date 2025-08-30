using CSharpFunctionalExtensions;
using Microsoft.Extensions.Options;
using TG.CarParkEsher.Booking.Domain.Primitives;

namespace TG.CarParkEsher.Booking
{
    public sealed class BookingRepository : BaseRepository, IBookingRepository
    {
        public BookingRepository(ILogger<BaseRepository> logger, IOptionsMonitor<ConnectionOption> connectionOption, IWebHostEnvironment webHost) : base(logger, connectionOption, webHost)
        {
        }
        public Task<Result<List<CarParkEsherDetail>>> GetPermittedParkingSpaces(bool blueBadge, bool ev, bool hybrid, DateTime startDate, DateTime? endDate, CancellationToken cancellationToken)
        {

            List<CarParkEsherDetail> carParkEsherDetails = new List<CarParkEsherDetail>();
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = @"SELECT * FROM v_avaliableparkingspaces ";
                    if (blueBadge)
                    {
                        command.CommandText += " WHERE  BlueBagde = $bluebage ";
                        var blueBadgeParam = command.CreateParameter();
                        blueBadgeParam.ParameterName = "$bluebage";
                        blueBadgeParam.Value = blueBadge;
                        command.Parameters.Add(blueBadgeParam);

                        var startDateParam = command.CreateParameter();
                        startDateParam.ParameterName = "$startdate";
                        startDateParam.Value = startDate.Date;
                        command.Parameters.Add(startDateParam);
                        if (endDate.HasValue) 
                        {
                            command.CommandText += " AND DateValue BETWEEN $startdate AND $enddate";
                            var endDateParam = command.CreateParameter();
                            endDateParam.ParameterName = "$enddate";
                            endDateParam.Value = endDate.Value.Date;
                            command.Parameters.Add(endDateParam);
                        }
                        else
                        {
                            command.CommandText += " AND DateValue = $startdate";
                        }
                        
                    }
                    else
                    if (ev)
                    {
                        command.CommandText += " WHERE  EV = $ev";
                        var evp = command.CreateParameter();
                        evp.ParameterName = "$ev";
                        evp.Value = ev;
                        command.Parameters.Add(evp);


                        var startDateParam = command.CreateParameter();
                        startDateParam.ParameterName = "$startdate";
                        startDateParam.Value = startDate.Date;
                        command.Parameters.Add(startDateParam);
                        if (endDate.HasValue)
                        {
                            command.CommandText += " AND DateValue BETWEEN $startdate AND $enddate";
                            var endDateParam = command.CreateParameter();
                            endDateParam.ParameterName = "$enddate";
                            endDateParam.Value = endDate.Value.Date;
                            command.Parameters.Add(endDateParam);
                        }
                        else
                        {
                            command.CommandText += " AND DateValue = $startdate";
                            
                        }

                    }
                    else
                    {
                        command.CommandText += "WHERE BlueBagde = 0 AND EV = 0";
                        var startDateParam = command.CreateParameter();
                        startDateParam.ParameterName = "$startdate";
                        startDateParam.Value = startDate.Date;
                        command.Parameters.Add(startDateParam);
                        if (endDate.HasValue)
                        {
                            command.CommandText += " AND DateValue BETWEEN $startdate AND $enddate";
                            var endDateParam = command.CreateParameter();
                            endDateParam.ParameterName = "$enddate";
                            endDateParam.Value = endDate.Value.Date;
                            command.Parameters.Add(endDateParam);
                        }
                        else
                        {
                            command.CommandText += " AND DateValue = $startdate";
                        }
                    }
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var id = reader.GetInt32(reader.GetOrdinal("ParkingSpaceId"));
                            var parkingStructureName = reader.GetInt32(reader.GetOrdinal("structurename"));
                            var blueBagde = reader.GetBoolean(reader.GetOrdinal("BlueBagde"));
                            var evExclusive = reader.GetBoolean(reader.GetOrdinal("Ev"));
                            var dateValue = reader.GetDateTime(reader.GetOrdinal("datevalue"));
                            var dateName = reader.GetString(reader.GetOrdinal("dayname"));

                            var vehicleTypes = Enumeration.GetAll<VehicleType>();
                            var vehicleTypeEnum = blueBagde ? vehicleTypes.FirstOrDefault(v => v.Name == VehicleType.BLUEBADGE.Name) :
                                                  evExclusive ? vehicleTypes.FirstOrDefault(v => v.Name == VehicleType.EV.Name) :
                                                  vehicleTypes.FirstOrDefault(v => v.Name == VehicleType.FOSSILFUEL.Name);

                           
                            carParkEsherDetails.Add(new CarParkEsherDetail( dateValue, dateName,id, vehicleTypeEnum is null ? string.Empty : vehicleTypeEnum.Name, vehicleTypeEnum is null ? string.Empty : vehicleTypeEnum.ColourCode));

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Task.FromResult(Result.Failure<List<CarParkEsherDetail>>($"An error occurred while retrieving parking bays.{ex.Message} {ex.InnerException?.Message}"));
            }
            if (!carParkEsherDetails.Any())
            {
                return Task.FromResult(Result.Failure<List<CarParkEsherDetail>>("No parking bays found."));
            }
            return Task.FromResult(Result.Success<List<CarParkEsherDetail>>(carParkEsherDetails));



        }
        public async Task<Result<DatabaseVerificationsFlags>> CheckParkingSpaceByIdAsync(int parkingSpaceId, DateTime dateBooked, bool bluebadge, bool ev, bool hybrid, CancellationToken cancellationToken)
        {
            DatabaseVerificationsFlags databaseVerificationsFlags = new DatabaseVerificationsFlags();
            if (parkingSpaceId <= 0)
            {
                return Result.Failure<DatabaseVerificationsFlags>("parking space id must be a positive integer.");
            }
            try
            {
                using (var connection = GetConnection())
                {
                    await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                    using (var transaction = connection.BeginTransaction())
                    {
                        var command = connection.CreateCommand();
                        command.CommandText = @"SELECT BookingId FROM v_employee_contact_booking WHERE  ParkingSpace = $ParkingSpaceId AND DateValue =$DateValue";

                        var parkingSpaceIdParam = command.CreateParameter();
                        parkingSpaceIdParam.ParameterName = "$ParkingSpaceId";
                        parkingSpaceIdParam.Value = parkingSpaceId;
                        command.Parameters.Add(parkingSpaceIdParam);

                        var dateBookedParam = command.CreateParameter();
                        dateBookedParam.ParameterName = "$DateValue";
                        dateBookedParam.Value = dateBooked.Date;
                        command.Parameters.Add(dateBookedParam);

                        using (var reader = await command.ExecuteReaderAsync(cancellationToken))
                        {
                            if (await reader.ReadAsync(cancellationToken))
                            {
                                int id = reader.GetInt32(reader.GetOrdinal("BookingId"));
                                databaseVerificationsFlags.IsParkingSpaceAvailable = reader.HasRows ? false : true;
                                databaseVerificationsFlags.IsDateAvailable = reader.HasRows ? false : true;
                            }
                        }
                        if (bluebadge)
                        {
                            command.CommandText = @"SELECT parkingspaceid 
                                                  FROM parkingspace 
                                                  WHERE parkingspaceid NOT IN(SELECT parkingspaceid FROM  v_parking_booking WHERE bluebadge =1) AND  bluebadge = 1";
                            using (var reader = await command.ExecuteReaderAsync(cancellationToken))
                            {
                                while (await reader.ReadAsync(cancellationToken))
                                {

                                    var id = reader.GetInt32(reader.GetOrdinal("parkingspaceid"));

                                    databaseVerificationsFlags.AvaliableBlueBadgeBays.Add(id);
                                    databaseVerificationsFlags.IsBlueBadgeValid = databaseVerificationsFlags.AvaliableBlueBadgeBays.Any() ? true : false;
                                }
                            }
                        }
                        else
                        if (ev)
                        {
                            command.CommandText = @"SELECT parkingspaceid 
                                                  FROM parkingspace 
                                                  WHERE parkingspaceid NOT IN(SELECT parkingspaceid FROM  v_parking_booking WHERE ev_exclusive =1) AND  ev_exclusive = 1";
                            using (var reader = await command.ExecuteReaderAsync(cancellationToken))
                            {
                                while (await reader.ReadAsync(cancellationToken))
                                {

                                    var id = reader.GetInt32(reader.GetOrdinal("parkingspaceid"));

                                    databaseVerificationsFlags.AvaliableEvBays.Add(id);
                                    databaseVerificationsFlags.IsEvValid = databaseVerificationsFlags.AvaliableBlueBadgeBays.Any() ? true : false;
                                }
                            }
                        }
                        else
                        if (hybrid)
                        {
                            command.CommandText = @"SELECT parkingspaceid 
                                                  FROM parkingspace 
                                                  WHERE parkingspaceid NOT IN(SELECT parkingspaceid FROM  v_parking_booking )";
                            using (var reader = await command.ExecuteReaderAsync(cancellationToken))
                            {
                                while (await reader.ReadAsync(cancellationToken))
                                {

                                    var id = reader.GetInt32(reader.GetOrdinal("parkingspaceid"));

                                    databaseVerificationsFlags.AvaliableStandardBays.Add(id);
                                    databaseVerificationsFlags.IsEvValid = databaseVerificationsFlags.AvaliableBlueBadgeBays.Any() ? true : false;
                                }
                            }
                        }
                        else
                        {
                            command.CommandText = @"SELECT parkingspaceid 
                                                  FROM parkingspace 
                                                  WHERE parkingspaceid NOT IN(SELECT parkingspaceid FROM  v_parking_booking ) AND  bluebadge = 0 AND ev_exclusive = 0";
                            using (var reader = await command.ExecuteReaderAsync(cancellationToken))
                            {
                                while (await reader.ReadAsync(cancellationToken))
                                {
                                    var id = reader.GetInt32(reader.GetOrdinal("parkingspaceid"));
                                    databaseVerificationsFlags.AvaliableStandardBays.Add(id);
                                    databaseVerificationsFlags.IsEvValid = databaseVerificationsFlags.AvaliableBlueBadgeBays.Any() ? true : false;
                                }
                            }
                        }
                        transaction.Commit();
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving account for username");
                return Result.Failure<DatabaseVerificationsFlags>($"{ex.Message}.{ex.InnerException?.Message}");
            }
            return Result.Success<DatabaseVerificationsFlags>(databaseVerificationsFlags);

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
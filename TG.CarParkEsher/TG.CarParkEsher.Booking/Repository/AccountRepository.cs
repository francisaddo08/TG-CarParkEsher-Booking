using CSharpFunctionalExtensions;
using Microsoft.Extensions.Options;
using System.Data;

namespace TG.CarParkEsher.Booking
{
    public class AccountRepository : BaseRepository, IAccountRepository
    {
        public AccountRepository(ILogger<BaseRepository> logger, IOptionsMonitor<ConnectionOption> connectionOption, IWebHostEnvironment webHost) : base(logger, connectionOption, webHost)
        {
        }
        public async Task<Result<bool>> GetAccountByContactIdAsync(int contactId, CancellationToken cancellationToken)
        {
            bool isFound = false;
            if (contactId <= 0)
            {
                return Result.Failure<bool>("Contact ID must be a positive integer.");
            }
            try
            {
                using (var connection = GetConnection())
                {
                    await connection.OpenAsync(cancellationToken);
                    var command = connection.CreateCommand();
                    command.CommandText = @"SELECT contact_id FROM account WHERE  contact_id = $ContactId";

                    var contactIdParam = command.CreateParameter();
                    contactIdParam.ParameterName = "$ContactId";
                    contactIdParam.Value = contactId;
                    command.Parameters.Add(contactIdParam);



                    using (var reader = await command.ExecuteReaderAsync(cancellationToken))
                    {
                        if (await reader.ReadAsync(cancellationToken))
                        {
                           int id = reader.GetInt32(reader.GetOrdinal("contact_id"));
                            isFound = reader.HasRows;


                        }
                    }
                }


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving account for username");
                return Result.Failure<bool>($"{ex.Message}.{ex.InnerException?.Message}");
            }
            return Result.Success<bool>(isFound);

        }
        public async Task<Result<CarParkEsherAccount?>> GetAccountByUsernameAsync(string username, CancellationToken cancellationToken)
        {
            CarParkEsherAccount? carParkEsherAccount = null;
            var bookings = new List<CarParkEsherBooking>();
            if (string.IsNullOrWhiteSpace(username))
            {
                return Result.Failure<CarParkEsherAccount?>("Username cannot be empty.");
            }
            try
            {
                using (var connection = GetConnection())
                {
                    await connection.OpenAsync(cancellationToken);
                    var command = connection.CreateCommand();
                    command.CommandText = @"SELECT v_employee_contact_account.EmployeeId, v_employee_contact_account.ContactId, v_employee_contact_account.FirstName, v_employee_contact_account.LastName, 
                                               VehicleType, Salt, PasswordHash, IsActive, IsBlocked, 
                                               ParkingSpace,DayName,DateValue ,BookingId
                                               FROM v_employee_contact_account
                                               LEFT JOIN v_employee_contact_booking ON v_employee_contact_account.ContactId = v_employee_contact_booking.ContactId
                                               WHERE  v_employee_contact_account.EmployeeId = $EmployeeId";

                    var employeeIdParam = command.CreateParameter();
                    employeeIdParam.ParameterName = "$EmployeeId";
                    employeeIdParam.Value = username;
                    command.Parameters.Add(employeeIdParam);

                    using (var reader = await command.ExecuteReaderAsync(cancellationToken))
                    {
                        if (await reader.ReadAsync(cancellationToken))
                        {
                            var empId = reader.IsDBNull(reader.GetOrdinal("EmployeeId")) ? string.Empty : reader.GetString(reader.GetOrdinal("EmployeeId"));
                            var contactId = reader.GetInt32(reader.GetOrdinal("ContactId"));
                            var vehicleType = reader.IsDBNull(reader.GetOrdinal("VehicleType")) ? string.Empty : reader.GetString(reader.GetOrdinal("VehicleType"));
                            var salt = reader.IsDBNull(reader.GetOrdinal("Salt")) ? string.Empty : reader.GetString(reader.GetOrdinal("Salt"));
                            var passwordHash = reader.IsDBNull(reader.GetOrdinal("PasswordHash")) ? string.Empty : reader.GetString(reader.GetOrdinal("PasswordHash"));

                            var fName = reader.GetString(reader.GetOrdinal("FirstName"));
                            var lName = reader.GetString(reader.GetOrdinal("LastName"));
                            var isActive = reader.GetBoolean(reader.GetOrdinal("IsActive"));
                            var isBlocked = reader.GetBoolean(reader.GetOrdinal("IsBlocked"));
                            carParkEsherAccount = new  CarParkEsherAccount(0,contactId, vehicleType, string.Empty, passwordHash, fName, lName, empId, salt, isActive, isBlocked);

                        }
                        
                        do
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal("BookingId")))
                            {
                                var bookingId = reader.GetInt32(reader.GetOrdinal("BookingId"));
                                var parkingSpace = reader.GetInt32(reader.GetOrdinal("ParkingSpace"));
                                var dayName = reader.GetString(reader.GetOrdinal("DayName"));
                                var dateValue = reader.GetDateTime(reader.GetOrdinal("DateValue"));
                                bookings.Add(new CarParkEsherBooking(bookingId, carParkEsherAccount!.ContactId, dateValue, dayName, parkingSpace, 0));
                            }
                        } while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false));
                        

                    }
                }

                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving account for username {Username}", username);
                return  Result.Failure<CarParkEsherAccount?>($"{ex.Message}.{ex.InnerException?.Message}");
            }
            if (carParkEsherAccount == null)
            {
                return  Result.Failure<CarParkEsherAccount?>("Account not found.");
            }
            carParkEsherAccount.Bookings = bookings;
            return Result.Success<CarParkEsherAccount?>(carParkEsherAccount);

        }
        public async Task<Result<CarParkEsherAccount?>> CreateAccountAsync(CarParkEsherAccount carParkEsherAccountValue, CancellationToken cancellationToken)
        {
          CarParkEsherAccount? carParkEsherAccount = null;
            try
            {
                using (var connection = GetConnection())
                {
                    await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                    using (var transaction = connection.BeginTransaction())
                    {
                        var command = connection.CreateCommand();
                        command.CommandText = @"INSERT INTO account (contact_id, vehicletype, password, salt, passwordhash) VALUES($contactid, $vehicletype,  $password, $salt, $passwordhash)";
                        var contactIdParam = command.CreateParameter();
                        contactIdParam.ParameterName = "$contactid";
                        command.Parameters.Add(contactIdParam);

                        var vehicleTypeParam = command.CreateParameter();
                        vehicleTypeParam.ParameterName = "$vehicletype";
                        command.Parameters.Add(vehicleTypeParam);

                       var passwordParam = command.CreateParameter();
                        passwordParam.ParameterName = "$password";
                        command.Parameters.Add(passwordParam);

                        var passwordSaltParam = command.CreateParameter();
                        passwordSaltParam.ParameterName = "$salt";
                        command.Parameters.Add(passwordSaltParam);

                        var passwordHashParam = command.CreateParameter();
                        passwordHashParam.ParameterName = "$passwordhash";
                        command.Parameters.Add(passwordHashParam);

                       
                        contactIdParam.Value = carParkEsherAccountValue.ContactId;
                        vehicleTypeParam.Value = carParkEsherAccountValue.VehicleType;
                        passwordParam.Value = carParkEsherAccountValue.Password;
                        passwordSaltParam.Value = carParkEsherAccountValue.Salt;
                        passwordHashParam.Value = carParkEsherAccountValue.PasswordHash;
                       
                       
                        await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);


                        command.CommandText = @"SELECT  FirstName, LastName
                                                FROM v_employee_contact
                                                WHERE contactid = $contactid
                                                LIMIT 1";
                        using (var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
                        {
                            while (reader.Read())
                            {
                               
                              
                                var firstName = reader.GetString(reader.GetOrdinal("FirstName"));
                                var lastName = reader.GetString(reader.GetOrdinal("LastName"));
                                
                                carParkEsherAccount = new CarParkEsherAccount(0, 0, string.Empty, string.Empty, string.Empty, firstName, lastName, string.Empty, string.Empty, true, false);
                            }

                        }
                        transaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating account for ContactId {ContactId}", carParkEsherAccountValue.ContactId);
                return Result.Failure<CarParkEsherAccount?>($"{ex.Message}.{ex.InnerException?.Message}");
            }
            if (carParkEsherAccount == null)
            {
                return Result.Failure<CarParkEsherAccount?>("Account could not be created.");
            }
            return Result.Success<CarParkEsherAccount?>(carParkEsherAccount);
        }

        public async Task<Result<CarParkEsherEmployeeContact>> GetAccountAsync(string firstName, string lastName, string emplyeeId, CancellationToken cancellationToken)
        {
            CarParkEsherEmployeeContact? contact = null;
            try
            {
                using (var connection = GetConnection())
                {
                    await connection.OpenAsync(cancellationToken);
                    var command = connection.CreateCommand();
                    command.CommandText = @"SELECT EmployeeId, ContactId, FirstName, LastName FROM v_employee_contact WHERE FirstName = @FirstName AND LastName = @LastName AND EmployeeId = @EmployeeId";

                    var employeeIdParam = command.CreateParameter();
                    employeeIdParam.ParameterName = "@EmployeeId";
                    employeeIdParam.Value = emplyeeId;
                    command.Parameters.Add(employeeIdParam);

                    var contactIdParam = command.CreateParameter();
                    contactIdParam.ParameterName = "@ContactId";
                    contactIdParam.Value = emplyeeId;
                    command.Parameters.Add(contactIdParam);

                    var firstNameParam = command.CreateParameter();
                    firstNameParam.ParameterName = "@FirstName";
                    firstNameParam.Value = firstName;
                    command.Parameters.Add(firstNameParam);

                    var lastNameParam = command.CreateParameter();
                    lastNameParam.ParameterName = "@LastName";
                    lastNameParam.Value = lastName;
                    command.Parameters.Add(lastNameParam);

                    using (var reader = await command.ExecuteReaderAsync(cancellationToken))
                    {
                        if (await reader.ReadAsync(cancellationToken))
                        {
                            var empId = reader.GetString(reader.GetOrdinal("EmployeeId"));
                            var contactId = reader.GetInt32(reader.GetOrdinal("ContactId"));
                            var fName = reader.GetString(reader.GetOrdinal("FirstName"));
                            var lName = reader.GetString(reader.GetOrdinal("LastName"));
                            contact = new CarParkEsherEmployeeContact(empId, contactId, fName, lName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving account for {FirstName} {LastName} with EmployeeId {EmployeeId}", firstName, lastName, emplyeeId);
                return Result.Failure<CarParkEsherEmployeeContact>("An error occurred while retrieving the account.");
            }
            if (contact == null)
            {
                return Result.Failure<CarParkEsherEmployeeContact>("Account not found.");
            }
            return Result.Success<CarParkEsherEmployeeContact>(contact);
        }

        
    }
}
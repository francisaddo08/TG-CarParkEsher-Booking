using CSharpFunctionalExtensions;
using Microsoft.Extensions.Options;

namespace TG.CarParkEsher.Booking
{
    public class AccountRepository : BaseRepository, IAccountRepository
    {
        public AccountRepository(ILogger<BaseRepository> logger, IOptionsMonitor<ConnectionOption> connectionOption) : base(logger, connectionOption)
        {
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

                        //var firstNameParam = command.CreateParameter();
                        //firstNameParam.ParameterName = "$firstname";
                        //command.Parameters.Add(firstNameParam);

                        //var lastNameParam = command.CreateParameter();
                        //lastNameParam.ParameterName = "$lastname";
                        //command.Parameters.Add(lastNameParam);

                        //var employeeIdParam = command.CreateParameter();
                        //employeeIdParam.ParameterName = "$employeeid";
                        //command.Parameters.Add(employeeIdParam);

                        contactIdParam.Value = carParkEsherAccountValue.ContactId;
                        vehicleTypeParam.Value = carParkEsherAccountValue.VehicleType;
                        passwordParam.Value = carParkEsherAccountValue.Password;
                        passwordSaltParam.Value = carParkEsherAccountValue.Salt;
                        passwordHashParam.Value = carParkEsherAccountValue.PasswordHash;
                       
                       

                        //firstNameParam.Value = value.FirstName;
                        //lastNameParam.Value = value.LastName;
                        //employeeIdParam.Value = value.EmployeeId;
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
                                
                                carParkEsherAccount = new CarParkEsherAccount(0, 0, string.Empty, string.Empty, string.Empty, firstName, lastName, string.Empty, string.Empty);
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
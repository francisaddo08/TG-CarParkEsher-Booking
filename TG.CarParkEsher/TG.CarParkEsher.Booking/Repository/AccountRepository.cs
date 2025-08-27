using CSharpFunctionalExtensions;
using Microsoft.Extensions.Options;

namespace TG.CarParkEsher.Booking
{
    public class AccountRepository : BaseRepository, IAccountRepository
    {
        public AccountRepository(ILogger<BaseRepository> logger, IOptionsMonitor<ConnectionOption> connectionOption) : base(logger, connectionOption)
        {
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
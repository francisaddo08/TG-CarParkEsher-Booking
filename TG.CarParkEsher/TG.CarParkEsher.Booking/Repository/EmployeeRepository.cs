using CSharpFunctionalExtensions;
using Microsoft.Extensions.Options;

namespace TG.CarParkEsher.Booking
{
    public sealed class EmployeeRepository : BaseRepository
    {
        public EmployeeRepository(ILogger<BaseRepository> logger, IOptionsMonitor<ConnectionOption> connectionOption, IWebHostEnvironment webHost) : base(logger, connectionOption, webHost)
        {
        }

        public async Task<Result<List<TGEmployee>>> GetEmployeesAsync(CancellationToken cancellationToken)
        {
            List<TGEmployee>? employees = new List<TGEmployee>();
            try
            {
                using (var connection = GetConnection())
                {
                   await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                    var command = connection.CreateCommand();
                    command.CommandText = "SELECT employeeid, contact_id FROM employee";
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var employeesResult = new List<TGEmployee>();
                        while (reader.Read())
                        {
                            var employeeId = reader.GetString(0);
                            var contactId = reader.GetInt32(1);
                            employees.Add(new TGEmployee(employeeId, contactId));
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                return Result.Failure<List<TGEmployee>>($"An error occurred while retrieving employees: {ex.Message}: {ex.InnerException?.Message}");
            }
            return Result.Success<List<TGEmployee>>(employees);
        }
   
    }
}

using Microsoft.Extensions.Options;

namespace TG.CarParkEsher.Booking
{
    internal sealed class EmployeeRepository : BaseRepository
    {
        public EmployeeRepository(ILogger<BaseRepository> logger, IOptionsMonitor<ConnectionOption> connectionOption) : base(logger, connectionOption)
        {
        }
        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            var sql = "SELECT * FROM Employees";
            return await QueryAsync<Employee>(sql);
        }
    }
}

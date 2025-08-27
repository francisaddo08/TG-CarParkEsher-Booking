
using CSharpFunctionalExtensions;

namespace TG.CarParkEsher.Booking
{
    public interface IAccountRepository
    {
        Task<Result<CarParkEsherEmployeeContact>> GetAccountAsync(string firstName, string lastName, string emplyeeId, CancellationToken cancellationToken);
    }
}
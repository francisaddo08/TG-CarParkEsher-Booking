
using CSharpFunctionalExtensions;

namespace TG.CarParkEsher.Booking
{
    public interface IAccountRepository
    {
        Task<Result<CarParkEsherAccount>> CreateAccountAsync(CarParkEsherAccount value, CancellationToken cancellationToken);
        Task<Result<CarParkEsherEmployeeContact>> GetAccountAsync(string firstName, string lastName, string emplyeeId, CancellationToken cancellationToken);
        Task<Result<CarParkEsherAccount?>> GetAccountByUsernameAsync(string username, CancellationToken cancellationToken);
    }
}
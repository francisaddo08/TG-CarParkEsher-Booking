
namespace TG.CarParkEsher.Booking
{
    public interface IAccountRepository
    {
        Task Result<> GetAccountAsync(string firstName, string lastName, string emplyeeId, CancellationToken cancellationToken);
    }
}
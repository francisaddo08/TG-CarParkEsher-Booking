using CSharpFunctionalExtensions;

namespace TG.CarParkEsher.Booking
{
    public interface IAccountService
    {
        Task<ContextResult<EsherCarParkrRegistrationResponseDto>> CreateAccountAsync(EsherCarParkRegistrationRequestDto request, CancellationToken cancellationToken);
        Task<Result<CarParkEsherAccount?>> GetAccountByUsernameAsync(string username, CancellationToken cancellationToken);
        Task<Result<bool>> ValidateUserCredentialsAsync(string userName, string password, CancellationToken cancellationToken);
    }
}
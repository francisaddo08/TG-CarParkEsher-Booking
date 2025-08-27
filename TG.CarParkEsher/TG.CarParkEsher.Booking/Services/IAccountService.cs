namespace TG.CarParkEsher.Booking
{
    public interface IAccountService
    {
        Task<ContextResult<EsherCarParkrRegistrationResponseDto>> CreateAccountAsync(EsherCarParkRegistrationRequestDto request, CancellationToken cancellationToken);
    }
}
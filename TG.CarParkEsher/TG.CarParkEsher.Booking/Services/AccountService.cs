using TG.CarParkEsher.Booking.Domain.Entities;

namespace TG.CarParkEsher.Booking
{
    public sealed class AccountService : IAccountService
    {
        private readonly ILogger<AccountService> _logger;
        private readonly IAccountRepository _userRepository;
        public AccountService(ILogger<AccountService> logger, IAccountRepository userRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<ContextResult<EsherCarParkrRegistrationResponseDto>> CreateAccountAsync(EsherCarParkRegistrationRequestDto request, CancellationToken cancellationToken)
        {
          var esherCarParkRegistrationResponse = EsherCarParkrRegistrationResponseDto.Create( request);
            if (!esherCarParkRegistrationResponse.Valid)
            {
                return ContextResult<EsherCarParkrRegistrationResponseDto>.Failure(esherCarParkRegistrationResponse, false);
            }
            var foundEmployee = await _userRepository.GetAccountAsync(request.FirstName, request.LastName, request.EmplyeeId, cancellationToken);
            if (foundEmployee.IsFailure)
            {
                _logger.LogWarning("Account not found for {FirstName} {LastName} with EmployeeId {EmployeeId}", request.FirstName, request.LastName, request.EmplyeeId);
                return ContextResult<EsherCarParkrRegistrationResponseDto>.Failure("Registration failed Problem with your details, Ensure your details are correct", true);
            }
            var newAccountForCreate = newAccount(foundEmployee.Value, request);




        }

        private async Task<Result<CarParkEsherAccount>> object newAccount(CarParkEsherEmployeeContact value, EsherCarParkRegistrationRequestDto request) 
        {
            throw new NotImplementedException();
        }
    }
}
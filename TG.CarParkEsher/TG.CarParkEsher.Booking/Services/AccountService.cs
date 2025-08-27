namespace TG.CarParkEsher.Booking.HostingExtensions
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
            var existingUser = await _userRepository.GetAccountAsync(request.FirstName, request.LastName, request.EmplyeeId, cancellationToken);



        }
    }
}
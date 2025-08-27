using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using TG.CarParkEsher.Booking.Models.Error;

namespace TG.CarParkEsher.Booking
{
    public sealed class AccountService : IAccountService
    {
        private readonly ILogger<AccountService> _logger;
        private readonly IAccountRepository _userRepository;
        private readonly IPasswordHasher<CarParkEsherAccount> _passwordHasher;
        public AccountService(ILogger<AccountService> logger, IAccountRepository userRepository, IPasswordHasher<CarParkEsherAccount> passwordHasher)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        }

        public async Task<ContextResult<EsherCarParkrRegistrationResponseDto>> CreateAccountAsync(EsherCarParkRegistrationRequestDto request, CancellationToken cancellationToken)
        {
            var esherCarParkRegistrationResponse = EsherCarParkrRegistrationResponseDto.Create(request);
            if (!esherCarParkRegistrationResponse.Valid)
            {
                return ContextResult<EsherCarParkrRegistrationResponseDto>.Failure(esherCarParkRegistrationResponse, false);
            }
            var foundEmployee = await _userRepository.GetAccountAsync(request.FirstName, request.LastName, request.EmplyeeId, cancellationToken);
            if (foundEmployee.IsFailure)
            {
                _logger.LogWarning("Account not found for {FirstName} {LastName} with EmployeeId {EmployeeId}", request.FirstName, request.LastName, request.EmplyeeId);
                var errors = new List<ErrorDto> { new ErrorDto { ErrorID = "AccountNotFound", ErrorDetail = "No account found with the provided details. Please ensure your details are correct." } };
                esherCarParkRegistrationResponse.SetValidation(false, errors);
                return ContextResult<EsherCarParkrRegistrationResponseDto>.Failure(esherCarParkRegistrationResponse, true);
            }
            var newAccountForCreate = await newAccount(foundEmployee.Value, request);
            if (newAccountForCreate.IsFailure)
            {
                _logger.LogError("Failed to create new account for {FirstName} {LastName} with EmployeeId {EmployeeId}: {Error}", request.FirstName, request.LastName, request.EmplyeeId, newAccountForCreate.Error);
                var errors = new List<ErrorDto> { new ErrorDto { ErrorID = "RegistrationFailed", ErrorDetail = "Problem with details, make sure correct details are supplied" } };
                return ContextResult<EsherCarParkrRegistrationResponseDto>.Failure( esherCarParkRegistrationResponse, true);
            }
            var createdAccount = await _userRepository.CreateAccountAsync(newAccountForCreate.Value, cancellationToken);
            if (createdAccount.IsFailure)
            {
                _logger.LogError("Failed to save new account for {FirstName} {LastName} with EmployeeId {EmployeeId}: {Error}", request.FirstName, request.LastName, request.EmplyeeId, createdAccount.Error);
                var errors = new List<ErrorDto> { new ErrorDto { ErrorID = "RegistrationFailed", ErrorDetail = ErrorMessages.InternalServerError } };
                esherCarParkRegistrationResponse.SetValidation(false, errors);
                return ContextResult<EsherCarParkrRegistrationResponseDto>.Failure( esherCarParkRegistrationResponse, true);
            }
            return ContextResult<EsherCarParkrRegistrationResponseDto>.Success(esherCarParkRegistrationResponse);




        }

        private async Task<Result<CarParkEsherAccount>> newAccount(CarParkEsherEmployeeContact esherEmployeeContact, EsherCarParkRegistrationRequestDto request)
        {
            string salt = GenerateSalt();
            string saltedPassword = salt + request.Password;
            var newAccountResult = CarParkEsherAccount.Create(
                                                                     esherEmployeeContact.ContactId,
                                                                     request.VehicleType,
                                                                     request.Password,
                                                                     string.Empty,
                                                                     esherEmployeeContact.FirstName,
                                                                     esherEmployeeContact.LastName,
                                                                     esherEmployeeContact.EmployeeId,
                                                                     salt);
             
            if (newAccountResult.IsFailure)
            {
                _logger.LogError("Failed to create account entity: {Error}", newAccountResult.Error);
                return Result.Failure<CarParkEsherAccount>(newAccountResult.Error);
            }
            newAccountResult.Value.PasswordHash = _passwordHasher.HashPassword(newAccountResult.Value, saltedPassword);
            return Result.Success<CarParkEsherAccount>(newAccountResult.Value);


        }
        private static string GenerateSalt(int size = 16)
        {
            var saltBytes = new byte[size];
            RandomNumberGenerator.Fill(saltBytes);
            return Convert.ToBase64String(saltBytes);
        }
    }
}
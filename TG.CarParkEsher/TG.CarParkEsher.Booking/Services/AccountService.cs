using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using TG.CarParkEsher.Booking.Models.Error;

namespace TG.CarParkEsher.Booking
{
    public sealed class AccountService : IAccountService
    {
        private readonly ILogger<AccountService> _logger;
        private readonly IAccountRepository _accountRepository;
        private readonly IPasswordHasher<CarParkEsherAccount> _passwordHasher;
        public AccountService(ILogger<AccountService> logger, IAccountRepository userRepository, IPasswordHasher<CarParkEsherAccount> passwordHasher)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _accountRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        }
        public async Task<Result<CarParkEsherAccount?>> GetAccountByUsernameAsync(string username, CancellationToken cancellationToken)
        {
            return await _accountRepository.GetAccountByUsernameAsync(username, cancellationToken);

        }
        public async Task<Result<bool>> ValidateUserCredentialsAsync(string userName, string password, CancellationToken cancellationToken)
        {
            var _userName = (userName ?? string.Empty).Trim();
            var _password = (password ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(_userName) ||
                string.IsNullOrWhiteSpace(_password))
            {
                Result.Failure<bool>("Invalid username or password");
            }
            var userResult = await _accountRepository.GetAccountByUsernameAsync( _userName, cancellationToken);
            if (userResult.IsFailure)
            {
                return Result.Failure<bool>(userResult.Error);
            }
            var _user = userResult.Value;
            if (_user == null)
            {
                return Result.Failure<bool>("Invalid username or password");
            }

           

            if (!_user.IsActive)
            {
                return Result.Failure<bool>("Invalid user account");
            }
            if (_user.IsLocked)
            {
                return Result.Failure<bool>("User account locked out");
            }
            var expectedPassword = _passwordHasher.HashPassword(_user, _password);
            var verificationResult = _passwordHasher.VerifyHashedPassword(_user, _user.Password, _password);

            if (verificationResult == PasswordVerificationResult.Success)
            {
                return Result.Success<bool>(true);
            }
            return Result.Failure<bool>("Invalid username or password");
        }
        public async Task<ContextResult<EsherCarParkrRegistrationResponseDto>> CreateAccountAsync(EsherCarParkRegistrationRequestDto request, CancellationToken cancellationToken)
        {
            var esherCarParkRegistrationResponse = EsherCarParkrRegistrationResponseDto.Create(request);
            if (!esherCarParkRegistrationResponse.Valid)
            {
                return ContextResult<EsherCarParkrRegistrationResponseDto>.Failure(esherCarParkRegistrationResponse, false);
            }
            var foundEmployee = await _accountRepository.GetAccountAsync(request.FirstName, request.LastName, request.EmplyeeId, cancellationToken);
            if (foundEmployee.IsFailure)
            {
                _logger.LogWarning("Account not found for {FirstName} {LastName} with EmployeeId {EmployeeId}", request.FirstName, request.LastName, request.EmplyeeId);
                var errors = new List<ErrorDto> { new ErrorDto { ErrorID = "AccountNotFound", ErrorDetail = "No account found with the provided details. Please ensure your details are correct." } };
                esherCarParkRegistrationResponse.SetValidation(false, errors);
                return ContextResult<EsherCarParkrRegistrationResponseDto>.Failure(esherCarParkRegistrationResponse, true);
            }
            var existingAccount = await _accountRepository.GetAccountByContactIdAsync(foundEmployee.Value.ContactId, cancellationToken);
            if (existingAccount.IsFailure)
            {
                _logger.LogError("Failed to check existing account for ContactId {ContactId}: {Error}", foundEmployee.Value.ContactId, existingAccount.Error);
                var errors = new List<ErrorDto> { new ErrorDto { ErrorID = "RegistrationFailed", ErrorDetail = ErrorMessages.InternalServerError } };
                esherCarParkRegistrationResponse.SetValidation(false, errors);
                return ContextResult<EsherCarParkrRegistrationResponseDto>.Failure(esherCarParkRegistrationResponse, true);
            }
            if(existingAccount.Value)
            {
                _logger.LogWarning("Account already exists for {FirstName} {LastName} with EmployeeId {EmployeeId}", request.FirstName, request.LastName, request.EmplyeeId);
                var errors = new List<ErrorDto> { new ErrorDto { ErrorID = "AccountExists", ErrorDetail = ErrorMessages.AccountCreationConflict} };
                esherCarParkRegistrationResponse.SetValidation(false, errors);
                return ContextResult<EsherCarParkrRegistrationResponseDto>.Failure(esherCarParkRegistrationResponse, false);
            }
            var newAccountForCreate = await newAccount(foundEmployee.Value, request);
            if (newAccountForCreate.IsFailure)
            {
                _logger.LogError("Failed to create new account for {FirstName} {LastName} with EmployeeId {EmployeeId}: {Error}", request.FirstName, request.LastName, request.EmplyeeId, newAccountForCreate.Error);
                var errors = new List<ErrorDto> { new ErrorDto { ErrorID = "RegistrationFailed", ErrorDetail = "Problem with details, make sure correct details are supplied" } };
                return ContextResult<EsherCarParkrRegistrationResponseDto>.Failure( esherCarParkRegistrationResponse, true);
            }
            var createdAccount = await _accountRepository.CreateAccountAsync(newAccountForCreate.Value, cancellationToken);
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
                                                                     salt, true, false);
             
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
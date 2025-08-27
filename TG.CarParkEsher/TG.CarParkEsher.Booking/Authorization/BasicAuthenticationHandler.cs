using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Encodings.Web;

namespace TG.CarParkEsher.Booking
{
    public sealed class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IAccountService _accountService;
        private readonly IPasswordHasher<CarParkEsherAccount> _passwordHasher;
        public BasicAuthenticationHandler
        (
          IOptionsMonitor<AuthenticationSchemeOptions> options,
          ILoggerFactory logger,
          UrlEncoder encoder,
          IAccountService accountService,
            IPasswordHasher<CarParkEsherAccount> passwordHasher

          ) : base(options, logger, encoder)
        {
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        }
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return await Task.FromResult(AuthenticateResult.Fail("Missing Authorization Header"));
            }
            try
            {
                CarParkEsherAccount? user = null;
                var authHeader = Request.Headers["Authorization"].ToString();
                var authHeaderValue = authHeader.Substring("Basic".Length).Trim();
                var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(authHeaderValue)).Split(":");
                var username = credentials[0];
                var password = credentials[1];
                var _userName = (username ?? string.Empty).Trim();
                var _password = (password ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(_userName) ||
                    string.IsNullOrWhiteSpace(_password))
                {
                    AuthenticateResult.Fail("Invalid username or password");
                }

                var accountResult = await _accountService.GetAccountByUsernameAsync(_userName, new CancellationTokenSource().Token);
                if (accountResult.IsSuccess)
                {
                    user = accountResult.Value;
                }
                var validationResult = await _accountService.ValidateUserCredentialsAsync(_userName, _password, new CancellationTokenSource().Token);

                if (validationResult.IsFailure)
                {
                    return AuthenticateResult.Fail(validationResult.Error);
                }

                if (user != null && validationResult.Value == true)
                {
                   
                }
                else
                {
                    return AuthenticateResult.Fail("Invalid username or password");
                }
            }
            catch
            {
                return AuthenticateResult.Fail("Invalid Authorization Header"));
            }

        }
    }
}

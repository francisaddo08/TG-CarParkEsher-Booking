using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Newtonsoft.Json;

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
                return  AuthenticateResult.Fail("Missing Authorization Header");
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
                   var bookings =   JsonConvert.SerializeObject(user.Bookings);
                    var claims = new List<Claim>()
                    {
                       new Claim(UserClaimTypes.BookSlot , "enabled"),
                       new Claim(UserClaimTypes.ViewAvailableSlot , "enabled"),
                       new Claim( UserClaimTypes.ContactId, user.ContactId.ToString()),
                       new Claim( UserClaimTypes.Bookings, bookings),
                    };
                    var permitedVehicleTypes = Enumeration.GetAll<VehicleType>().FirstOrDefault(vt => vt.Name.Equals(user.VehicleType, StringComparison.InvariantCultureIgnoreCase));
                    if (permitedVehicleTypes != null)
                    {
                        if (permitedVehicleTypes.Name.Equals("BLUEBADGE", StringComparison.OrdinalIgnoreCase))
                        {
                            claims.Add(new Claim(UserClaimTypes.BlueBadge, "true"));
                        }
                        if (permitedVehicleTypes.Name.Equals("EV", StringComparison.OrdinalIgnoreCase))
                        {
                            claims.Add(new Claim(UserClaimTypes.EV, "true"));
                        }
                        if (permitedVehicleTypes.Name.Equals("HYBRID", StringComparison.OrdinalIgnoreCase))
                        {
                            claims.Add(new Claim(UserClaimTypes.Hybrid, "true"));
                        }

                    }
                    
                    var identity = new ClaimsIdentity(claims, Scheme.Name);
                    var principal = new ClaimsPrincipal(identity);
                    var _ticket = new AuthenticationTicket(principal, Scheme.Name);
                    return AuthenticateResult.Success(_ticket);
                }
                else
                {
                    return AuthenticateResult.Fail("Invalid username or password");
                }
            }
            catch
            {
                return AuthenticateResult.Fail("Invalid Authorization Header");
            }

        }
    }
}

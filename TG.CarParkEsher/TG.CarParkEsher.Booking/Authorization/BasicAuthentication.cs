using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;

namespace TG.CarParkEsher.Booking.Authorization
{
    public sealed class BasicAuthentication : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public BasicAuthentication
        (
          IOptionsMonitor<AuthenticationSchemeOptions> options,
          ILoggerFactory logger, 
          UrlEncoder encoder

          ) : base(options, logger, encoder)
        {
        }
        protected override Task<AuthenticateResult> HandleAuthenticateAsync() => throw new NotImplementedException();
    }
}

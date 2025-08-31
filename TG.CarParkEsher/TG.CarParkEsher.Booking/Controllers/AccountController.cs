using Microsoft.AspNetCore.Mvc;

namespace TG.CarParkEsher.Booking
{
    [Tags("Account")]
    [Route("api/v1-0/tg")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ILoggingService _logger;
        private readonly IAccountService _accountService;
        public AccountController(ILoggingService logger, IAccountService accountService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
        }
        [HttpPost("account/create-account")]
        public async Task<ActionResult<EsherCarParkrRegistrationResponseDto>> CreateAccountAsync(EsherCarParkRegistrationRequestDto request, CancellationToken cancellationToken)
        {
            var userResult = await _accountService.CreateAccountAsync(request, cancellationToken);
            if (userResult.IsFailure)
            {
                if (userResult.IsServerError)
                {

                    return StatusCode(StatusCodes.Status500InternalServerError, userResult.Result);
                }
                return StatusCode(StatusCodes.Status406NotAcceptable, userResult.Result);
            }
            return Ok(userResult.Result);
        }
    }
}

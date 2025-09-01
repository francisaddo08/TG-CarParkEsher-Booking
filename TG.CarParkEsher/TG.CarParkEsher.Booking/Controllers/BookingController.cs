        using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TG.CarParkEsher.Booking
{
    [Tags("Booking")]
    [Route("api/v1-0/tg")]
    [ApiController]
    [Authorize("TGEmployeePolicy")]
    public class BookingController : ControllerBase
    {
        private readonly ILogger<BookingController> _logger;
        private readonly IBookingService _bookingService;
        public BookingController(ILogger<BookingController> logger, IBookingService bookingService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _bookingService = bookingService ?? throw new ArgumentNullException(nameof(bookingService));
        }
        [HttpPost("booking/create")]
        public async Task<ActionResult<EsherCarParkBookingResponseDto>> BookParkingSpaceAsync(EsherCarParkBookingRequestDto request, CancellationToken cancellationToken)
        {
            var bookingresult = await _bookingService.BookParkingSpaceAsync(request, cancellationToken);
            if (bookingresult.IsFailure)
            {
                if (bookingresult.IsServerError)
                {

                    return StatusCode(StatusCodes.Status500InternalServerError, bookingresult.Result);
                }
                return StatusCode(StatusCodes.Status406NotAcceptable, bookingresult.Result);
            }
            return Ok(bookingresult.Result);
        }
        [HttpPost("booking/avaliable-parkingspaces")]
        public async Task<ActionResult<EsherCarParkBookingResponseDto>> GetAvaliableParkingSpacesAsync(EsherCarParkAvaliableBayRequestDto request, CancellationToken cancellationToken)
        {
            var bookingresult = await _bookingService.GetAllAvaliableParkingSpacesAsync(request, cancellationToken);
            if (bookingresult.IsFailure)
            {
                if (bookingresult.IsServerError)
                {

                    return StatusCode(StatusCodes.Status500InternalServerError, bookingresult.Result);
                }
                return StatusCode(StatusCodes.Status406NotAcceptable, bookingresult.Result);
            }
            if ( bookingresult.IsSuccess && !bookingresult.Result.Any())
            {
                return StatusCode(StatusCodes.Status404NotFound, bookingresult.Result);
            }
            return Ok(bookingresult.Result);
        }
    }
}

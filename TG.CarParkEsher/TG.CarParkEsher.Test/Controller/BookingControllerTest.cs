using FakeItEasy;
using Microsoft.Extensions.Logging;
using TG.CarParkEsher.Booking;
using TG.CarParkEsher.Booking.Controllers;

namespace TG.CarParkEsher.Test.Controller
{
    public class BookingControllerTest
    {
        private readonly IBookingService _bookingService;
        private readonly ILogger<BookingController> _logger;

        public BookingControllerTest()
        {
            _bookingService = A.Fake<IBookingService>();
            _logger = A.Fake<ILogger<BookingController>>();
        }
        [Fact]
        public void BookingController_CreateBookingAsync_Returns()
        {
            // Arrange
            var booking = A.Fake<EsherCarParkBookingResponseDto>();
            var controller = new BookingController(_logger, _bookingService);


            //Act
            var result = controller.CreateBookingAsync(A.Fake<EsherCarParkBookingRequestDto>(), CancellationToken.None);
            //Assert

        }
    }
}

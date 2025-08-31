using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using TG.CarParkEsher.Booking;

namespace TG.CarParkEsher.Test.Controller
{
    public class BookingControllerTest
    {
        private IBookingService _bookingService;
        private readonly ILogger<BookingController> _logger;

        public BookingControllerTest()
        {
            var ioptionMonitorCon = A.Fake<Microsoft.Extensions.Options.IOptionsMonitor<ConnectionOption>>();
            var webHost = A.Fake<Microsoft.AspNetCore.Hosting.IWebHostEnvironment>();
            var baseRepositoryLogger = A.Fake<ILogger<BaseRepository>>();

            //var repoFake = A.Fake<IBookingRepository>(x =>
            //            x.WithArgumentsForConstructor(new object[] { baseRepositoryLogger, ioptionMonitorCon, webHost }));
            //var httpcontextAccessor = A.Fake<Microsoft.AspNetCore.Http.IHttpContextAccessor>();

            //_bookingService = A.Fake<IBookingService>(x => x.WithArgumentsForConstructor(new object[] { repoFake, httpcontextAccessor }));
            _bookingService = A.Fake<IBookingService>();
            _logger = A.Fake<ILogger<BookingController>>();
        }
        [Fact]
        public void BookingController_CreateBookingAsync_ReturnsOk()
        {
            // Arrange
            var requestDto = new EsherCarParkBookingRequestDto() { DateBooked = DateTime.Now, ParkingSpaceId = 1, ParkingStructureId = 1 };
            var cancellationToken = new CancellationTokenSource().Token;
            var r = A.Fake<IBookingRepository>();
            var bs = new BookingService(r, A.Fake<Microsoft.AspNetCore.Http.IHttpContextAccessor>());


            var controller = new BookingController(_logger, bs);

            // Act
            var result = controller.CreateBookingAsync(requestDto, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            //result.Should().BeOfType<Microsoft.AspNetCore.Mvc.ActionResult<EsherCarParkBookingResponseDto>>();
        }
    }
}

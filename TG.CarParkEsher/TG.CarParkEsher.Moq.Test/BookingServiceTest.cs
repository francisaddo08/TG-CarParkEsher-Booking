using Microsoft.AspNetCore.Http;
using Moq;
using TG.CarParkEsher.Booking;

namespace TG.CarParkEsher.Moq.Test
{
    public class BookingServiceTest
    {
    private readonly BookingService _sut;
        private readonly Mock<IBookingRepository> _bookingRepositoryMock = new Mock<IBookingRepository>();
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        public BookingServiceTest()
        {
            
            _sut = new BookingService(_bookingRepositoryMock.Object, _httpContextAccessorMock.Object);
        }
        [Fact]
        public async Task CreateBooking_ShouldReturn_EsherCarParkrRegistrationResponseDto_WhenCreatedSuccessfully()
        {
            //Arrange
            var bookingRequest = new EsherCarParkBookingRequestDto
            {
                DateBooked = DateTime.UtcNow.AddDays(1),
                ParkingSpaceId = 1,
                ParkingStructureId = 1

            };
            //Act
            //Assert

        }
    }
}

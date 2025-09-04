using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;
using TG.CarParkEsher.Booking;
using TG.CarParkEsher.Booking.Domain.Primitives;

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
            var bookingResponse = EsherCarParkBookingResponseDto.Create(bookingRequest);

            // Mock HttpContext and User Claims
            var mockHttpContext = new DefaultHttpContext();
            int userId = 1; // Example user ID
            string VehicleType = "ev"; // Example vehicle type
            var claims = new List<Claim>
               {
                   new Claim(UserClaimTypes.BookSlot , "enabled"),
                   new Claim(UserClaimTypes.ViewAvailableSlot , "enabled"),
                   new Claim( UserClaimTypes.ContactId,userId.ToString() ),
                     new Claim( UserClaimTypes.EV, VehicleType ),
                   new Claim(UserClaimTypes.Bookings, "[{\"Id\":1,\"ParkingSpaceId\":1,\"ParkingStructureId\":1}]")
               };
            var mockIdentity = new ClaimsIdentity(claims);
            var mockPrincipal = new ClaimsPrincipal(mockIdentity);
            mockHttpContext.User = mockPrincipal;

            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(mockHttpContext);

            var bookings = _httpContextAccessorMock.Object.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == UserClaimTypes.Bookings)?.Value;
            var bookingsList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CarParkEsherBooking>>(bookings ?? "[]");

            var blueBadge = _httpContextAccessorMock.Object.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == UserClaimTypes.BlueBadge)?.Value == "true";
            var ev = _httpContextAccessorMock.Object.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == UserClaimTypes.EV)?.Value == "true";
            var hybrid = _httpContextAccessorMock.Object.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == UserClaimTypes.Hybrid)?.Value == "true";

            var cancellationToken = new CancellationToken();
            _bookingRepositoryMock.Setup(repo => repo.CheckParkingSpaceByIdAsync(bookingRequest.ParkingSpaceId, bookingRequest.DateBooked, blueBadge, ev, hybrid, cancellationToken))
                .ReturnsAsync(Result.Success(new DatabaseVerificationsFlags
                {
                    AvaliableBlueBadgeParkingSpace = new List<int>(){ 1},
                    AvaliableEvParkingSpace = new List<int>() { },
                    AvaliableHybridParkingSpace = new List<int>() { },
                    AvaliableStandardParkingSpace = new List<int>() { },
                    IsEvValid = true,

                }));
                
            var createdBooking = CarParkEsherBooking.Create(userId, bookingRequest.DateBooked, bookingRequest.ParkingSpaceId, bookingRequest.ParkingStructureId);
         
            _bookingRepositoryMock.Setup(repo =>  repo.CreateBookingAsync(It.IsAny<CarParkEsherBooking>(), cancellationToken))
                .ReturnsAsync(Result.Success(createdBooking.Value));

            // Create a temporary booking to use in the Act section
            var temp = CarParkEsherBooking.Create(1,  DateTime.UtcNow,  1, 1);
            //Act
          
            var result = await _sut.BookParkingSpaceAsync(bookingRequest, cancellationToken);
            //Assert
            // Assert.(ContextResult<EsherCarParkBookingResponseDto>.Success(bookingResponse));
        }
    }
}

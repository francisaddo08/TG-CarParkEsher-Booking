using CSharpFunctionalExtensions;
using TG.CarParkEsher.Booking.Services.Result;

namespace TG.CarParkEsher.Booking.HostingExtensions
{
    internal sealed class BookingService
    {
     private readonly IBookingRepository _bookingRepository;
        public BookingService(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository ?? throw new ArgumentNullException(nameof(bookingRepository));
        }
        public async Task<ContextResult<EsherCarParkBookingResponseDto>> BookSlotAsync(EsherCarParkBookingRequestDto bookingRequest, CancellationToken cancellationToken)
        {
            var bookingResponse = EsherCarParkBookingResponseDto.Create(bookingRequest);
            if (!bookingResponse.Valid)
            {
                return ContextResult<EsherCarParkBookingResponseDto>.Failure(bookingResponse);
            }
            try
            {
                //var isBooked = await _bookingRepository.BookSlotAsync(bookingRequest.ParkingSpaceId, bookingRequest.DateBooked, cancellationToken);
                //if (!isBooked)
                //{
                //    return ContextResult<EsherCarParkBookingResponseDto>.Failure("The parking space is already booked for the selected date.");
                //}
                return ContextResult<EsherCarParkBookingResponseDto>.Success(bookingResponse);
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                return ContextResult<EsherCarParkBookingResponseDto>.Failure("An error occurred while processing your booking request.", true);
            }

        }

    }
}
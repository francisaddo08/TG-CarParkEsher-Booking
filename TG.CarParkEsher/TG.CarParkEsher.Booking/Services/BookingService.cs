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
        public async Task<ContextResult<bool>> BookSlotAsync(EsherCarParkBookingRequestDto bookingRequest, CancellationToken cancellationToken)
        {
          if (bookingRequest == null) 
          {
                return Result.Failure<bool>("Booking request cannot be null");
            }
            try
            {
                var result = await _bookingRepository.BookSlotAsync(bookingRequest, cancellationToken);
                if (result.IsFailure)
                {
                    return Result.Failure<bool>($"Failed to book slot: {result.Error}");
                }
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                return Result.Failure<bool>($"An error occurred while booking slot. {ex.Message} {ex.InnerException?.Message}");
            }
        }

    }
}
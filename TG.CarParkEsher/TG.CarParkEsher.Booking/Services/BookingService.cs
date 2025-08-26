using CSharpFunctionalExtensions;


namespace TG.CarParkEsher.Booking
{
    public sealed class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly int _defaultParkingStructureId = 1;
        public BookingService(IBookingRepository bookingRepository, IHttpContextAccessor httpContextAccessor)
        {
            _bookingRepository = bookingRepository ?? throw new ArgumentNullException(nameof(bookingRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }
        public async Task<ContextResult<EsherCarParkBookingResponseDto>> CreateBookSlotAsync(EsherCarParkBookingRequestDto bookingRequest, CancellationToken cancellationToken)
        {
            var esherCarParkBookingResponse = EsherCarParkBookingResponseDto.Create(bookingRequest);
            if (!esherCarParkBookingResponse.Valid)
            {
                return ContextResult<EsherCarParkBookingResponseDto>.Failure(esherCarParkBookingResponse);
            }
            var bookingForCreate = await NewBookingAsync(bookingRequest);
            if (bookingForCreate.IsFailure || bookingForCreate.Value is null)
            {
                return ContextResult<EsherCarParkBookingResponseDto>.Failure(bookingForCreate.Error, true);
            }
            var createdBooking = await CreateBookingAsync(bookingForCreate.Value, cancellationToken);
            if (createdBooking is null)
            {
                var errors = new List<ErrorDto>()
                { 
                  new ErrorDto { ErrorID = "BookingCreationFailed", ErrorDetail = "An error occurred while creating the booking. Please try again later." }
                };
                esherCarParkBookingResponse.SetValidation(false, errors );
                return ContextResult<EsherCarParkBookingResponseDto>.Failure(esherCarParkBookingResponse, true);
            }

            return ContextResult<EsherCarParkBookingResponseDto>.Success(esherCarParkBookingResponse);
        }
        private async Task<Result<CarParkEsherBooking>> NewBookingAsync(EsherCarParkBookingRequestDto bookingRequest)
        {

            return CarParkEsherBooking.Create(1, bookingRequest.DateBooked, bookingRequest.ParkingSpaceId, _defaultParkingStructureId);

        }
        private async Task<CarParkEsherBooking?> CreateBookingAsync(CarParkEsherBooking bookingForCreate, CancellationToken cancellationToken)
        {
            CarParkEsherBooking? booking = null;
            var bookingResult = await _bookingRepository.CreateBookingAsync(bookingForCreate, cancellationToken);
            if (bookingResult.IsSuccess && bookingResult.Value is not null)
            {
                booking = bookingResult.Value;
            }
            return booking;

        }


    }
}
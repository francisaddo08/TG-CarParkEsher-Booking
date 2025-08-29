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
        public async Task<ContextResult<List<EsherCarParkAvaliableBayResponseDto>>> GetAllAvaliableBaysAsync(EsherCarParkAvaliableBayRequestDto request, CancellationToken cancellationToken)
        {
            var bookings = _httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == UserClaimTypes.Bookings)?.Value;
            var bookingsList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CarParkEsherBooking>>(bookings ?? "[]");
            var blueBadge = _httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == UserClaimTypes.BlueBadge)?.Value == "true";
            var ev = _httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == UserClaimTypes.EV)?.Value == "true";
            var hybrid = _httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == UserClaimTypes.Hybrid)?.Value == "true";



            var avaliableParkingSpaces = await _bookingRepository.GetPermittedParkingSpaces(blueBadge, ev, hybrid, request.StartDate, request.EndDate, cancellationToken);
            if (avaliableParkingSpaces.IsFailure)
            {
                return ContextResult<List<EsherCarParkAvaliableBayResponseDto>>.Failure(avaliableParkingSpaces.Error);
            }

            EsherCarParkAvaliableBayResponseDto esherCarParkAvaliableBayResponse = new(true,null);


            esherCarParkAvaliableBayResponse.ParkingSpaces = avaliableParkingSpaces.Value.Select(p => new EsherCarParkAvaliableBayDetailDto(p.ParkingSpaceId, p.DateAvaliable, p.Day, p.VehicleType,p.ColourCode)  ).ToList();



            return ContextResult<List<EsherCarParkAvaliableBayResponseDto>>.Success(new List<EsherCarParkAvaliableBayResponseDto> { esherCarParkAvaliableBayResponse });



        }
        public async Task<ContextResult<EsherCarParkBookingResponseDto>> CreateBookSlotAsync(EsherCarParkBookingRequestDto bookingRequest, CancellationToken cancellationToken)
        {
            var esherCarParkBookingResponse = EsherCarParkBookingResponseDto.Create(bookingRequest);
            if (!esherCarParkBookingResponse.Valid)
            {
                return ContextResult<EsherCarParkBookingResponseDto>.Failure(esherCarParkBookingResponse);
            }
            var bookings = _httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == UserClaimTypes.Bookings)?.Value;
            var bookingsList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CarParkEsherBooking>>(bookings ?? "[]");
            if (bookingsList != null && bookingsList.Any(b => b.DateBooked.Value.Date == bookingRequest.DateBooked.Date))
            {
                var errors = new List<ErrorDto>()
                {
                  new ErrorDto { ErrorID = "DuplicateBooking", ErrorDetail = "You already have a booking for the selected date." }
                };
                esherCarParkBookingResponse.SetValidation(false, errors);
                return ContextResult<EsherCarParkBookingResponseDto>.Failure(esherCarParkBookingResponse);
            }
            var blueBadge = _httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == UserClaimTypes.BlueBadge)?.Value == "true";
            var ev = _httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == UserClaimTypes.EV)?.Value == "true";
            var hybrid = _httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == UserClaimTypes.Hybrid)?.Value == "true";

            // var permitedBay = await _bookingRepository.GetPermittedParkingSpaces( );
            var bayResult = await _bookingRepository.CheckParkingSpaceByIdAsync(bookingRequest.ParkingSpaceId, bookingRequest.DateBooked, blueBadge, ev, hybrid, cancellationToken);
            if (bayResult.IsFailure)
            {
                var errors = new List<ErrorDto>()
                {
                  new ErrorDto { ErrorID = "InvalidParkingSpace", ErrorDetail = bayResult.Error }
                };
                esherCarParkBookingResponse.SetValidation(false, errors);
                return ContextResult<EsherCarParkBookingResponseDto>.Failure(esherCarParkBookingResponse);
            }
            if (!bayResult.Value.IsParkingSpaceAvailable)
            {
                var errors = new List<ErrorDto>()
                {
                  new ErrorDto { ErrorID = "ParkingSpaceNotAvailable", ErrorDetail = "The selected parking space is not available for the chosen date." }
                };
                esherCarParkBookingResponse.SetValidation(false, errors);
                return ContextResult<EsherCarParkBookingResponseDto>.Failure(esherCarParkBookingResponse);

            }
            if (bayResult.Value.IsBlueBadgeValid && !bayResult.Value.AvaliableBlueBadgeBays.Contains(bookingRequest.ParkingSpaceId))
            {
                var errors = new List<ErrorDto>()
                {
                  new ErrorDto { ErrorID = "InvalidBlueBadgeBay", ErrorDetail = "The selected parking space is not valid for Blue Badge holders." }
                };
                esherCarParkBookingResponse.SetValidation(false, errors);
                return ContextResult<EsherCarParkBookingResponseDto>.Failure(esherCarParkBookingResponse);

            }

            if (bayResult.Value.IsEvValid && !bayResult.Value.AvaliableEvBays.Contains(bookingRequest.ParkingSpaceId))
            {
                var errors = new List<ErrorDto>()
                {
                  new ErrorDto { ErrorID = "InvalidEvBay", ErrorDetail = "The selected parking space is not valid for Electric Vehicles." }
                };
                esherCarParkBookingResponse.SetValidation(false, errors);
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
                esherCarParkBookingResponse.SetValidation(false, errors);
                return ContextResult<EsherCarParkBookingResponseDto>.Failure(esherCarParkBookingResponse, true);
            }

            return ContextResult<EsherCarParkBookingResponseDto>.Success(esherCarParkBookingResponse);
        }

        private async Task<Result<CarParkEsherBooking>> NewBookingAsync(EsherCarParkBookingRequestDto bookingRequest)
        {
            var contactId = _httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == UserClaimTypes.ContactId)?.Value;
            if (string.IsNullOrWhiteSpace(contactId) || !int.TryParse(contactId, out int bookeeId))
            {
                return Result.Failure<CarParkEsherBooking>("Invalid contact id");
            }
            return CarParkEsherBooking.Create(bookeeId, bookingRequest.DateBooked, bookingRequest.ParkingSpaceId, _defaultParkingStructureId);

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
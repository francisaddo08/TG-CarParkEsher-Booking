using CSharpFunctionalExtensions;

namespace TG.CarParkEsher.Booking
{
    public sealed class EsherCarParkBookingResponseDto : RequestValidationResultDto
    {
        private EsherCarParkBookingResponseDto(bool valid, IList<ErrorDto>? errors, int parkingSpaceId, DateTime dateBooked)
            : base(valid, errors)
        {
            ParkingSpaceId = parkingSpaceId;
            DateBooked = dateBooked;
        }

        public static EsherCarParkBookingResponseDto Create(EsherCarParkBookingRequestDto esherCarParkBookingRequestDto)
        {
            var valid = true;
            var errors = new List<ErrorDto>();
            if (!IsValidDate(esherCarParkBookingRequestDto.DateBooked))
            {
              valid = false;
                errors.Add(new ErrorDto { ErrorID = "InvalidDate", ErrorDetail = "The booking date must be today or in the future." });
            }
            if (esherCarParkBookingRequestDto.ParkingSpaceId <= 0)
            {
                valid = false;
                errors.Add(new ErrorDto { ErrorID = "InvalidParkingSpaceId", ErrorDetail = "Parking Space ID must be a positive integer." });
            }
            errors = errors.Any() ? errors : null;
            return new EsherCarParkBookingResponseDto(valid, errors, esherCarParkBookingRequestDto.ParkingSpaceId, esherCarParkBookingRequestDto.DateBooked);

        }
        private static bool IsValidDate(DateTime dateBooked)
        {
            return dateBooked.Date >= DateTime.UtcNow.Date;
        }
        public int ParkingSpaceId { get; }
        public DateTime DateBooked { get; }
    }
}

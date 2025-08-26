using CSharpFunctionalExtensions;

namespace TG.CarParkEsher.Booking
{
    internal sealed class EsherCarParkBookingResponseDto : RequestValidationResultDto
    {
        private EsherCarParkBookingResponseDto(bool valid, IList<ErrorDto>? errors, int parkingSpaceId, DateTime dateBooked)
            : base(valid, errors)
        {
            ParkingSpaceId = parkingSpaceId;
            DateBooked = dateBooked;
        }

        internal static EsherCarParkBookingResponseDto Create(EsherCarParkBookingRequestDto esherCarParkBookingRequestDto)
        {
            var valid = true;
            var errors = new List<ErrorDto>();
            if (!IsValidDate(esherCarParkBookingRequestDto.DateBooked))
            {
              valid = false;
                errors.Add(new ErrorDto { ErrorID = "InvalidDate", ErrorDetail = "The booking date must be today or in the future." });
            }
            errors = errors.Any() ? errors : null;
            return new EsherCarParkBookingResponseDto(valid, errors, esherCarParkBookingRequestDto.ParkingSpaceId, esherCarParkBookingRequestDto.DateBooked);

        }
        private static bool IsValidDate(DateTime dateBooked)
        {
            return dateBooked.Date >= DateTime.UtcNow;
        }
        internal int ParkingSpaceId { get; }
        internal DateTime DateBooked { get; }
    }
}

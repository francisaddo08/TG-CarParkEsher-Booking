using CSharpFunctionalExtensions;

namespace TG.CarParkEsher.Booking
{
    internal sealed class EsherCarParkBookingResponseDto
    {
        private EsherCarParkBookingResponseDto(string parkingSpaceId, DateTime dateBooked, string? message)
        {
            Message = message;
            ParkingSpaceId = parkingSpaceId;
            DateBooked = dateBooked;
        }
        internal static Result<EsherCarParkBookingResponseDto> Create(int parkingSpaceId, DateTime dateBooked, string? message)
        {
            if (parkingSpaceId < 1 || parkingSpaceId > 35)
            {
                return Result.Failure<EsherCarParkBookingResponseDto>("Invalid parking space Id");
            }
            return new EsherCarParkBookingResponseDto(isBooked, message, parkingSpaceId);
        }

        internal string? Message { get; }
        internal string ParkingSpaceId { get; }
        internal DateTime DateBooked { get; }

    }
}

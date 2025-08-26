namespace TG.CarParkEsher.Booking
{
    internal sealed class EsherCarParkBookingRequestDto
    {
        internal required string ParkingSpaceId { get; set; } 
        internal DateTime DateBooked { get; set; }

    }
}

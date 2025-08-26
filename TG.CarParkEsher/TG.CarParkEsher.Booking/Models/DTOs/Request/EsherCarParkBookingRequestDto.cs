namespace TG.CarParkEsher.Booking
{
    public sealed class EsherCarParkBookingRequestDto
    {
        
        public  int  ParkingSpaceId { get; set; }
        public int ParkingStructureId { get; set; }
        public DateTime DateBooked { get; set; }

    }
}

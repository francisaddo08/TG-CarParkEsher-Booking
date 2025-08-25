namespace TG.CarParkEsher.Booking
{
    internal sealed class EsherCarParkBookingRequestDto
    {
        public string? EmployeeId { get; set; }
        public string? CarParkId { get; set; }
        public DateTime? BookingDate { get; set; }
        public string? BookingType { get; set; }
        public string? BookingStatus { get; set; }
        public string? BookingReference { get; set; }

    }
}

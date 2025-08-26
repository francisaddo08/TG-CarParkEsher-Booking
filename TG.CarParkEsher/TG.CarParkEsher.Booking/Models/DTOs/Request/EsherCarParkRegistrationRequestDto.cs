namespace TG.CarParkEsher.Booking.Models.DTOs.Request
{
    public sealed class EsherCarParkRegistrationRequestDto
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string  EmplyeeId { get; set; }
        public required List<string> VehicleType { get; set; }
        public required string Password { get; set; }
        public required string ConfirmPassword { get; set; }
    }
}
    
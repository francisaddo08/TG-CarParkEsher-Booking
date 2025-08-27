namespace TG.CarParkEsher.Booking
{
    public sealed class EsherCarParkRegistrationRequestDto
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string  EmplyeeId { get; set; }
        public required string VehicleType { get; set; }
        public required string Password { get; set; }
        public required string ConfirmPassword { get; set; }
    }
}
    
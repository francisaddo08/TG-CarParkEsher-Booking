namespace TG.CarParkEsher.Booking
{
    public sealed class EsherCarParkAvaliableBayDetailDto
    {
         public DateTime DateAvaliable { get; set; }
         public string  Day{ get; set; }
        public int ParkingSpaceId { get; set; }
        public string VehicleType { get; set; } = string.Empty;
        public string ColourCode { get; set; } = string.Empty;

    }

}

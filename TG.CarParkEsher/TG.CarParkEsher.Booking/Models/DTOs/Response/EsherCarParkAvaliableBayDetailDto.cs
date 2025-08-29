namespace TG.CarParkEsher.Booking
{
    public sealed class EsherCarParkAvaliableBayDetailDto
    {
        public EsherCarParkAvaliableBayDetailDto(int parkingSpaceId, DateTime dateAvaliable, string day, string vehicleType, string colourCode)
        {
            ParkingSpaceId = parkingSpaceId;
            DateAvaliable = dateAvaliable;
            Day = day;
            VehicleType = vehicleType;
            ColourCode = colourCode;
        }
        public DateTime DateAvaliable { get; }
         public string  Day{ get;  }
        public int ParkingSpaceId { get; }
        public string VehicleType { get; set; } 
        public string ColourCode { get; set; } 

    }

}

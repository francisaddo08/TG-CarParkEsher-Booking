namespace TG.CarParkEsher.Booking
{
    public sealed class VehicleType : Enumeration
    {
        public static VehicleType EV = new(1, "EV");
        public static VehicleType FOSSILFUEL = new(2, "FOSSIL");
        public static VehicleType HYBRID = new(3, "HYBRID");
        public static VehicleType BIKE = new(4, "BIKE");
        public static VehicleType MOTORBIKE = new(5, "MOTORBIKE");
        public static VehicleType BLUEBADGE = new(6, "BLUEBADGE");
        public VehicleType(int id, string name) : base(id, name)
        {
        }
      public string ColourCode
        {
            get
            {
                return Id switch
                {
                    1 => "#4CAF50", // Green for EV
                    2 => "#F44336", // Red for Fossil Fuel
                    3 => "#A52A2A", // Brown for Hybrid
                    4 => "#4CAF50", // Green for Bike
                    5 => "#4CAF50", // Green for Motor Bike
                    6 => "#2196F3", // Blue for Blue Badge
                    _ => "#000000", // Default to Black
                };
            }
        }
    }
}

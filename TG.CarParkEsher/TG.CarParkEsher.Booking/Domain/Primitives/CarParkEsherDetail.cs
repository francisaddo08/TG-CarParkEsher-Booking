using CSharpFunctionalExtensions;

namespace TG.CarParkEsher.Booking.Domain.Primitives
{
    public sealed class CarParkEsherDetail : ValueObject<CarParkEsherDetail>
    {
        public CarParkEsherDetail(DateTime dateAvaliable, string day, int parkingSpaceId, string vehicleType, string colourCode)
        {
            DateAvaliable = dateAvaliable;
            Day = day;
            ParkingSpaceId = parkingSpaceId;
            VehicleType = vehicleType;
            ColourCode = colourCode;
        }

        public DateTime DateAvaliable { get; }
        public string Day { get;}
        public int ParkingSpaceId { get; }
        public string VehicleType { get; }
        public string ColourCode { get;  }

        protected override bool EqualsCore(CarParkEsherDetail other)
            => (DateAvaliable, Day, ParkingSpaceId, VehicleType, ColourCode)
            == (other.DateAvaliable, other.Day, other.ParkingSpaceId, other.VehicleType, other.ColourCode);
        protected override int GetHashCodeCore() =>
            HashCode.Combine(DateAvaliable, Day, ParkingSpaceId, VehicleType, ColourCode);
    }
}

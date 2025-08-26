using CSharpFunctionalExtensions;

namespace TG.CarParkEsher.Booking
{
    public sealed class CarParkEsherBooking : Entity<int>
    {
        private static int DefaultBookingId = 0;
        public CarParkEsherBooking(int id, int bookeeId, DateTime dateBooked, string? dayName, int parkingSpaceId, int parkingStructureId)
        {
            Id = id;
            BookeeId = bookeeId;
            DateBookedId = (int) dateBooked.DayOfWeek  == 0 ? DomainContants.UKSundayDayNumber : (int)dateBooked.DayOfWeek;
            DateBooked = dateBooked;
            DayName = dayName;
            ParkingSpaceId = parkingSpaceId;
            ParkingStructureId = parkingStructureId;
        }
        public static Result<CarParkEsherBooking> Create(int bookeeId, DateTime dateBooked, int parkingSpaceId, int parkingStructureId)
        {
            if (bookeeId <= 0)
            {
                return Result.Failure<CarParkEsherBooking>("Bookee ID must be a positive integer.");
            }
            if (dateBooked.Date < DateTime.UtcNow.Date)
            {
                return Result.Failure<CarParkEsherBooking>("The booking date must be today or in the future.");
            }
            if (parkingSpaceId <= 0)
            {
                return Result.Failure<CarParkEsherBooking>("Parking Space ID must be a positive integer.");
            }
            if (parkingStructureId <= 0)
            {
                return Result.Failure<CarParkEsherBooking>("Parking Structure ID must be a positive integer.");
            }
            return Result.Success<CarParkEsherBooking>(new CarParkEsherBooking(DefaultBookingId, bookeeId, dateBooked,null, parkingSpaceId, parkingStructureId));
        }
        public int BookeeId { get; private set; }
        public int? DateBookedId { get; private set; }
        public DateTime? DateBooked { get; private set; }
        public string? DayName { get; private set; }
        public int ParkingSpaceId { get; private set; }
        public int ParkingStructureId { get; private set; }



    }
}

using CSharpFunctionalExtensions;

namespace TG.CarParkEsher.Booking
{
    public sealed class Booking : Entity<int>
    {
        private static int DefaultBookingId = 0;
        public Booking(int id, int bookeeId, DateTime dateBooked, int parkingSpaceId, int parkingStructureId)
        {
            Id = id;
            BookeeId = bookeeId;
            DateBooked = dateBooked;
            ParkingSpaceId = parkingSpaceId;
            ParkingStructureId = parkingStructureId;
        }
        public static Result<Booking> Create(int bookeeId, DateTime dateBooked, int parkingSpaceId, int parkingStructureId)
        {
            if (bookeeId <= 0)
            {
                return Result.Failure<Booking>("Bookee ID must be a positive integer.");
            }
            if (dateBooked.Date < DateTime.UtcNow.Date)
            {
                return Result.Failure<Booking>("The booking date must be today or in the future.");
            }
            if (parkingSpaceId <= 0)
            {
                return Result.Failure<Booking>("Parking Space ID must be a positive integer.");
            }
            if (parkingStructureId <= 0)
            {
                return Result.Failure<Booking>("Parking Structure ID must be a positive integer.");
            }
            return Result.Success<Booking>(new Booking(DefaultBookingId, bookeeId, dateBooked, parkingSpaceId, parkingStructureId));
        }
        public int BookeeId { get; private set; }
        public DateTime DateBooked { get; private set; }
        public int ParkingSpaceId { get; private set; }
        public int ParkingStructureId { get; private set; }



    }
}

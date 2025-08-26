using CSharpFunctionalExtensions;

namespace TG.CarParkEsher.Booking
{
    internal sealed class Booking : Entity<int>
    {
        private static int DefaultBookingId = 0;
        internal Booking(int id, int bookeeId, DateTime dateBooked, int parkingSpaceId, int parkingStructureId)
        {
            Id = id;
            BookeeId = bookeeId;
            DateBooked = dateBooked;
            ParkingSpaceId = parkingSpaceId;
            ParkingStructureId = parkingStructureId;
        }
        internal static Result<Booking> Create(int bookeeId, DateTime dateBooked, int parkingSpaceId, int parkingStructureId)
        {
            if (bookeeId <= 0)
            {
                return Result.Failure<Booking>("Bookee ID must be a positive integer.");
            }
            if (dateBooked < DateTime.UtcNow)
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
        internal int BookeeId { get; private set; }
        internal DateTime DateBooked { get; private set; }
        internal int ParkingSpaceId { get; private set; }
        internal int ParkingStructureId { get; private set; }



    }
}

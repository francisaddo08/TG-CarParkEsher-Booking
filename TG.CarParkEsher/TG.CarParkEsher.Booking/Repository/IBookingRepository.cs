
using CSharpFunctionalExtensions;
using TG.CarParkEsher.Booking.Domain.Primitives;

namespace TG.CarParkEsher.Booking
{
    public interface IBookingRepository
    {
        Task<Result<CarParkEsherBooking?>> CreateBookingAsync(CarParkEsherBooking bookingForCreate, CancellationToken cancellationToken);
        Task<Result<DatabaseVerificationsFlags>> CheckParkingSpaceByIdAsync(int parkingSpaceId, DateTime dateBooked, bool bluebadge, bool ev, bool hybrid, CancellationToken cancellationToken);
    }
}
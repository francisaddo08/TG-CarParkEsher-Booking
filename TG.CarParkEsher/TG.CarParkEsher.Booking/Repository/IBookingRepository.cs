
using CSharpFunctionalExtensions;

namespace TG.CarParkEsher.Booking
{
    public interface IBookingRepository
    {
        Task<Result<CarParkEsherBooking?>> CreateBookingAsync(CarParkEsherBooking bookingForCreate, CancellationToken cancellationToken);
        Task<Result<bool>> CheckParkingSpaceByIdAsync(int parkingSpaceId, DateTime dateBooked, CancellationToken cancellationToken);
    }
}
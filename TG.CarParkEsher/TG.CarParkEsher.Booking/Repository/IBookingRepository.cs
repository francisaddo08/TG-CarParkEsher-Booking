
using CSharpFunctionalExtensions;

namespace TG.CarParkEsher.Booking
{
    public interface IBookingRepository
    {
        Task<Result<Booking?>> CreateBookingAsync(Booking bookingForCreate, CancellationToken cancellationToken);
    }
}
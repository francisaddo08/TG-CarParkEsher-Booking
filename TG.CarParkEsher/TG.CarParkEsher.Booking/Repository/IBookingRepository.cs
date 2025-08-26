
using CSharpFunctionalExtensions;

namespace TG.CarParkEsher.Booking
{
    internal interface IBookingRepository
    {
        Task<Result<Booking?>> CreateBookingAsync(Booking bookingForCreate, CancellationToken cancellationToken);
    }
}
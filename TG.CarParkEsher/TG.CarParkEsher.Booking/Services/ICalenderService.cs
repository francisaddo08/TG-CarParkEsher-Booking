using CSharpFunctionalExtensions;

namespace TG.CarParkEsher.Booking
{
    public interface ICalenderService
    {
        Task<Result<bool>> UpdateWeekDaysAsync(CancellationToken cancellationToken);
    }
}
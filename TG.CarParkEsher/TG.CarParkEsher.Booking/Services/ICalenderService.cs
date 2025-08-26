using CSharpFunctionalExtensions;

namespace TG.CarParkEsher.Booking
{
    internal interface ICalenderService
    {
        Task<Result<bool>> UpdateWeekDaysAsync(CancellationToken cancellationToken);
    }
}
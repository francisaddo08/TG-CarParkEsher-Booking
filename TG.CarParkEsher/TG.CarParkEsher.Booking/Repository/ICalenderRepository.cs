using CSharpFunctionalExtensions;

namespace TG.CarParkEsher.Booking
{
    internal interface ICalenderRepository
    {
        Task<Result<List<EsherCarParkDayInfo>>> GetDaysOfWeek(CancellationToken cancellationToken);
    }
}
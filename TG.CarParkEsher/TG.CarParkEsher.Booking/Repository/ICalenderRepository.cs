using CSharpFunctionalExtensions;

namespace TG.CarParkEsher.Booking
{
    public interface ICalenderRepository
    {
        Task<Result<HashSet<CarParkEsherDayInfo>>> GetDaysOfWeek(CancellationToken cancellationToken);
        Task<Result<bool>> UpdateDaysOfWeek(HashSet<CarParkEsherDayInfo> esherCarParkDayInfos, CancellationToken cancellationToken);
        Task<Result<bool>> SeedDaysOfWeekTable(HashSet<CarParkEsherDayInfo> esherCarParkDayInfos, CancellationToken cancellationToken);
    }
}
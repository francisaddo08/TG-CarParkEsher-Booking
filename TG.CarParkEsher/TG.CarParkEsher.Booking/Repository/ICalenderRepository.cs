using CSharpFunctionalExtensions;

namespace TG.CarParkEsher.Booking
{
    internal interface ICalenderRepository
    {
        Task<Result<HashSet<EsherCarParkDayInfo>>> GetDaysOfWeek(CancellationToken cancellationToken);
        Task<Result<bool>> UpdateDaysOfWeek(HashSet<EsherCarParkDayInfo> esherCarParkDayInfos, CancellationToken cancellationToken);
    }
}
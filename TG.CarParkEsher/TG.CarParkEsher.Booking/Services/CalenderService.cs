using CSharpFunctionalExtensions;

namespace TG.CarParkEsher.Booking
{
    public  class CalenderService : ICalenderService
    {
        private readonly ICalenderRepository _calenderRepository;
        public CalenderService(ICalenderRepository calenderRepository)
        {
            _calenderRepository = calenderRepository ?? throw new ArgumentNullException(nameof(calenderRepository));
        }

        public async Task<Result<bool>> SeedDaysOfWeekTable(CancellationToken cancellationToken)
        { 
            return await  _calenderRepository.SeedDaysOfWeekTable(WeekDays(), cancellationToken);
        }

        public async Task<Result<bool>> UpdateWeekDaysAsync(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _calenderRepository.UpdateDaysOfWeek(WeekDays(), cancellationToken);
                if (result.IsFailure)
                {
                    return Result.Failure<bool>($"Failed to retrieve days of the week: {result.Error}");
                }
              
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                return Result.Failure<bool>($"An error occurred while updating week days. {ex.Message} {ex.InnerException?.Message}");
            }

        }
        public HashSet<CarParkEsherDayInfo> WeekDays()
        {

            DateTime Monday = DateTime.Now.StartOfWeek(DayOfWeek.Monday);
            DateTime Tuesday = Monday.AddDays(1);
            DateTime Wednesday = Monday.AddDays(2);
            DateTime Thursday = Monday.AddDays(3);
            DateTime Friday = Monday.AddDays(4);
            DateTime Saturday = Monday.AddDays(5);
            DateTime Sunday = Monday.AddDays(6);

            HashSet<CarParkEsherDayInfo> WeekDates = new HashSet<CarParkEsherDayInfo>()
                {
                  new CarParkEsherDayInfo(DayOfWeek.Monday.ToString(), 1,Monday),
                  new CarParkEsherDayInfo(DayOfWeek.Tuesday.ToString(),2, Tuesday),
                  new CarParkEsherDayInfo(DayOfWeek.Wednesday.ToString(),3, Wednesday),
                  new CarParkEsherDayInfo(DayOfWeek.Thursday.ToString(), 4,Thursday),
                  new CarParkEsherDayInfo(DayOfWeek.Friday.ToString(), 5,Friday),
                  new CarParkEsherDayInfo(DayOfWeek.Saturday.ToString(),6, Saturday),
                  new CarParkEsherDayInfo(DayOfWeek.Sunday.ToString(), 7,Sunday)
                };

            return WeekDates;
        }


    }
}

using CSharpFunctionalExtensions;

namespace TG.CarParkEsher.Booking.Domain.Entities
{
    internal class CarParkEsherDaysOfWeek : BaseEntity<int>
    {
       internal string DayName { get; private set; }
        internal int DayNumber { get; private set; }
        internal DateTime DateValue { get; private set; }
        private CarParkEsherDaysOfWeek(int id, string dayName, int dayNumber, DateTime dateValue):base(id)
        {
            DayName = dayName;
            DayNumber = dayNumber;
            DateValue = dateValue;
        }
        internal static Result<CarParkEsherDaysOfWeek> Create(int id ,string dayName, int dayNumber, DateTime dateValue)
        {
            if (string.IsNullOrWhiteSpace(dayName) || dayName.Length > DomainContants.MaxDayNameLength)
            {
                return Result.Failure<CarParkEsherDaysOfWeek>("Day name cannot be null or empty.");
            }
            if (dayNumber < 1 || dayNumber > 7)
            {
                return Result.Failure<CarParkEsherDaysOfWeek>("Day number must be between 1 and 7.");
            }
          
            return Result.Success<CarParkEsherDaysOfWeek>( new CarParkEsherDaysOfWeek(id, dayName, dayNumber, dateValue));
        }
    }
}

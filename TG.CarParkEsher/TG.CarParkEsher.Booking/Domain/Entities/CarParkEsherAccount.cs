using CSharpFunctionalExtensions;

namespace TG.CarParkEsher.Booking
{
    public sealed class CarParkEsherAccount :Entity<int>
    {
    private const int DefaultId = 0;
        public CarParkEsherAccount
        (
         int id, 
         int contactId,
         string vehicleType,
         string password, 
         string passwordHash, 
         string firstName,
         string lastName,
         string employeeId, 
         string salt, 
         bool isActive,
         bool isBlocked,
         IReadOnlyList<CarParkEsherBooking>? bookings = null
         ) : base(id)
        {
            ContactId = contactId;
            VehicleType = vehicleType;
            Password = password;
            PasswordHash = passwordHash;
            FirstName = firstName;
            LastName = lastName;
            EmployeeId = employeeId;
            Salt = salt;
            IsActive = isActive;
            IsLocked = isBlocked;
            Bookings = bookings ?? new List<CarParkEsherBooking>();
        }
        public static Result<CarParkEsherAccount> Create( int contactId, string vehicleType, string password, string passwordHash, string firstName, string lastName, string employeeId, string salt, bool isActive, bool isBlocked)
        {
            if (contactId <= 0)
            {
                return Result.Failure<CarParkEsherAccount>("Contact ID must be a positive integer.");
            }
            if (string.IsNullOrWhiteSpace(vehicleType))
            {
                return Result.Failure<CarParkEsherAccount>("Vehicle Type cannot be empty.");
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                return Result.Failure<CarParkEsherAccount>("Password cannot be empty.");
            }
            if (string.IsNullOrWhiteSpace(firstName))
            {
                return Result.Failure<CarParkEsherAccount>("First Name cannot be empty.");
            }
            if (string.IsNullOrWhiteSpace(lastName))
            {
                return Result.Failure<CarParkEsherAccount>("Last Name cannot be empty.");
            }
            if (string.IsNullOrWhiteSpace(employeeId))
            {
                return Result.Failure<CarParkEsherAccount>("Employee ID cannot be empty.");
            }
            if (string.IsNullOrWhiteSpace(salt))
            {
                return Result.Failure<CarParkEsherAccount>("Salt cannot be empty.");
            }
            return Result.Success(new CarParkEsherAccount(DefaultId, contactId, vehicleType, password, passwordHash, firstName, lastName, employeeId, salt, isActive, isBlocked));
        }
        
        public int ContactId { get;}
        public string VehicleType { get; }
        public string Password { get;  }
        public string PasswordHash { get; set; }
        public string FirstName { get; }
        public string LastName { get; }
        public string EmployeeId { get; }
        public string Salt { get; }
        public bool IsActive { get; } 
        public bool IsLocked { get; }
        public IReadOnlyList<CarParkEsherBooking> Bookings { get; } 


    }
}

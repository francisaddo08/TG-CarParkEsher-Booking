using CSharpFunctionalExtensions;

namespace TG.CarParkEsher.Booking
{
    public sealed class CarParkEsherAccount :Entity<int>
    {
        public CarParkEsherAccount(int id, int contactIdMy, string vehicleType, string password, string passwordHash, string firstName, string lastName, string employeeId, string salt) : base(id)
        {
            ContactIdMy = contactIdMy;
            VehicleType = vehicleType;
            Password = password;
            PasswordHash = passwordHash;
            FirstName = firstName;
            LastName = lastName;
            EmployeeId = employeeId;
            Salt = salt;
        }
        public int ContactIdMy { get;}
        public string VehicleType { get; }
        public string Password { get;  }
        public string PasswordHash { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string EmployeeId { get; }
        public string Salt { get; }
   
    }
}

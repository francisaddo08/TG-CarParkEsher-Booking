using CSharpFunctionalExtensions;

namespace TG.CarParkEsher.Booking
{
    public sealed class CarParkEsherEmployeeContact : ValueObject<CarParkEsherEmployeeContact>, IEquatable<CarParkEsherEmployeeContact?>
    {
        public CarParkEsherEmployeeContact(string employeeId, int contactId, string firstName, string lastName)
        {
            EmployeeId = employeeId;
            ContactId = contactId;
            FirstName = firstName;
            LastName = lastName;
        }

        public string EmployeeId { get; private set; }
        public int ContactId { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }

        public override bool Equals(object? obj) => Equals(obj as CarParkEsherEmployeeContact);
        public bool Equals(CarParkEsherEmployeeContact? other) => other is not null && base.Equals(other) && EmployeeId == other.EmployeeId && ContactId == other.ContactId && FirstName == other.FirstName && LastName == other.LastName;
        public override int GetHashCode() => base.GetHashCode();
        protected override bool EqualsCore(CarParkEsherEmployeeContact other) => (EmployeeId, ContactId, FirstName, LastName) == (other.EmployeeId, other.ContactId, other.FirstName, other.LastName);
        protected override int GetHashCodeCore() => HashCode.Combine(EmployeeId, ContactId, FirstName, LastName);

    }
}

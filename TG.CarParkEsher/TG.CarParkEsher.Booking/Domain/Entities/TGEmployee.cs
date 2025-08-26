using CSharpFunctionalExtensions;

namespace TG.CarParkEsher.Booking
{
    public sealed class TGEmployee :Entity<string>
    {
       public TGEmployee(string employeeid, int contact_id):base(employeeid) { ContactId = contact_id; }
       public int ContactId { get; set; }
    }
}

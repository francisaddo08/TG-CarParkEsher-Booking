namespace TG.CarParkEsher.Booking
{
    internal sealed class TGEmployee :BaseEntity<string>
    {
       internal TGEmployee(string employeeid, int contact_id):base(employeeid) { ContactId = contact_id; }
       internal int ContactId { get; set; }
    }
}

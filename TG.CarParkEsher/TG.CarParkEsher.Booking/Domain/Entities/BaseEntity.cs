namespace TG.CarParkEsher.Booking
{
    internal abstract class BaseEntity<T>
    {
     internal T Id { get; set; }
        protected internal BaseEntity()
        {
            Id = default!;
        }
        protected internal BaseEntity(T id)
        {
            Id = id;
        }
    }
}
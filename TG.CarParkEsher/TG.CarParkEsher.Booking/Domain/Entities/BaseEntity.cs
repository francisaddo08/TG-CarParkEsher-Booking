namespace TG.CarParkEsher.Booking
{
    public class BaseEntity<T>
    {
     public T Id { get; set; }
        public BaseEntity()
        {
            Id = default!;
        }
        public BaseEntity(T id)
        {
            Id = id;
        }
    }
}
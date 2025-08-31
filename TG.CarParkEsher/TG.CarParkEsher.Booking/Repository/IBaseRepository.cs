using Microsoft.Data.Sqlite;

namespace TG.CarParkEsher.Booking
{
    public interface IBaseRepository
    {
       protected SqliteConnection GetConnection();
    }
}
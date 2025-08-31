namespace TG.CarParkEsher.Booking
{
    public interface ILoggingService
    {
        void LogInfo(string message, params object[] parameters);
        
    }
}

namespace TG.CarParkEsher.Booking
{
    public sealed class ErrorDto
    {
        public bool IsError => true;
        public string ErrorID { get; set; } = string.Empty;
        public string ErrorDetail { get; set; } = string.Empty;
        public string ErrorMessage => $"[{ErrorID}] {ErrorDetail}";

    }
}

namespace TG.CarParkEsher.Booking.Domain.Primitives
{
    public  sealed class DatabaseVerificationsFlags
    {
      public bool IsValidParkingSpaceId { get; set; } = false;
        public bool IsParkingSpaceAvailable { get; set; } = false;
        public bool IsDateAvailable { get; set; } = false;
        public bool IsBlueBadgeValid { get; set; } = false;
        public bool IsEvValid { get; set; } = false;
        public bool IsHybridValid { get; set; } = false;
        public List<int> AvaliableBlueBadgeBays { get; set; } = new List<int>();
        public List<int> AvaliableEvBays { get; set; } = new List<int>();
        public List<int> AvaliableHybridBays { get; set; } = new List<int>();
        public List<int> AvaliableStandardBays { get; set; } = new List<int>();


    }
}

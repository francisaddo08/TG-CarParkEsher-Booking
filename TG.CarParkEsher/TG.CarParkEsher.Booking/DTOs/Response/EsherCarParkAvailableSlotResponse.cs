namespace TG.CarParkEsher.Booking
{
    internal sealed class EsherCarParkAvailableSlotResponse
    {
        private EsherCarParkAvailableSlotResponse(string? slotId, string? carParkId, DateTime? startTime, DateTime? endTime)
        {
            SlotId = slotId;
            CarParkId = carParkId;
            StartTime = startTime;
            EndTime = endTime;
        }
        internal static EsherCarParkAvailableSlotResponse Create(string? slotId, string? carParkId, DateTime? startTime, DateTime? endTime)
        {
            return new EsherCarParkAvailableSlotResponse(slotId, carParkId, startTime, endTime);
        }
        public string? SlotId { get;  }
        public string? CarParkId { get;  }
        public DateTime? StartTime { get;  }
        public DateTime? EndTime { get;  }
        
       
    }
}

namespace TG.CarParkEsher.Booking
{
    public interface IBookingService
    {
        public Task<ContextResult<EsherCarParkBookingResponseDto>> CreateBookSlotAsync(EsherCarParkBookingRequestDto bookingRequest, CancellationToken cancellationToken);
        //Task<ContextResult<EsherCarParkAvaliableBayResponseDto>> GetAllAvaliableBaysAsync(EsherCarParkAvaliableBayRequestDto request, CancellationToken cancellationToken);
    }
}
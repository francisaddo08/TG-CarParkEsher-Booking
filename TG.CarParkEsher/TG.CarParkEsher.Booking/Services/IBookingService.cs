using CSharpFunctionalExtensions;

namespace TG.CarParkEsher.Booking
{
    public interface IBookingService
    {
        public Task<ContextResult<EsherCarParkBookingResponseDto>> BookParkingSpaceAsync(EsherCarParkBookingRequestDto bookingRequest, CancellationToken cancellationToken);
        Task<ContextResult<List<EsherCarParkAvaliableBayResponseDto>>> GetAllAvaliableParkingSpacesAsync(EsherCarParkAvaliableBayRequestDto request, CancellationToken cancellationToken);
     
    }
}
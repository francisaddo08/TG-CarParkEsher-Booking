
namespace TG.CarParkEsher.Booking
{
    public class CarParkEsherRegistrationResponseDto : RequestValidationResultDto
    {
        public CarParkEsherRegistrationResponseDto(bool valid, IList<ErrorDto>? errors) : base(valid, errors)
        {
        }
    }        
}

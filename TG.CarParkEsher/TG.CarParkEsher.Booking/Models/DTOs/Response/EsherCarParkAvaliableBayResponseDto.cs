
namespace TG.CarParkEsher.Booking
{
    public sealed class EsherCarParkAvaliableBayResponseDto :RequestValidationResultDto
    {
        public EsherCarParkAvaliableBayResponseDto(bool valid, IList<ErrorDto>? errors) : base(valid, errors)
        {
        }

        public List<EsherCarParkAvaliableBayDetailDto> AvaliableBays { get; set; } = new List<EsherCarParkAvaliableBayDetailDto>();

    }

}

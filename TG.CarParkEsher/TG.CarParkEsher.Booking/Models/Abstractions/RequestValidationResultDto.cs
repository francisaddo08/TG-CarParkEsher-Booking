using System.Text.Json.Serialization;

namespace TG.CarParkEsher.Booking
{
    public abstract class RequestValidationResultDto
    {
        public RequestValidationResultDto(bool valid, IList<ErrorDto>? errors)
        {
            _valid = valid;
            _errors = errors;
        }

        private bool _valid;

        [JsonIgnore]
        public bool Valid => _valid;

        private IList<ErrorDto>? _errors = null;
        public IReadOnlyList<ErrorDto>? Errors
        {
            get => _errors?.ToList();
            private set => _errors = value?.ToList();
        }
        public void SetValidation(bool valid, IList<ErrorDto>? errors)
        {
            _valid = valid;
            if (errors != null)
            {
                _errors = errors.ToList();
            }
        }
    }
}

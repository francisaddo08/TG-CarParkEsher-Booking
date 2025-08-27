

using CSharpFunctionalExtensions;
using System.Text.RegularExpressions;

namespace TG.CarParkEsher.Booking
{
    public class EsherCarParkrRegistrationResponseDto : RequestValidationResultDto
    {
        private EsherCarParkrRegistrationResponseDto(string firstName, string lastName, string vehicleType, bool valid, IList<ErrorDto>? errors) : base(valid, errors)
        {
            FirstName = firstName;
            LastName = lastName;
            VehicleType = vehicleType;
        }

        public static EsherCarParkrRegistrationResponseDto Create(EsherCarParkRegistrationRequestDto request)
        {
            var firstName = (request.FirstName ?? string.Empty).Trim();
            var lastName = (request.LastName ?? string.Empty).Trim();
            var password = (request.Password ?? string.Empty).Trim();
            var confirmPassword = (request.ConfirmPassword ?? string.Empty).Trim();
            var vehicleType = (request.VehicleType ?? string.Empty).Trim();
            var valid = true;
            var errors = new List<ErrorDto>();

            if (password.Length < PasswordValidationConstants.MinimumLength)
            {
                valid = false;
                errors.Add(new ErrorDto { ErrorID = "InvalidPassword", ErrorDetail = "Password must be at least 8 characters long." });
            }
            if( password == confirmPassword )
            {
                valid = false;
                errors.Add(new ErrorDto { ErrorID = "InvalidPassword", ErrorDetail = "Password and Confirm Password must match." });
            }
            if (!Regex.IsMatch(password, @"[A-Z]"))
            {
                valid = false;
                errors.Add(new ErrorDto { ErrorID = "InvalidPassword", ErrorDetail = "Password must contain at least one uppercase letter." });
            }
            if (!Regex.IsMatch(password, @"[^\w\s]"))
            {
                valid = false;
                errors.Add(new ErrorDto { ErrorID = "InvalidPassword", ErrorDetail = "Password must contain at least one special character." });
            }

            if (firstName.Length == 0)
            {
                valid = false;
                errors.Add(new ErrorDto { ErrorID = "InvalidFirstName", ErrorDetail = "First Name must be at least 2 characters long." });
            }
            if (lastName.Length == 0)
            {
                valid = false;
                errors.Add(new ErrorDto { ErrorID = "InvalidLastName", ErrorDetail = "Last Name must be at least 2 characters long." });
            }
            if (ContainsSpecialOrNumber(firstName))
            {
                valid = false;
                errors.Add(new ErrorDto { ErrorID = "InvalidFirstName", ErrorDetail = "First Name must not contain special characters or numbers." });
            }
            if (ContainsSpecialOrNumber(lastName))
            {
                valid = false;
                errors.Add(new ErrorDto { ErrorID = "InvalidLastName", ErrorDetail = "Last Name must not contain special characters or numbers." });
            }
            if (vehicleType.Length == 0)
            {
                valid = false;
                errors.Add(new ErrorDto { ErrorID = "InvalidVehicleType", ErrorDetail = "Vehicle Type must be at least 2 characters long." });
            }
            if (ContainsSpecialOrNumber(vehicleType))
            {
                valid = false;
                errors.Add(new ErrorDto { ErrorID = "InvalidVehicleType", ErrorDetail = "Vehicle Type must not contain special characters or numbers." });
            }
            var permitedVehicleTypes = Enumeration.GetAll<VehicleType>().FirstOrDefault(vt => vt.Name.Equals(vehicleType, StringComparison.InvariantCultureIgnoreCase));
            if (permitedVehicleTypes == null)
            {
                valid = false;
                errors.Add(new ErrorDto { ErrorID = "InvalidVehicleType", ErrorDetail = $"Vehicle Type must be one of the following: {string.Join(", ", Enumeration.GetAll<VehicleType>().Select(vt => vt.Name))}." });
            }
            errors = errors.Any() ? errors : null;

            return new EsherCarParkrRegistrationResponseDto(firstName, lastName, vehicleType, valid, errors);
        }
        public string FirstName { get; }
        public string LastName { get; }
        public string VehicleType { get; }


        private static bool ContainsSpecialOrNumber(string input)
        {
            if (input.Length == 0 || input.Length == 0)
            {
                return false;
            }

            bool containsSpecialOrNumber = Regex.IsMatch(input, @"[\d\W]");
            return containsSpecialOrNumber;
        }
    }
}

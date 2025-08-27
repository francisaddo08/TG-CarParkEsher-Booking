using CSharpFunctionalExtensions;

namespace TG.CarParkEsher.Booking.Domain.Primitives
{
    public sealed class AccountCredentials : ValueObject<AccountCredentials>
    {
        private AccountCredentials(string userName, string password, bool isActive, bool isLocked)
        {
            UserName = userName;
            Password = password;
            IsActive = isActive;
            IsLocked = isLocked;
        }
        public static Result<AccountCredentials> Create(string userName, string password, bool isActive, bool isLocked)
        {
            var _username = (userName ?? string.Empty).Trim();
            var _password = (password ?? string.Empty).Trim();

            if (_username.Length == 0 || _username.Length < 5)
            {
                return Result.Failure<AccountCredentials>("Invalid username");
            }
            if (_username.Length < 5)
            {
                return Result.Failure<AccountCredentials>("Invalid username. Username cannot be less than 5 characters");
            }
            if (_password.Length == 0)
            {
                return Result.Failure<AccountCredentials>("Invalid password");
            }
            if (_password.Length < 8)
            {
                return Result.Failure<AccountCredentials>("Invalid password. Password cannot be less than 8 characters");
            }
            return Result.Success<AccountCredentials>(new AccountCredentials(_username, _password, isActive, isLocked));
        }

        public string UserName { get; }
        public string Password { get; }
        public bool IsActive { get; }
        public bool IsLocked { get; }
        protected override bool EqualsCore(AccountCredentials other)
        {
            return UserName.Equals(other.UserName) && Password.Equals(other.Password) && IsActive.Equals(other.IsActive) && IsLocked.Equals(other.IsLocked);
        }

        protected override int GetHashCodeCore()
        {
            return (UserName, Password, IsActive, IsLocked).GetHashCode();
        }

    }
}

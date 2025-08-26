namespace TG.CarParkEsher.Booking.HostingExtensions
{
    public sealed class UserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly IUserRepository _userRepository;
        public UserService(ILogger<UserService> logger, IUserRepository userRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }  
       


    }
}
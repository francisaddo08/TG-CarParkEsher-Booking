namespace TG.CarParkEsher.Booking.HostingExtensions
{
    public static  class HostingExtensions
    {
       public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            //services.AddScoped<IBookingService, BookingService>();
            //services.AddScoped<IBookingRepository, BookingRepository>();
            //services.AddScoped<IUserRepository, UserRepository>();
            //services.AddScoped<IUserService, UserService>();
            //services.AddScoped<IEmailService, EmailService>();
            //services.AddScoped<IEmailSender, EmailSender>();
            return services;
        }
        public static WebApplicationBuilder AddApplicationConfigurations(this WebApplicationBuilder builder)
        {
            builder.Services.Configure<ConnectionOption>(builder.Configuration.GetSection("ConnectionOption"));
            return builder;
        }
    }
}

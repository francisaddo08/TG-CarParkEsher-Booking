using TG.CarParkEsher.Booking.HostingExtensions;

namespace TG.CarParkEsher.Booking
{
    internal static  class HostingExtension
    {
       internal static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddSingleton<EmployeeRepository>();
            services.AddHostedService<CalenderWorkerService>();
            //services.AddScoped<IBookingService, BookingService>();
            //services.AddScoped<IBookingRepository, BookingRepository>();
            //services.AddScoped<IUserRepository, UserRepository>();
            //services.AddScoped<IUserService, UserService>();
            //services.AddScoped<IEmailService, EmailService>();
            //services.AddScoped<IEmailSender, EmailSender>();
            return services;
        }
        internal static WebApplicationBuilder AddConfigurationsOptions(this WebApplicationBuilder builder)
        {
            builder.Services.Configure<ConnectionOption>(builder.Configuration.GetSection("ConnectionOption"));
            return builder;
        }
        public static WebApplication AddEndPoints(this WebApplication app)
        {
            app.MapGet("/employees", async (EmployeeRepository employeeRepository, CancellationToken cancellationToken) =>
            {
                var result = await employeeRepository.GetEmployeesAsync(cancellationToken);
                if (result.IsSuccess)
                {
                    var data = result.Value.Select(e => new
                    {
                        e.Id,
                        e.ContactId
                    }).ToList();
                    return Results.Ok(data);
                }
                return Results.BadRequest(result.Error);
            });
           
            return app;
        }

    }
}

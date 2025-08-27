using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace TG.CarParkEsher.Booking
{
    public static class HostingExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<ICalenderService, CalenderService>();
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<IAccountService, AccountService>();
            return services;
        }
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<EmployeeRepository>();
            services.AddScoped<ICalenderRepository, CalenderRepository>();
            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();

            return services;
        }
        public static WebApplicationBuilder AddConfigurationsOptions(this WebApplicationBuilder builder)
        {
            builder.Services.Configure<ConnectionOption>(builder.Configuration.GetSection("ConnectionOption"));
            return builder;
        }
        public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddControllers()
            .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;

                });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = "TG.CarParkEsher.Booking", Version = "v1" });

            });

            builder.Services.AddAuthentication("BasicAuthentication").AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);
            builder.Services.AddHttpContextAccessor();
            builder.AddConfigurationsOptions();
            builder.Services.AddScoped<IPasswordHasher<CarParkEsherAccount>, PasswordHasher<CarParkEsherAccount>>();
            builder.Services.AddRepositories();
            builder.Services.AddApplicationServices();
            builder.Services.AddHostedService<CalenderWorkerService>();
            return builder.Build();
        }
        public static WebApplication ConfigurePipeline(this WebApplication app)
        {

            // Configure the HTTP request pipeline.

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.MapControllers();

            return app;
        }

    }
}

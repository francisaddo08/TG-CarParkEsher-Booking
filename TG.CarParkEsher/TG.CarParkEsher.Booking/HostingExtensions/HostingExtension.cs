using TG.CarParkEsher.Booking.HostingExtensions;

namespace TG.CarParkEsher.Booking
{
    internal static  class HostingExtension
    {
        internal static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<ICalenderService, CalenderService>();
            return services;
        }
        internal static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<EmployeeRepository>();
            services.AddScoped<ICalenderRepository, CalenderRepository>();

            return services;
        }
        internal static WebApplicationBuilder AddConfigurationsOptions(this WebApplicationBuilder builder)
        {
            builder.Services.Configure<ConnectionOption>(builder.Configuration.GetSection("ConnectionOption"));
            return builder;
        }
        internal static WebApplication ConfigureServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.AddConfigurationsOptions();
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

            var summaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };


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


            app.MapGet("/weatherforecast", () =>
            {
                var forecast = Enumerable.Range(1, 5).Select(index =>
                    new WeatherForecast
                    (
                        DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                        Random.Shared.Next(-20, 55),
                        summaries[Random.Shared.Next(summaries.Length)]
                    ))
                    .ToArray();
                return forecast;
            })
            .WithName("GetWeatherForecast")
            .WithOpenApi();

            return app;
        }

    }
}

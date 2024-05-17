using NOS.Engineering.Challenge.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddEventSourceLogger();
builder.Logging.AddFilter("Microsoft", LogLevel.Warning)
               .AddFilter("System", LogLevel.Warning)
               .AddFilter("Default", LogLevel.Information);

// Register services
builder.ConfigureWebHost()
       .RegisterServices();

var app = builder.Build();

app.MapControllers();
app.UseSwagger()
    .UseSwaggerUI();

app.Run();

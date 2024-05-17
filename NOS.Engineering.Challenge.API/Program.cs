using Microsoft.EntityFrameworkCore;
using NOS.Engineering.Challenge.API.Extensions;
using NOS.Engineering.Challenge.Database;
using NOS.Engineering.Challenge.Managers;
using NOS.Engineering.Challenge.Models;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddEventSourceLogger();
builder.Logging.AddFilter("Microsoft", LogLevel.Warning)
               .AddFilter("System", LogLevel.Warning)
               .AddFilter("Default", LogLevel.Information);

// Add EF Core and SQLite
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=content.db"));

// Add DI for custom services
builder.Services.AddScoped<IMapper<Content, ContentDto>, ContentMapper>();
builder.Services.AddScoped<IDatabase<Content, ContentDto>, DatabaseService>();
builder.Services.AddScoped<IContentsManager, ContentsManager>();

// Register services
builder.ConfigureWebHost()
       .RegisterServices();

var app = builder.Build();

app.MapControllers();
app.UseSwagger()
    .UseSwaggerUI();

app.Run();

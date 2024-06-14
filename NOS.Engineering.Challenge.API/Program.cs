using Microsoft.OpenApi.Models;
using NOS.Engineering.Challenge.API.Extensions;
using NOS.Engineering.Challenge.Database;
using NOS.Engineering.Challenge.Managers;
using NOS.Engineering.Challenge.Models;

var builder = WebApplication.CreateBuilder(args)
    .ConfigureWebHost()
    .RegisterServices();

var app = builder.Build();

app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI();

app.Run();
// Configura��o do logger e dos servi�os
IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureLogging((hostingContext, logging) =>
        {
            logging.ClearProviders();
            logging.AddConsole();
        })
        .ConfigureServices((hostContext, services) =>
        {
            // Configura��o do MongoDB
            services.Configure<MongoDbSettings>(hostContext.Configuration.GetSection(nameof(MongoDbSettings)));
            services.AddSingleton<MongoDbContext>();
            services.AddScoped<IDatabase<Content?, ContentDto>, MongoRepository>();

            // Configura��o dos servi�os MVC
            services.AddControllers();

            // Configura��o do Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "NOS.Engineering.Challenge", Version = "v1" });
            });

            // Configura��o do cache Redis
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = hostContext.Configuration.GetConnectionString("Redis");
                options.InstanceName = "NOSCache";
            });

            // Registro do gerenciador de conte�do com cache
            services.AddScoped<IContentsManager, CachedContentsManager>();
        });

var host = CreateHostBuilder(args).Build();
host.Run();

using NOS.Engineering.Challenge.API.Extensions;

var builder = WebApplication.CreateBuilder(args)
        .ConfigureWebHost()
        .RegisterServices();
var app = builder.Build();

app.MapControllers();
app.UseSwagger()
    .UseSwaggerUI();

app.Run();

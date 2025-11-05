var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();                      
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddScoped<IAirlineService, AirlineService>();
builder.Services.AddScoped<IBoardingService, BoardingService>();
builder.Services.AddScoped<IFlightService, FlightService>();

//Añade los controllers
builder.Services.AddControllers();


// DI - Añadir servicios de la capa de negocio
builder.Services.AddHttpClient(); // Registrar HttpClient
builder.Services.AddSingleton<ISlotService, SlotService>();
builder.Services.AddSingleton<IGateService, GateService>();
builder.Services.AddSingleton<ITicketService, TicketService>();
// Construir la app
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthorization();

app.MapControllers();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
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

app.Run();
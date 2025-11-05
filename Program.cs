using PortHub.Api.Interface;
using PortHub.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Añade los controllers
builder.Services.AddControllers();


// DI - Añadir servicios de la capa de negocio
builder.Services.AddHttpClient(); // Registrar HttpClient
builder.Services.AddScoped<IAirlineService, AirlineService>();
builder.Services.AddScoped<IBoardingService, BoardingService>();
builder.Services.AddScoped<IFlightService, FlightService>();
builder.Services.AddScoped<ISlotService, SlotService>();
builder.Services.AddScoped<IGateService, GateService>();
builder.Services.AddScoped<ITicketService, TicketService>();
// Construir la app
var app = builder.Build();

// Configurar el pipeline de la aplicación
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//Mapea los controllers.
app.MapControllers();

app.Run();
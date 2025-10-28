using PortHub.Api.Interface;
using PortHub.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DI - Añadir servicios de la capa de negocio
builder.Services.AddSingleton<ISlotService, SlotService>();
builder.Services.AddSingleton<IGateService, GateService>();
builder.Services.AddSingleton<ITicketService, TicketService>();

// Construir la app
var app = builder.Build();

// Configurar el pipeline de la aplicación
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();
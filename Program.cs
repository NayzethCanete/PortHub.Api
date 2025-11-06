using Microsoft.EntityFrameworkCore;
using PortHub.Api.Data;
using PortHub.Api.Interfaces;
using PortHub.Api.Services;
using PortHub.Api.Security;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

// Cargar variables de entorno desde .env
Env.Load();

// Add services to the container.
builder.Services.AddControllers();

// Configurar HttpClient con timeout adecuado
builder.Services.AddHttpClient<ITicketService, TicketService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Configuración de la Base de Datos usando la variable de entorno
var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") 
    ?? "Server=LENOVO\\SQLEXPRESS;Database=PortHubApi;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// Registrar Servicios con Scoped (importante para DbContext)
builder.Services.AddScoped<ISlotService, SlotService>();
builder.Services.AddScoped<IGateService, GateService>();
builder.Services.AddScoped<IAirlineService, AirlineService>();
builder.Services.AddScoped<IBoardingService, BoardingService>();
builder.Services.AddScoped<ITicketService, TicketService>();

// Configurar CORS (importante para desarrollo)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "PortHub API", Version = "v1" });
    
    // Configurar API Key en Swagger
    c.AddSecurityDefinition("ApiKey", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "API Key necesaria para endpoints de aerolíneas. Header: X-API-Key",
        Name = "X-API-Key",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "ApiKeyScheme"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PortHub API v1");
        c.RoutePrefix = string.Empty; // Swagger en la raíz
    });
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

// Habilitar CORS
app.UseCors("AllowAll");

// Añadir el Middleware de API Key ANTES de UseAuthorization
app.UseMiddleware<ApiKeyAuthMiddleware>();

app.UseAuthorization();

app.MapControllers();

// Mensaje de inicio
app.Logger.LogInformation("🚀 PortHub API iniciada correctamente");
app.Logger.LogInformation("📊 Swagger UI disponible en: http://localhost:5000");
app.Logger.LogInformation("🔗 Connection String: {ConnectionString}", connectionString.Replace("Password=", "Password=***"));

app.Run();
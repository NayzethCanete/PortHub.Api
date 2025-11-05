using Microsoft.EntityFrameworkCore;
using PortHub.Api.Data;
using PortHub.Api.Interfaces;
using PortHub.Api.Services;
using DotNetEnv;

// Cargar variables de entorno
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// ===== CONFIGURACIÓN DE BASE DE DATOS SQL SERVER =====
var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException(
        "No se encontró la cadena de conexión. " +
        "Configura DB_CONNECTION_STRING en .env o en appsettings.json"
    );
}

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(connectionString); 
    
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

// ===== INYECCIÓN DE DEPENDENCIAS =====
builder.Services.AddScoped<IAirlineService, AirlineService>();
//builder.Services.AddScoped<ISlotService, SlotService>();
//builder.Services.AddScoped<IGateService, GateService>();
builder.Services.AddScoped<IBoardingService, BoardingService>();

builder.Services.AddHttpClient("AirlineApiClient", client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("User-Agent", "PortHub-Airport-System");
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.WriteIndented = true;
    });

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "PortHub Airport API",
        Version = "v1",
        Description = "API REST para la gestión de operaciones aeroportuarias"
    });
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        try
        {
            dbContext.Database.Migrate();
            Console.WriteLine(" Base de datos migrada exitosamente");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al migrar la base de datos: {ex.Message}");
        }
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PortHub API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseRouting();
app.UseAuthorization();

app.MapControllers();

// Health check
app.MapGet("/health", () => Results.Ok(new
{
    status = "healthy",
    timestamp = DateTime.UtcNow,
    environment = app.Environment.EnvironmentName,
    database = "SQL Server"
}));

Console.WriteLine("PortHub API iniciada correctamente");
Console.WriteLine($"Swagger UI: http://localhost:5000");

app.Run();

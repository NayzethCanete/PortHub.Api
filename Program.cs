using Microsoft.EntityFrameworkCore;
using PortHub.Api.Data;
using PortHub.Api.Interfaces; 
using PortHub.Api.Services;
using PortHub.Api.Models;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;

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

//AuthOptions:
builder.Services.Configure<AuthOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });


//DBContext añadido.
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(connectionString); 
    //options.UseNpgsql(connectionString); //Para Postgree
    
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

// ===== INYECCIÓN DE DEPENDENCIAS (Combinadas) =====
builder.Services.AddScoped<IAirlineService, AirlineService>();
builder.Services.AddScoped<ISlotService, SlotService>();
builder.Services.AddScoped<IGateService, GateService>();
builder.Services.AddScoped<IBoardingService, BoardingService>();
//builder.Services.AddScoped<IFlightService, FlightService>();
// La logica de vuelo es para Aerolineas.
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();

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
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PortHub.Api", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Solo pega tu token JWT. El prefijo 'Bearer' se añadirá automáticamente."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
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

app.Use(async (context, next) => //MIDDLEWARE que añade "Bearer" Antes del Header [Authorization].
{
    var auth = context.Request.Headers["Authorization"].FirstOrDefault();
    if (!string.IsNullOrEmpty(auth) && !auth.StartsWith("Bearer "))
    {
        context.Request.Headers["Authorization"] = "Bearer " + auth;
    }
    await next();
});

app.UseAuthentication();
app.UseAuthorization();

//Mapea los controllers.
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
Console.WriteLine($"Swagger UI: http://localhost:5000"); // Asumiendo puerto 5000

app.Run();
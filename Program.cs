using Microsoft.EntityFrameworkCore;
using PortHub.Api.Data;
using PortHub.Api.Interfaces;
using PortHub.Api.Services;
using PortHub.Api.Security; 
using PortHub.Api.Models; 
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;

// Cargar variables de entorno desde .env
Env.Load();

var builder = WebApplication.CreateBuilder(args);



// DEFINICIÓN DEL CONNECTION STRING (IGUALMENTE SE UTILIZA .ENV)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("La variable 'ConnectionStrings__DefaultConnection' no está definida en .env");

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("La variable 'Jwt__Key' no está definida en .env");

if (string.IsNullOrEmpty(builder.Configuration["AirlineApi:BaseUrl"]) || string.IsNullOrEmpty(builder.Configuration["AirlineApi:ApiKey"]))
{
    throw new InvalidOperationException("Las variables 'AirlineApi__BaseUrl' o 'AirlineApi__ApiKey' no están definidas en .env");
}

builder.Services.AddControllers();

// ==========================================================
// CONFIGURACIÓN DE SEGURIDAD JWT (Authentication)
// ==========================================================

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


// ==========================================================
// CONFIGURACIÓN DE BASE DE DATOS
// ==========================================================
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(connectionString); 
    
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});


// ==========================================================
// REGISTRO DE SERVICIOS Y HTTP CLIENTS
// ==========================================================

builder.Services.AddHttpClient(); 
builder.Services.AddScoped<IAirlineService, AirlineService>();
builder.Services.AddScoped<ISlotService, SlotService>();
builder.Services.AddScoped<IGateService, GateService>();
builder.Services.AddScoped<IBoardingService, BoardingService>();
builder.Services.AddScoped<IAirlineIntegrationService, AirlineIntegrationService>();
builder.Services.AddScoped<IUserService, UserService>(); 
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddHostedService<SlotCleanupService>();

builder.Services.Configure<SlotReservationOptions>(
    builder.Configuration.GetSection("SlotReservation")
);


// ==========================================================
// 5. CONFIGURACIÓN DE CORS
// ==========================================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// ==========================================================
// 6. CONFIGURACIÓN DE SWAGGER (Soporte JWT y API Key)
// ==========================================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { 
        Title = "PortHub API", 
        Version = "v1",
        Description = "API de gestión aeroportuaria con integración a aerolíneas"
    });
    
    
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Token JWT para endpoints de administración. Solo pega el token sin 'Bearer'."
    });

   
    c.AddSecurityDefinition("ApiKey", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "API Key necesaria para endpoints de aerolíneas. Header: X-API-Key",
        Name = "X-API-Key",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "ApiKeyScheme"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        },
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "ApiKey" }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// ==========================================================
// 7. CONFIGURACIÓN DEL PIPELINE HTTP (ORDEN CRÍTICO)
// ==========================================================

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
app.UseCors("AllowAll");

//Middleware de API Key, DEBE ir ANTES de UseAuthentication/UseAuthorization 

app.UseMiddleware<ApiKeyAuthMiddleware>();


app.Use(async (context, next) => 
{
    var auth = context.Request.Headers["Authorization"].FirstOrDefault();
    if (!string.IsNullOrEmpty(auth) && !auth.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
    {
        context.Request.Headers["Authorization"] = "Bearer " + auth;
    }
    await next();
});


app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

// ===== MENSAJES DE INICIO =====
app.Logger.LogInformation("PortHub API iniciada correctamente");
app.Logger.LogInformation("Base de datos: {ConnectionString}", 
    connectionString.Replace(connectionString.Split(';')[0], "Server=***"));

app.Run();
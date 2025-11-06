using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PortHub.Api.Data;
using System.Linq;
using System.Threading.Tasks;

namespace PortHub.Api.Security
{
    public class ApiKeyAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private const string API_KEY_HEADER = "X-API-Key";
        private readonly string _masterApiKey; // <-- Variable para la clave maestra

        public ApiKeyAuthMiddleware(RequestDelegate next)
        {
            _next = next;
            // Cargamos la clave maestra (del .env) UNA SOLA VEZ al inicio
            _masterApiKey = Environment.GetEnvironmentVariable("API_KEY_AEROPUERTO") ?? "";
        }

        public async Task InvokeAsync(HttpContext context, AppDbContext dbContext)
        {
            var path = context.Request.Path.Value?.ToLower() ?? "";

            // Rutas públicas (Swagger, etc.)
            var publicPaths = new[] { "/swagger", "/index.html", "/_framework", "/favicon.ico" };
            if (publicPaths.Any(p => path.StartsWith(p.ToLower())))
            {
                await _next(context);
                return;
            }

            // Rutas protegidas
            var protectedPaths = new[]
            {
                "/api/slot/reserve",
                "/api/slot/confirm",
                "/api/slot/cancel",
                "/api/airlinevalidation/validate" // <-- AÑADE OTRAS RUTAS PROTEGIDAS AQUÍ
            };

            // Si NO es una ruta protegida, permitir acceso
            if (!protectedPaths.Any(p => path.StartsWith(p.ToLower())))
            {
                await _next(context);
                return;
            }

            // --- INICIO DE LA LÓGICA CORREGIDA ---

            // 1. VALIDAR API KEY (Revisar si el header existe)
            if (!context.Request.Headers.TryGetValue(API_KEY_HEADER, out var extractedApiKey))
            {
                context.Response.StatusCode = 401; // Unauthorized
                await context.Response.WriteAsJsonAsync(new { code = "UNAUTHORIZED", message = "API Key no proporcionada." });
                return;
            }

            var apiKeyValue = extractedApiKey.ToString();

            // 2. Revisar si es la CLAVE MAESTRA (la del .env)
            if (!string.IsNullOrEmpty(_masterApiKey) && _masterApiKey.Equals(apiKeyValue))
            {
                // Es la clave maestra. Dejar pasar.
                context.Items["Airline"] = "AEROPUERTO_ADMIN"; // Identificador especial
                await _next(context);
                return;
            }

            // 3. Si NO es la clave maestra, buscar en la BD si es una Aerolínea
            var airline = await dbContext.Airlines
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.ApiKey == apiKeyValue);

            if (airline == null)
            {
                // La clave no es la maestra Y TAMPOCO es de una aerolínea
                context.Response.StatusCode = 403; // Forbidden
                await context.Response.WriteAsJsonAsync(new { code = "FORBIDDEN", message = "API Key inválida." });
                return;
            }

            // 4. Es una clave de aerolínea válida. Dejar pasar.
            context.Items["Airline"] = airline;
            context.Items["AirlineId"] = airline.Id;
            context.Items["AirlineCode"] = airline.Code;

            await _next(context);
            // --- FIN DE LA LÓGICA CORREGIDA ---
        }
    }
}
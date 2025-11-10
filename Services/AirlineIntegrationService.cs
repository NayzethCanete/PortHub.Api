using System.Net.Http.Json;
using PortHub.Api.Dtos;
using PortHub.Api.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


/*


Es el servicio que maneja la integracion con API externa de aerolina
Va a encargarse de validar los tickets con la aerolinea correspondiente

Recibe un TicketValidationRequest y devuelve un TicketValidationResponse


*/




namespace PortHub.Api.Services
{
    public class AirlineIntegrationService : IAirlineIntegrationService
    {
        private readonly HttpClient _httpClient; //Client HTTP usado para  request externos 
        private readonly ILogger<AirlineIntegrationService> _logger;

        public AirlineIntegrationService(HttpClient httpClient, IConfiguration config, ILogger<AirlineIntegrationService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;

            // Configuración base desde appsettings.json
            var baseUrl = config["AirlineApi:BaseUrl"];
            var apiKey = config["AirlineApi:ApiKey"];
            

            //Validaciones basicas, si no estan validades lanza excepcion

            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new ArgumentException("Airline API BaseUrl no está configurada.");
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new InvalidOperationException("AirlineApi:ApiKey no está configurada en el .env");

            _httpClient.BaseAddress = new Uri(baseUrl);

            if (!_httpClient.DefaultRequestHeaders.Contains("X-API-Key"))
            {
                _httpClient.DefaultRequestHeaders.Add("X-API-Key", apiKey);
             }
        }



        //Es el metodo principal que va a validar un ticket, lo transforma y envia a la API externa
        public async Task<TicketValidationResponse> ValidateTicketAsync(TicketValidationRequest request)
        {
            try
            {
                var externalRequest = new ExternalTicketValidationRequest(request.TicketNumber, request.FlightCode);

                var response = await _httpClient.PostAsJsonAsync("/api/ticket/validar", externalRequest);

                var responseContent = await response.Content.ReadAsStringAsync();



                //Evalua en base a las respuestas posibles de la API externa

                if (response.IsSuccessStatusCode) //Si es valido 
                {
                    bool isValid = responseContent.Trim().Equals("true", StringComparison.OrdinalIgnoreCase);

                    if (isValid)
                    {
                        return new TicketValidationResponse(true, "Ticket validado correctamente por la aerolínea.");
                    }
                    else
                    {
                        return new TicketValidationResponse(false, $"Respuesta inesperada de aerolínea: {responseContent}");
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    return new TicketValidationResponse(false, $"Ticket rechazado: {responseContent}");
                }
                else
                {
                     //Otros posibles erroes de conexión o internos
                    _logger.LogWarning("Error API Aerolínea: {StatusCode} - {Content}", response.StatusCode, responseContent);
                    return new TicketValidationResponse(false, $"Error de comunicación con aerolínea (Código {response.StatusCode})");
                }
            }
            catch (Exception ex)
            {
            
                _logger.LogError(ex, "Excepción conectando con Airline API");
                return new TicketValidationResponse(false, $"Error interno de integración: {ex.Message}");
            }
        }
    }
}
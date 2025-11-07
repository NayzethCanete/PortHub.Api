using PortHub.Api.Dtos;
using PortHub.Api.Interfaces;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace PortHub.Api.Services
{
    public class AirlineIntegrationService : IAirlineIntegrationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AirlineIntegrationService> _logger;

        public AirlineIntegrationService(HttpClient httpClient, ILogger<AirlineIntegrationService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<TicketValidationResponse> ValidateTicketWithAirlineAsync(
            int ticketNumber,
            string flightDate,
            string airlineApiUrl)
        {
            try
            {
                var fullUrl = $"{airlineApiUrl.TrimEnd('/')}/validate-ticket";
                
                // Usamos el DTO de Request que acabamos de crear
                var request = new TicketValidationRequest(ticketNumber, flightDate);

                var response = await _httpClient.PostAsJsonAsync(fullUrl, request);

                if (response.IsSuccessStatusCode)
                {
                    // Usamos el DTO de Response que acabamos de crear
                    var validationResponse = await response.Content
                        .ReadFromJsonAsync<TicketValidationResponse>();

                    if (validationResponse == null)
                    {
                        _logger.LogError("Respuesta de aerolínea fue null");
                        return new TicketValidationResponse(false, "Error: Respuesta de aerolínea inválida");
                    }
                    return validationResponse;
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Error al validar ticket. Status: {StatusCode}, Content: {Content}", response.StatusCode, errorContent);

                return new TicketValidationResponse(false, $"Error al contactar aerolínea: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al validar con aerolínea en {Url}", airlineApiUrl);
                return new TicketValidationResponse(false, $"Error interno al validar ticket: {ex.Message}");
            }
        }
    }
}
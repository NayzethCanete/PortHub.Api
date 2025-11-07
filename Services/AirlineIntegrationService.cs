using System.Net.Http.Json;
using PortHub.Api.Dtos;
using PortHub.Api.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace PortHub.Api.Services
{
    public class AirlineIntegrationService : IAirlineIntegrationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AirlineIntegrationService> _logger;

        public AirlineIntegrationService(HttpClient httpClient, IConfiguration config, ILogger<AirlineIntegrationService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;

            var apiKey = config["AirlineApi:ApiKey"];
            var baseUrl = config["AirlineApi:BaseUrl"];

            if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(baseUrl))
                throw new ArgumentException("Airline API configuration missing.");

            _httpClient.BaseAddress = new Uri(baseUrl);
            _httpClient.DefaultRequestHeaders.Add("X-API-Key", apiKey);
        }

        public async Task<TicketValidationResponse> ValidateTicketAsync(TicketValidationRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/tickets/validate", request);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Airline API responded with {StatusCode}", response.StatusCode);
                    return new TicketValidationResponse(false, $"Error: {response.StatusCode}");
                }

                var result = await response.Content.ReadFromJsonAsync<TicketValidationResponse>();
                return result ?? new TicketValidationResponse(false, "No se pudo leer la respuesta de la aerolínea");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error comunicando con Airline API");
                return new TicketValidationResponse(false, $"Excepción al validar ticket: {ex.Message}");
            }
        }
    }
}

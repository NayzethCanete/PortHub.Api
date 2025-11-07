using PortHub.Api.Dtos;

namespace PortHub.Api.Interfaces
{
    public interface IAirlineIntegrationService
    {
        Task<TicketValidationResponse> ValidateTicketAsync(TicketValidationRequest request);
    }
}
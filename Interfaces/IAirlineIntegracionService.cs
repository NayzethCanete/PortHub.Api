using PortHub.Api.Dtos;
using System.Threading.Tasks;

namespace PortHub.Api.Interfaces
{
    public interface IAirlineIntegrationService
    {
        Task<TicketValidationResponse> ValidateTicketWithAirlineAsync(
            int ticketNumber, 
            string flightDate, 
            string airlineApiUrl
        );
    }
}
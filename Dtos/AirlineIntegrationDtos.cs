using System.ComponentModel.DataAnnotations;

namespace PortHub.Api.Dtos
{
    // DTO que PortHub envía a la Aerolínea para validar un ticket
    public record TicketValidationRequest(
        [Required] int TicketNumber,
        [Required] string FlightDate
    );
    
    // DTO que PortHub espera de vuelta de la Aerolínea
    public record TicketValidationResponse(
        [Required] bool IsValid,
        string Message
    );
}
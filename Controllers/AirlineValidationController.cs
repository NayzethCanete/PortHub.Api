using Microsoft.AspNetCore.Mvc;
using PortHub.Api.Dtos;
using Microsoft.AspNetCore.Authorization;

/* DATOS DE BODY, PARA TESTEAR API DE VALIDACIÓN:
{
  "TicketNumber": 123,
  "FlightDate": "2025-11-04"
}
*/

namespace PortHub.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/airline")]
    public class AirlineValidationController : ControllerBase
    {
        [HttpPost("validate-ticket")]
        public ActionResult<TicketValidationResponse> ValidateTicket([FromBody] TicketValidationRequest request)
        {
            // Simulación de lógica de negocio
            if (request.TicketNumber == 123 && request.FlightDate == "2025-11-04")
            {
                return Ok(new TicketValidationResponse(true, "Ticket válido para la fecha"));
            }
            else if (request.TicketNumber == 123)
            {
                return Ok(new TicketValidationResponse(false, "Ticket no válido para la fecha proporcionada"));
            }
            else
            {
                return Ok(new TicketValidationResponse(false, "Ticket no encontrado"));
            }
        }
    }
}
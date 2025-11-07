using Microsoft.AspNetCore.Mvc;
using PortHub.Api.Dtos;
using Microsoft.AspNetCore.Authorization;

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
            if (request.TicketNumber == "000123" && request.FlightDate.Date == new DateTime(2025,11,04))
            {
                return Ok(new TicketValidationResponse(true, "Ticket válido para la fecha"));
            }
            else if (request.TicketNumber == "000123")
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

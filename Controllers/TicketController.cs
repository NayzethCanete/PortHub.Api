using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PortHub.Api.Interfaces;
using PortHub.Api.Models;
using PortHub.Api.Dtos;

namespace PortHub.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        private static ResponseTicketDto ToDto(Ticket t) =>
            new(t.Id, t.FlightCode, t.PassengerName, t.Seat, t.Status); 

        [HttpGet]
        public IActionResult GetAll()
        {
            var tickets = _ticketService.GetAll().Select(ToDto);
            return Ok(tickets);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var ticket = _ticketService.GetById(id);
            if (ticket == null)
                return NotFound(new { code = "NOT_FOUND", message = "Ticket no encontrado" });

            return Ok(ToDto(ticket));
        }

        [HttpPost]
        public IActionResult Add([FromBody] RequestTicketDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { code = "VALIDATION_ERROR", message = "Datos inválidos", details = ModelState });

            var ticket = new Ticket
            {
                FlightCode= dto.FlightCode,
                PassengerName = dto.PassengerName, 
                Seat = dto.Seat,
                Status = dto.Status ?? "Emitido"
            };

            var created = _ticketService.Add(ticket);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, ToDto(created));
        }

        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] RequestTicketDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { code = "VALIDATION_ERROR", message = "Datos inválidos", details = ModelState });

            var ticket = new Ticket
            {
                Id = id,
                FlightCode = dto.FlightCode, 
                PassengerName = dto.PassengerName, 
                Seat = dto.Seat,
                Status = dto.Status ?? "Emitido"
            };

            var updated = _ticketService.Update(ticket, id);
            if (updated == null)
                return NotFound(new { code = "NOT_FOUND", message = "Ticket no encontrado" });

            return Ok(ToDto(updated));
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var deleted = _ticketService.Delete(id);
            if (!deleted)
                return NotFound(new { code = "NOT_FOUND", message = "Ticket no encontrado" });

            return NoContent();
        }
    }
}
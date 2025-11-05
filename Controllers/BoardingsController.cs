using System; 
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortHub.Api.Dtos;
using PortHub.Api.Interface;
using PortHub.Api.Models;


namespace PortHub.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BoardingController : ControllerBase
    {
        private readonly IBoardingService _boardingService;

        public BoardingController(IBoardingService boardingService)
        {
            _boardingService = boardingService;
        }

        // GET: api/boarding
        [HttpGet]
        public IActionResult GetAll()
        {
            var boardings = _boardingService.GetAll();

            // Pasamos de modelo a DTO para no devolver datos internos
            var response = boardings.Select(b => new BoardingResponseDto(
                b.BoardingId,
                b.TicketId.ToString(),
                b.GateId,
                b.AccessTime,
                b.Validation
            )).ToList();

            return Ok(response);
        }

        // GET: api/boarding/{id}
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var boarding = _boardingService.GetById(id);

            if (boarding == null)
            {
                return NotFound($"No se encontró un embarque con el ID {id}");
            }

            var response = new BoardingResponseDto(
                boarding.BoardingId,
                boarding.TicketId.ToString(),
                boarding.GateId,
                boarding.AccessTime,
                boarding.Validation
            );

            return Ok(response);
        }

        // POST: api/boarding
        [HttpPost]
        public IActionResult Create([FromBody] BoardingRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Convertir el TicketNumber (string) a número
            int.TryParse(dto.TicketNumber, out int ticketId);

            var newBoarding = new Boarding
            {
                TicketId = ticketId,
                GateId = (int)dto.GateId,
                AccessTime = DateTime.UtcNow,
                Validation = false
            };

            var created = _boardingService.Add(newBoarding);

            var response = new BoardingResponseDto(
                created.BoardingId,
                created.TicketId.ToString(),
                created.GateId,
                created.AccessTime,
                created.Validation
            );

            return CreatedAtAction(nameof(GetById), new { id = created.BoardingId }, response);
        }

        // PUT: api/boarding/{id}
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] BoardingRequestDto dto)
        {
            var existing = _boardingService.GetById(id);
            if (existing == null)
                return NotFound($"No se encontró el embarque con ID {id}");

            int.TryParse(dto.TicketNumber, out int ticketId);

            var updated = new Boarding
            {
                TicketId = ticketId,
                GateId = (int)dto.GateId,
                AccessTime = DateTime.UtcNow,
                Validation = true
            };

            var result = _boardingService.Update(updated, id);

            var response = new BoardingResponseDto(
                result.BoardingId,
                result.TicketId.ToString(),
                result.GateId,
                result.AccessTime,
                result.Validation
            );

            return Ok(response);
        }

        // DELETE: api/boarding/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var deleted = _boardingService.Delete(id);

            if (!deleted)
                return NotFound($"No se encontró el embarque con ID {id}");

            return NoContent();
        }
    }
}
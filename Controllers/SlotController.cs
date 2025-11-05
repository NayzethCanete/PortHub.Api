using Microsoft.AspNetCore.Mvc;
using PortHub.Api.Interface;
using PortHub.Api.Models;
using PortHub.Api.Dtos;

namespace PortHub.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class SlotController : ControllerBase
    {
        private readonly ISlotService _slotService;

        public SlotController(ISlotService slotService)
        {
            _slotService = slotService;
        }

        // Helper privado para convertir entidades a DTOs
        private static ResponseSlotDto ToDto(Slot s) =>
            new(s.Id, s.Date, s.Runway, s.Gate_id, s.Status, s.Flight_id);

        // CRUD básico
        [HttpGet]
        public IActionResult GetAll()
        {
            var slots = _slotService.GetAll().Select(ToDto);
            return Ok(slots);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var slot = _slotService.GetById(id);
            if (slot == null)
                return NotFound(new { code = "NOT_FOUND", message = "Slot no encontrado" });

            return Ok(ToDto(slot));
        }

        [HttpPost]
        public IActionResult Add([FromBody] RequestSlotDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { code = "VALIDATION_ERROR", message = "Datos inválidos", details = ModelState });

            try
            {
                var slot = new Slot
                {
                    Date = dto.Date,
                    Runway = dto.Runway,
                    Gate_id = dto.Gate_id,
                    Status = dto.Status,
                    Flight_id = dto.Flight_id
                };

                var created = _slotService.Add(slot);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, ToDto(created));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { code = "DUPLICATE_SLOT", message = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] RequestSlotDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { code = "VALIDATION_ERROR", message = "Datos inválidos", details = ModelState });

            var slot = new Slot
            {
                Id = id,
                Date = dto.Date,
                Runway = dto.Runway,
                Gate_id = dto.Gate_id,
                Status = dto.Status,
                Flight_id = dto.Flight_id
            };

            var updated = _slotService.Update(slot, id);
            if (updated == null)
                return NotFound(new { code = "NOT_FOUND", message = "Slot no encontrado" });

            return Ok(ToDto(updated));
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var deleted = _slotService.Delete(id);
            if (!deleted)
                return NotFound(new { code = "NOT_FOUND", message = "Slot no encontrado" });

            return NoContent();
        }

        // Integración con Aerolínea
        [HttpPost("reserve")]
        public IActionResult Reserve([FromBody] RequestSlotDto dto)
        {
            var slot = new Slot
            {
                Date = dto.Date,
                Runway = dto.Runway,
                Gate_id = dto.Gate_id,
                Status = "Reservado",
                Flight_id = dto.Flight_id
            };

            var reserved = _slotService.ReserveSlot(slot);
            return Ok(ToDto(reserved));
        }

        [HttpPost("confirm/{id:int}")]
        public IActionResult Confirm(int id)
        {
            try
            {
                var confirmed = _slotService.ConfirmSlot(id);
                return Ok(ToDto(confirmed));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { code = "NOT_FOUND", message = ex.Message });
            }
        }

        [HttpPost("cancel/{id:int}")]
        public IActionResult Cancel(int id)
        {
            try
            {
                var canceled = _slotService.CancelSlot(id);
                return Ok(ToDto(canceled));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { code = "NOT_FOUND", message = ex.Message });
            }
        }
    }
}

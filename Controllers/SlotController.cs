using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PortHub.Api.Interfaces;
using PortHub.Api.Models;
using PortHub.Api.Dtos;
using PortHub.Api.Security;

namespace PortHub.Api.Controllers
{
    [Authorize]
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

        private static ResponseSlotDto ToDto(Slot s) =>
            new(
                s.Id,
                s.ScheduleTime,
                s.Runway,
                s.GateId ?? 0,
                s.Status,
                s.FlightCode,
                s.ReservationExpiresAt
            );

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

        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] RequestSlotDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { code = "VALIDATION_ERROR", message = "Datos inválidos", details = ModelState });

            var slot = new Slot
            {
                Id = id,
                ScheduleTime = dto.Date,
                Runway = dto.Runway,
                GateId = dto.Gate_id,
                Status = dto.Status,
                FlightCode = dto.FlightCode
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

        [HttpPost("reserve")]
        [RequireApiKey]
        [ProducesResponseType(typeof(ResponseSlotDto), 201)]
        [ProducesResponseType(409)]
        public IActionResult Reserve([FromBody] RequestSlotDto dto)
        {
            // 1. Obtener el ID de la aerolínea autenticada
            int? airlineId = HttpContext.Items["AirlineId"] as int?;

            var slot = new Slot
            {
                ScheduleTime = dto.Date,
                Runway = dto.Runway,
                GateId = dto.Gate_id,
                FlightCode = dto.FlightCode,
                AirlineId = airlineId 
            };

            try
            {
                var reserved = _slotService.ReserveSlot(slot);
                return CreatedAtAction(nameof(GetById), new { id = reserved.Id }, ToDto(reserved));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { code = "DUPLICATE_SLOT", message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                 return BadRequest(new { code = "INVALID_DATA", message = ex.Message });
            }
        }

        [HttpPost("confirm/{id:int}")]
        [RequireApiKey]
        [ProducesResponseType(typeof(ResponseSlotDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)] 
        [ProducesResponseType(404)]
        public IActionResult Confirm(int id)
        {
            try
            {
                
                var airlineId = (int)HttpContext.Items["AirlineId"]!;
                
                
                var confirmed = _slotService.ConfirmSlot(id, airlineId);
                return Ok(ToDto(confirmed));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { code = "NOT_FOUND", message = ex.Message });
            }
            catch (UnauthorizedAccessException ex) 
            {
                return StatusCode(403, new { code = "FORBIDDEN", message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { code = "INVALID_OPERATION", message = ex.Message });
            }
        }

        [HttpPost("cancel/{id:int}")]
        [RequireApiKey]
        [ProducesResponseType(typeof(ResponseSlotDto), 200)]
        [ProducesResponseType(403)] 
        [ProducesResponseType(404)]
        public IActionResult Cancel(int id)
        {
            try
            {
                
                var airlineId = (int)HttpContext.Items["AirlineId"]!;

                
                var canceled = _slotService.CancelSlot(id, airlineId);
                return Ok(ToDto(canceled));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { code = "NOT_FOUND", message = ex.Message });
            }
            catch (UnauthorizedAccessException ex) 
            {
                return StatusCode(403, new { code = "FORBIDDEN", message = ex.Message });
            }
        }
    }
}
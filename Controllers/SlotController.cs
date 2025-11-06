using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PortHub.Api.Interfaces;
using PortHub.Api.Models;
using PortHub.Api.Dtos;
//using PortHub.Api.Security; // <-- Importante: Para la API Key
//using Microsoft.AspNetCore.Authorization; // <-- Importante: Para JWT

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
                s.FlightCode
            );

  //      [Authorize]
        [HttpGet]
        public IActionResult GetAll()
        {
            var slots = _slotService.GetAll().Select(ToDto);
            return Ok(slots);
        }

    //    [Authorize] 
        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var slot = _slotService.GetById(id);
            if (slot == null)
                return NotFound(new { code = "NOT_FOUND", message = "Slot no encontrado" });

            return Ok(ToDto(slot));
        }

      //  [Authorize] 
        [HttpPost]
        public IActionResult Add([FromBody] RequestSlotDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { code = "VALIDATION_ERROR", message = "Datos inválidos", details = ModelState });

            try
            {
                
                var slot = new Slot
                {
                    ScheduleTime = dto.Date,
                    Runway = dto.Runway,
                    GateId = dto.Gate_id,    
                    Status = dto.Status,
                    FlightCode = dto.FlightCode  
                };

                var created = _slotService.Add(slot);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, ToDto(created));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { code = "DUPLICATE_SLOT", message = ex.Message });
            }
        }

       // [Authorize] 
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

        //[Authorize] 
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var deleted = _slotService.Delete(id);
            if (!deleted)
                return NotFound(new { code = "NOT_FOUND", message = "Slot no encontrado" });

            return NoContent();
        }


        [HttpPost("reserve")]
        //[ApiKeyAuthorize] 
        public IActionResult Reserve([FromBody] RequestSlotDto dto)
        {
            var slot = new Slot
            {
                ScheduleTime = dto.Date, 
                Runway = dto.Runway,
                GateId = dto.Gate_id,     
                Status = "Reservado",     
                FlightCode = dto.FlightCode  
            };

            var reserved = _slotService.ReserveSlot(slot);
            return Ok(ToDto(reserved));
        }

        [HttpPost("confirm/{id:int}")]
       // [ApiKeyAuthorize] 
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
        //[ApiKeyAuthorize] 
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
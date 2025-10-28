
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Linq;
using PortHub.Api.Dtos;
using PortHub.Api.Interface;
using PortHub.Api.Models;

namespace PortHub.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class SlotsController : ControllerBase
    {
        private readonly ISlotService _slotService;

        public SlotsController(ISlotService slotService)
        {
            _slotService = slotService;
        }

        // GET: api/slots (LISTAR)
        [HttpGet]
        public IActionResult GetAll()
        {
            var slots = _slotService.GetAll();
            var response = slots.Select(a => new ResponseSlotDto(
                a.Id,
                a.Date,
                a.Runway,
                a.Gate_id,
                a.Status,
                a.Flight_id
            )).ToList();
            return Ok(response);
        }

        // GET: api/slots/{id} (LISTAR POR ID)
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var slot = _slotService.GetById(id);
            if (slot == null)
                return NotFound($"No se encontró el Slot con el ID {id}");
            var response = new ResponseSlotDto(
                slot.Id,
                slot.Date,
                slot.Runway,
                slot.Gate_id,
                slot.Status,
                slot.Flight_id
            );
            return Ok(response);
        }

        // POST: api/slots (NUEVO SLOT)
        [HttpPost]
        public IActionResult Create([FromBody] RequestSlotDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var newSlot = new Slot
            {
                Id = dto.id,
                Date = dto.Date,
                Runway = dto.Runway,
                Gate_id = dto.Gate_id,
                Status = dto.Status,
                Flight_id = dto.Flight_id
            };

            var created = _slotService.Add(newSlot);

            var response = new ResponseSlotDto(
                created.Id,
                created.Date,
                created.Runway,
                created.Gate_id,
                created.Status,
                created.Flight_id
            );

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
        }

        // PUT: api/slots/{id} (MODIFICAR SLOT)
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] RequestSlotDto dto)
        {
            var existing = _slotService.GetById(id);
            if (existing == null)
                return NotFound($"No se encontró el Slot con ID {id}");

                existing.Id = dto.id;
                existing.Date = dto.Date;
                existing.Runway = dto.Runway;
                existing.Gate_id = dto.Gate_id;
                existing.Status = dto.Status;
                existing.Flight_id = dto.Flight_id;

            var updated = _slotService.Update(existing, id);

            if (updated == null)
                return NotFound($"No se pudo actualizar el Slot con ID {id}");

            var response = new ResponseSlotDto(
                updated.Id,
                updated.Date,
                updated.Runway,
                updated.Gate_id,
                updated.Status,
                updated.Flight_id
            );

            return Ok(response);
        }

        // DELETE: api/slots/{id} (BORRAR SLOT)
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var deleted = _slotService.Delete(id);

            if (!deleted)
                return NotFound($"No se encontró el Slot con ID {id}");

            return NoContent();
        }

         // --- Endpoints para integración con la aerolínea ---

        // RESERVA: la aerolínea solicita reservar un slot
        [HttpPost("airline/reserve")]
        public async Task<IActionResult> AirlineReserve([FromBody] AirlineReserveRequest req)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorResponse { Code = "VALIDATION_ERROR", Message = "Datos inválidos", Details = ModelState });

            // Opcional: mapear preferred gate code a GateId
            Guid? gateId = null;
            if (!string.IsNullOrWhiteSpace(req.PreferredGateCode))
            {
                var gates = await _gateRepo.GetAllAsync();
                var g = gates.FirstOrDefault(x => string.Equals(x.Code, req.PreferredGateCode, StringComparison.OrdinalIgnoreCase));
                if (g != null) gateId = g.Id;
            }

            var slot = new Slot
            {
                ScheduledAt = req.ScheduledAt.ToUniversalTime(),
                Runway = req.Runway,
                GateId = gateId,
                FlightNumber = req.FlightNumber,
                AssignedToAirlineCode = req.AirlineCode,
                State = SlotState.Reserved
            };

            try
            {
                var created = await _slotService.ReserveFromAirlineAsync(slot);
                var res = new AirlineReserveResponse { SlotId = created.Id, Status = "reserved" };
                return Ok(res);
            }
            catch (SlotConflictException ex)
            {
                return Conflict(new AirlineReserveResponse { SlotId = Guid.Empty, Status = "rejected", Reason = ex.Message });
            }
        }

        // CONFIRMACIÓN: por parte de aerolínea (o sistema). Confirma slot existente
        [HttpPost("airline/confirm/{slotId:guid}")]
        public async Task<IActionResult> AirlineConfirm(Guid slotId)
        {
            try
            {
                await _slotService.ConfirmSlotAsync(slotId);
                return Ok(new { status = "confirmed" });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new ErrorResponse { Code = "NOT_FOUND", Message = "Slot no encontrado", Details = null });
            }
        }

        // CANCELAR: desde aerolínea -> libera el slot
        [HttpPost("airline/cancel/{slotId:guid}")]
        public async Task<IActionResult> AirlineCancel(Guid slotId)
        {
            try
            {
                await _slotService.CancelSlotAsync(slotId);
                return Ok(new { status = "cancelled" });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new ErrorResponse { Code = "NOT_FOUND", Message = "Slot no encontrado", Details = null });
            }
        }
    }
}

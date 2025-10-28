
using System; 
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortHub.Api.Dtos;
using PortHub.Api.Interfaces;
using PortHub.Api.Models;

namespace PortHub.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SlotsController : ControllerBase
    {
        private readonly ISlotService _slotService;

        public SlotsController(ISlotService slotService)
        {
            _slotService = slotService;
        }

        // GET: api/slots
        [HttpGet]
        public IActionResult GetAll()
        {
            foreach (var claim in User.Claims)
        {
            Console.WriteLine(claim.Type + ": " + claim.Value);
        }
            var slots = _slotService.GetAll();

            var response = slots.Select(a => new SlotResponseDto(
                a.Id,
                a.Name,
                a.Code,
                a.Country,
                a.BaseAddress
            )).ToList();

            return Ok(response);
        }

        // GET: api/slots/{id}
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var slot = _slotService.GetById(id);

            if (slot == null)
                return NotFound($"No se encontró una aerolínea con el ID {id}");

            var response = new SlotResponseDto(
                slot.Id,
                slot.Name,
                slot.Code,
                slot.Country,
                slot.BaseAddress
            );

            return Ok(response);
        }

        // POST: api/slots
        [HttpPost]
        public IActionResult Create([FromBody] SlotRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var newSlot = new Slot
            {
                Name = dto.Name,
                Code = dto.Code,
                Country = dto.Country,
                BaseAddress = dto.BaseAddress
            };

            var created = _slotService.Add(newSlot);

            var response = new SlotResponseDto(
                created.Id,
                created.Name,
                created.Code.ToString(),
                created.Country,
                created.BaseAddress
            );

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
        }

        // PUT: api/slots/{id}
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] SlotRequestDto dto)
        {
            var existing = _slotService.GetById(id);
            if (existing == null)
                return NotFound($"No se encontró la aerolínea con ID {id}");

            existing.Name = dto.Name;
            existing.Code = dto.Code;
            existing.Country = dto.Country;
            existing.BaseAddress = dto.BaseAddress;

            var updated = _slotService.Update(existing, id);

            var response = new SlotResponseDto(
                updated.Id,
                updated.Name,
                updated.Code,
                updated.Country,
                updated.BaseAddress
            );

            return Ok(response);
        }

        // DELETE: api/slots/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var deleted = _slotService.Delete(id);

            if (!deleted)
                return NotFound($"No se encontró la aerolínea con ID {id}");

            return NoContent();
        }
    }
}

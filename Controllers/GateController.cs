using Microsoft.AspNetCore.Mvc;
using PortHub.Api.Models;
using PortHub.Api.Interfaces;
using PortHub.Api.Dtos;
using Microsoft.AspNetCore.Authorization;

namespace PortHub.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class GateController : ControllerBase
    {
        private readonly IGateService _gateService;

        public GateController(IGateService gateService)
        {
            _gateService = gateService;
        }

        private static ResponseGateDto ToDto(Gate g) =>
            new(g.Name, g.Location);

        [HttpGet]
        public IActionResult GetAll()
        {
            var gates = _gateService.GetAll().Select(ToDto);
            return Ok(gates);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var gate = _gateService.GetById(id);
            if (gate == null)
                return NotFound(new { code = "NOT_FOUND", message = "Gate no encontrado" });

            return Ok(ToDto(gate));
        }

        [HttpPost]
        public IActionResult Add([FromBody] RequestGateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { code = "VALIDATION_ERROR", message = "Datos inválidos", details = ModelState });

            var gate = new Gate
            {
                Name = dto.Name,
                Location = dto.Location
            };

            var created = _gateService.Add(gate);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, ToDto(created));
        }

        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] RequestGateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { code = "VALIDATION_ERROR", message = "Datos inválidos", details = ModelState });

            var gate = new Gate
            {
                Id = id,
                Name = dto.Name,
                Location = dto.Location
            };

            var updated = _gateService.Update(gate, id);
            if (updated == null)
                return NotFound(new { code = "NOT_FOUND", message = "Gate no encontrado" });

            return Ok(ToDto(updated));
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var deleted = _gateService.Delete(id);
            if (!deleted)
                return NotFound(new { code = "NOT_FOUND", message = "Gate no encontrado" });

            return NoContent();
        }
    }
}

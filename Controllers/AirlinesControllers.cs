
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
    public class AirlinesController : ControllerBase
    {
        private readonly IAirlineService _airlineService;

        public AirlinesController(IAirlineService airlineService)
        {
            _airlineService = airlineService;
        }

        // GET: api/airlines
        [HttpGet]
        public IActionResult GetAll()
        {
            foreach (var claim in User.Claims)
        {
            Console.WriteLine(claim.Type + ": " + claim.Value);
        }
            var airlines = _airlineService.GetAll();

            var response = airlines.Select(a => new AirlineResponseDto(
                a.Id,
                a.Name,
                a.Code,
                a.Country,
                a.BaseAddress
            )).ToList();

            return Ok(response);
        }

        // GET: api/airlines/{id}
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var airline = _airlineService.GetById(id);

            if (airline == null)
                return NotFound($"No se encontró una aerolínea con el ID {id}");

            var response = new AirlineResponseDto(
                airline.Id,
                airline.Name,
                airline.Code,
                airline.Country,
                airline.BaseAddress
            );

            return Ok(response);
        }

        // POST: api/airlines
        [HttpPost]
        public IActionResult Create([FromBody] AirlineRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var newAirline = new Airline
            {
                Name = dto.Name,
                Code = dto.Code,
                Country = dto.Country,
                BaseAddress = dto.BaseAddress
            };

            var created = _airlineService.Add(newAirline);

            var response = new AirlineResponseDto(
                created.Id,
                created.Name,
                created.Code.ToString(),
                created.Country,
                created.BaseAddress
            );

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
        }

        // PUT: api/airlines/{id}
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] AirlineRequestDto dto)
        {
            var existing = _airlineService.GetById(id);
            if (existing == null)
                return NotFound($"No se encontró la aerolínea con ID {id}");

            existing.Name = dto.Name;
            existing.Code = dto.Code;
            existing.Country = dto.Country;
            existing.BaseAddress = dto.BaseAddress;

            var updated = _airlineService.Update(existing, id);

            var response = new AirlineResponseDto(
                updated.Id,
                updated.Name,
                updated.Code,
                updated.Country,
                updated.BaseAddress
            );

            return Ok(response);
        }

        // DELETE: api/airlines/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var deleted = _airlineService.Delete(id);

            if (!deleted)
                return NotFound($"No se encontró la aerolínea con ID {id}");

            return NoContent();
        }
    }
}
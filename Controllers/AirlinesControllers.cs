using System; 
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortHub.Api.Dtos;
using PortHub.Api.Interfaces;
using PortHub.Api.Models;

namespace PortHub.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AirlinesController : ControllerBase
{
    private readonly IAirlineService _airlineService;

    public AirlinesController(IAirlineService airlineService)
    {
        _airlineService = airlineService;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var airlines = _airlineService.GetAll();
        var response = airlines.Select(a => new AirlineResponseDto(
            a.Id,
            a.Name,
            a.Code,
            a.Country,
            a.BaseAddress,
            a.ApiUrl
        )).ToList();
        return Ok(response);
    }

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
            airline.BaseAddress,
            airline.ApiUrl
        );
        return Ok(response);
    }

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
            BaseAddress = dto.BaseAddress,
            ApiUrl = dto.ApiUrl
        };

        var created = _airlineService.Add(newAirline);

        var response = new AirlineResponseDto(
            created.Id,
            created.Name,
            created.Code,
            created.Country,
            created.BaseAddress,
            created.ApiUrl,
            created.ApiKey  // Incluir API Key solo al crear
        );

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
    }

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
        existing.ApiUrl = dto.ApiUrl;

        var updated = _airlineService.Update(existing, id);

        var response = new AirlineResponseDto(
            updated.Id,
            updated.Name,
            updated.Code,
            updated.Country,
            updated.BaseAddress,
            updated.ApiUrl
        );

        return Ok(response);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var deleted = _airlineService.Delete(id);
        if (!deleted)
            return NotFound($"No se encontró la aerolínea con ID {id}");

        return NoContent();
    }
}
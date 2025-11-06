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
public class BoardingController : ControllerBase
{
    private readonly IBoardingService _boardingService;

    public BoardingController(IBoardingService boardingService)
    {
        _boardingService = boardingService;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var boardings = _boardingService.GetAll();

        var response = boardings.Select(b => new BoardingResponseDto(
            b.BoardingId,
            b.TicketId.ToString(),
            b.Slot.GateId ?? 0,
            b.AccessTime,
            b.Validation
        )).ToList();

        return Ok(response);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var boarding = _boardingService.GetById(id);

        if (boarding == null)
        {
            return NotFound($"No se encontró un boarding con el ID {id}");
        }

        var response = new BoardingResponseDto(
            boarding.BoardingId,
            boarding.TicketId.ToString(),
            boarding.Slot?.GateId ?? 0,
            boarding.AccessTime,
            boarding.Validation
        );

        return Ok(response);
    }

    [HttpPost]
    public IActionResult Create([FromBody] BoardingRequestDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        int.TryParse(dto.TicketNumber, out int ticketId);

        var newBoarding = new Boarding
        {
            TicketId = ticketId,
            SlotId = dto.SlotId,
            AccessTime = DateTime.UtcNow,
            Validation = false
        };

        var created = _boardingService.Add(newBoarding);

        var response = new BoardingResponseDto(
            created.BoardingId,
            created.TicketId.ToString(),
            created.Slot?.GateId ?? 0, 
            created.AccessTime,
            created.Validation
        );

        return CreatedAtAction(nameof(GetById), new { id = created.BoardingId }, response);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] BoardingRequestDto dto)
    {
        var existing = _boardingService.GetById(id);
        if (existing == null)
            return NotFound($"No se encontró el boarding con ID {id}");

        int.TryParse(dto.TicketNumber, out int ticketId);

        existing.TicketId = ticketId;
        existing.AccessTime = DateTime.UtcNow;
        existing.Validation = true;

        var result = _boardingService.Update(existing, id);

        var response = new BoardingResponseDto(
            result.BoardingId,
            result.TicketId.ToString(),
            result.Slot?.GateId ?? 0, 
            result.AccessTime,
            result.Validation
        );

        return Ok(response);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var deleted = _boardingService.Delete(id);

        if (!deleted)
            return NotFound($"No se encontró el boarding con ID {id}");

        return NoContent();
    }
}
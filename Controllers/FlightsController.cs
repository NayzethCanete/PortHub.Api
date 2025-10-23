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
    public class FlightsController : ControllerBase
    {
        private readonly IFlightService _flightService;

        public FlightsController(IFlightService flightService)
        {
            _flightService = flightService;
        }

        // GET: api/flights
        [HttpGet]
        public IActionResult GetAll()
        {
            var flights = _flightService.GetAll();

            var response = flights.Select(f => new FlightResponseDto(
                f.FlightId,
                f.FlightCode,
                DateTime.UtcNow,  // temporal hasta tener campos reales de horarios
                DateTime.UtcNow.AddHours(2),
                long.TryParse(f.AirlineId, out var id) ? id : 0
            )).ToList();

            return Ok(response);
        }

        // GET: api/flights/{id}
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var flight = _flightService.GetById(id);

            if (flight == null)
                return NotFound($"No se encontró el vuelo con ID {id}");

            var response = new FlightResponseDto(
                flight.FlightId,
                flight.FlightCode,
                DateTime.UtcNow, 
                DateTime.UtcNow.AddHours(2),
                long.TryParse(flight.AirlineId, out var idNum) ? idNum : 0
            );

            return Ok(response);
        }

        // POST: api/flights
        [HttpPost]
        public IActionResult Create([FromBody] FlightRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var newFlight = new Flight
            {
                FlightCode = dto.FlightNumber,
                AirlineId = dto.AirlineId.ToString(),
                Origin = "Desconocido",
                Destination = "Desconocido",
                Status = "Programado",
                SlotId = null
            };

            var created = _flightService.Add(newFlight);

            var response = new FlightResponseDto(
                created.FlightId,
                created.FlightCode,
                dto.DepartureTime,
                dto.ArrivalTime,
                dto.AirlineId
            );

            return CreatedAtAction(nameof(GetById), new { id = created.FlightId }, response);
        }

        // PUT: api/flights/{id}
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] FlightRequestDto dto)
        {
            var existing = _flightService.GetById(id);
            if (existing == null)
                return NotFound($"No se encontró el vuelo con ID {id}");

            existing.FlightCode = dto.FlightNumber;
            existing.AirlineId = dto.AirlineId.ToString();
            existing.Status = "Libre";
            existing.Origin = "Pendiente";
            existing.Destination = "Pendiente";

            var updated = _flightService.Update(existing, id);

            var response = new FlightResponseDto(
                updated.FlightId,
                updated.FlightCode,
                dto.DepartureTime,
                dto.ArrivalTime,
                dto.AirlineId
            );

            return Ok(response);
        }

        // DELETE: api/flights/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var deleted = _flightService.Delete(id);

            if (!deleted)
                return NotFound($"No se encontró el vuelo con ID {id}");

            return NoContent();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PortHubApi.Interfaces;
using PortHubApi.Models;
using PortHubApi.Models.Dtos;

namespace PortHubApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FlightController : ControllerBase
    {
        private readonly IFlightService _flightService;

        public FlightController(IFlightService flightService)
        {
            _flightService = flightService;
        }

        // GET /api/flight
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult<List<FlightDto>> GetAll()
        {
            var flights = _flightService.GetAll();
            var dtoList = flights.Select(f => new FlightDto(
                f.FlightId,
                f.FlightCode,
                f.AirlineId,
                f.Origin,
                f.Destination,
                f.Status,
                f.SlotId)).ToList();

            return Ok(dtoList);
        }

        // GET /api/flight/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult<FlightDto> GetById(int id)
        {
            var f = _flightService.GetById(id);
            if (f == null)
                return NotFound($"No se encontró el vuelo con ID {id}");

            var dto = new FlightDto(
                f.FlightId,
                f.FlightCode,
                f.AirlineId,
                f.Origin,
                f.Destination,
                f.Status,
                f.SlotId);

            return Ok(dto);
        }

        // POST /api/flight
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult<FlightDto> Register([FromBody] RegisterFlightDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var flight = _flightService.RegisterFlight(dto);

            if (flight == null)
                return BadRequest("No se pudo registrar el vuelo (puede que ya exista o falte información).");

            var response = new FlightDto(
                flight.FlightId,
                flight.FlightCode,
                flight.AirlineId,
                flight.Origin,
                flight.Destination,
                flight.Status,
                flight.SlotId);

            return CreatedAtAction(nameof(GetById), new { id = flight.FlightId }, response);
        }

        // PUT /api/flight/{id}/status
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public ActionResult UpdateStatus(int id, [FromBody] UpdateFlightStatusDto dto)
        {
            var updated = _flightService.UpdateStatus(id, dto.Status);

            if (!updated)
                return NotFound($"No se encontró el vuelo con ID {id}");

            return Ok($"Estado del vuelo actualizado a '{dto.Status}'");
        }

        // DELETE /api/flight/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            var result = _flightService.Delete(id);

            if (!result)
                return NotFound($"No se encontró el vuelo con ID {id}");

            return NoContent();
        }
    }
}
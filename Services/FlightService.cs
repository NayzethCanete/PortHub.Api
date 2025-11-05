using System;
using System.Collections.Generic;
using System.Linq;
using PortHub.Api.Interface;
using PortHub.Api.Models;

namespace PortHub.Api.Services
{
    public class FlightService : IFlightService
    {
        private readonly List<Flight> _flights;

        public FlightService()
        {
            _flights = new List<Flight>
            {
                new Flight { FlightId = 1, FlightCode = "FL100", AirlineId = "1", Origin = "Buenos Aires", Destination = "Santiago", Status = "En horario", SlotId = "S01" },
                new Flight { FlightId = 2, FlightCode = "FL200", AirlineId = "2", Origin = "Lima", Destination = "Bogotá", Status = "Demorado", SlotId = "S02" },
                new Flight { FlightId = 3, FlightCode = "FL300", AirlineId = "3", Origin = "Montevideo", Destination = "Asunción", Status = "Cancelado", SlotId = "S03" }
            };
        }

        public List<Flight> GetAll()
        {
            return _flights.ToList();
        }

        public Flight? GetById(int id)
        {
            return _flights.FirstOrDefault(f => f.FlightId == id);
        }

        public Flight Add(Flight flight)
        {
            var nextId = _flights.Any() ? _flights.Max(f => f.FlightId) + 1 : 1;
            flight.FlightId = nextId;
            _flights.Add(flight);
            return flight;
        }

        public Flight Update(Flight flight, int id)
        {
            var existing = _flights.FirstOrDefault(f => f.FlightId == id);
            if (existing == null)
                throw new KeyNotFoundException($"Flight con ID {id} no encontrado.");

            existing.FlightCode = flight.FlightCode;
            existing.AirlineId = flight.AirlineId;
            existing.Origin = flight.Origin;
            existing.Destination = flight.Destination;
            existing.Status = flight.Status;
            existing.SlotId = flight.SlotId;

            return existing;
        }

        public bool Delete(int id)
        {
            var existing = _flights.FirstOrDefault(f => f.FlightId == id);
            if (existing == null) return false;
            _flights.Remove(existing);
            return true;
        }
    }
}

/*using System;
using System.Collections.Generic;
using System.Linq;
using PortHub.Api.Interfaces;
using PortHub.Api.Models;
using PortHub.Api.Data;  
using Microsoft.EntityFrameworkCore;

namespace PortHub.Api.Services
{
    public class FlightService : IFlightService
    {
        private readonly AppDbContext _context;

        public FlightService(AppDbContext context)
        {
            _context = context;
        }

        public List<Flight> GetAll()
        {
            return _context.Flights.Include(f => f.Airline).ToList();
        }

        public Flight? GetById(int id)
        {
            return _context.Flights.Include(f => f.Airline)
                .FirstOrDefault(f => f.FlightId == id);
        }

        public Flight Add(Flight flight)
        {
            _context.Flights.Add(flight);
             _context.SaveChanges();
             return flight;
        }

        public Flight Update(Flight flight, int id)
        {
            var existing = _context.Flights.Find(id);
            if (existing == null)
                throw new KeyNotFoundException($"Flight con id {id} no se encontro.");
            
            existing.FlightCode = flight.FlightCode;
            existing.AirlineId = flight.AirlineId;
            existing.Origin = flight.Origin;
            existing.Destination = flight.Destination;
            existing.Status = flight.Status;
            existing.SlotId = flight.SlotId;

             _context.SaveChanges();
             return existing;
        }

        public bool Delete(int id)
        {
            var existing = _context.Flights.Find(id);
            if (existing == null) return false;
            
            _context.Flights.Remove(existing);
            _context.SaveChanges();
            return true;
        }
    }
}*/

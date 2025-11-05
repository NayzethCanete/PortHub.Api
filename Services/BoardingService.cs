using System;
using System.Collections.Generic;
using System.Linq;
using PortHub.Api.Interface;
using PortHub.Api.Models;

namespace PortHub.Api.Services
{
    public class BoardingService : IBoardingService
    {
        //Implementacion en memoria para testeo.
        private readonly List<Boarding> _boardings;

        public BoardingService()
        {
            _boardings = new List<Boarding>
            {
                new Boarding { BoardingId = 1, TicketId = 1001, AccessTime = DateTime.UtcNow.AddMinutes(-30), GateId = 5, Validation = true },
                new Boarding { BoardingId = 2, TicketId = 1002, AccessTime = DateTime.UtcNow.AddMinutes(-20), GateId = 3, Validation = true },
                new Boarding { BoardingId = 3, TicketId = 1003, AccessTime = DateTime.UtcNow.AddMinutes(-10), GateId = 7, Validation = false }
            };
        }

        public List<Boarding> GetAll()
        {
            return _boardings.ToList();
        }

        public Boarding? GetById(int id)
        {
            return _boardings.FirstOrDefault(b => b.BoardingId == id);
        }

        public Boarding Add(Boarding boarding)
        {
            var nextId = _boardings.Any() ? _boardings.Max(b => b.BoardingId) + 1 : 1;
            boarding.BoardingId = nextId;
            _boardings.Add(boarding);
            return boarding;
        }

        public Boarding Update(Boarding boarding, int id)
        {
            var existing = _boardings.FirstOrDefault(b => b.BoardingId == id);
            if (existing == null) throw new KeyNotFoundException($"Boarding with id {id} not found.");

            existing.TicketId = boarding.TicketId;
            existing.AccessTime = boarding.AccessTime;
            existing.GateId = boarding.GateId;
            existing.Validation = boarding.Validation;

            return existing;
        }

        public bool Delete(int id)
        {
            var existing = _boardings.FirstOrDefault(b => b.BoardingId == id);
            if (existing == null) return false;
            _boardings.Remove(existing);
            return true;
        }
    }
}
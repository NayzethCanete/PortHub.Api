using System;
using System.Collections.Generic;
using System.Linq;
using PortHub.Api.Interface;
using PortHub.Api.Models;

namespace PortHub.Api.Services
{
    public class AirlineService : IAirlineService
    {
        // Implementación en memoria (temporal) para prueba.Despues se reemplaza por dbcontext y bd. 
        private readonly List<Airline> _airlines;

        public AirlineService()
        {
            _airlines = new List<Airline>
            {
                new Airline { Id = 1, Name = "AeroSol", Code = "AS", Country = "Argentina", BaseAddress = "Buenos Aires" },
                new Airline { Id = 2, Name = "Pacific Wings", Code = "PW", Country = "Chile", BaseAddress = "Santiago" },
                new Airline { Id = 3, Name = "Andes Air", Code = "AA", Country = "Perú", BaseAddress = "Lima" },
                new Airline { Id = 4, Name = "CaribeFly", Code = "CF", Country = "México", BaseAddress = "Ciudad de México" },
                new Airline { Id = 5, Name = "LatAm Connect", Code = "LC", Country = "Colombia", BaseAddress = "Bogotá" }
            };
        }

        public List<Airline> GetAll()
        {
            return _airlines.ToList();
        }

        public Airline? GetById(int id)
        {
            return _airlines.FirstOrDefault(a => a.Id == id);
        }

        public Airline Add(Airline airline)
        {
            var nextId = _airlines.Any() ? _airlines.Max(a => a.Id) + 1 : 1;
            airline.Id = nextId;
            _airlines.Add(airline);
            return airline;
        }

        public Airline Update(Airline airline, int id)
        {
            var existing = _airlines.FirstOrDefault(a => a.Id == id);
            if (existing == null) throw new KeyNotFoundException($"Airline with id {id} not found.");

            existing.Name = airline.Name;
            existing.Code = airline.Code;
            existing.Country = airline.Country;
            existing.BaseAddress = airline.BaseAddress;

            return existing;
        }

        public bool Delete(int id)
        {
            var existing = _airlines.FirstOrDefault(a => a.Id == id);
            if (existing == null) return false;
            _airlines.Remove(existing);
            return true;
        }
    }
}
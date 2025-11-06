using PortHub.Api.Models;
using PortHub.Api.Interfaces;
using PortHub.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace PortHub.Api.Services
{
    public class GateService : IGateService
    {
        private readonly AppDbContext _context;

        public GateService(AppDbContext context)
        {
            _context = context;
        }

        public List<Gate> GetAll()
        {
            return _context.Gates.ToList();
        }

        public Gate? GetById(int id)
        {
            return _context.Gates.FirstOrDefault(g => g.Id == id);
        }

        public Gate Add(Gate gate)
        {
            _context.Gates.Add(gate);
            _context.SaveChanges(); 
            return gate;
        }

        public Gate? Update(Gate gate, int id)
        {
            var existing = _context.Gates.FirstOrDefault(g => g.Id == id); // CORREGIDO
            if (existing == null)
                return null;

            existing.Name = gate.Name;
            existing.Location = gate.Location;
            
            _context.SaveChanges(); 
            return existing;
        }

        public bool Delete(int id)
        {
            var gate = _context.Gates.FirstOrDefault(g => g.Id == id);
            if (gate == null) return false;
            
            _context.Gates.Remove(gate); 
            _context.SaveChanges(); 
            return true;
        }
    }
}

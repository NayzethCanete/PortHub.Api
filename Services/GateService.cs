using PortHub.Api.Models;
using PortHub.Api.Interfaces;

namespace PortHub.Api.Services
{
    public class GateService : IGateService
    {
        // Simulación de almacenamiento en memoria
        private static readonly List<Gate> _gates = new();
        private static int _nextId = 1;

        public List<Gate> GetAll()
        {
            return _gates;
        }

        public Gate? GetById(int id)
        {
            return _gates.FirstOrDefault(g => g.Id == id);
        }

        public Gate Add(Gate gate)
        {
            gate.Id = _nextId++;
            _gates.Add(gate);
            return gate;
        }

        public Gate? Update(Gate gate, int id)
        {
            var existing = _gates.FirstOrDefault(g => g.Id == id);
            if (existing == null)
                return null;

            existing.Name = gate.Name;
            existing.Location = gate.Location;
            return existing;
        }

        public bool Delete(int id)
        {
            var gate = _gates.FirstOrDefault(g => g.Id == id);
            if (gate == null) return false;

            _gates.Remove(gate);
            return true;
        }
    }
}

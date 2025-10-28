using PortHub.Api.Models;
using PortHub.Api.Interface;

namespace PortHub.Api.Services
{
    public class SlotService : ISlotService
    {
        // Simulación de base de datos en memoria
        private static readonly List<Slot> _slots = new();
        private static int _nextId = 1;

        public IEnumerable<Slot> GetAll()
        {
            return _slots;
        }
        public Slot? GetById(int id)
        {
            return _slots.FirstOrDefault(s => s.Id == id);
        }

        public Slot Add(Slot slot)
        {
            if (_slots.Any(s => s.Date == slot.Date && s.Runway == slot.Runway))
                throw new InvalidOperationException("Ya hay un Slot con el mismo Horario y Pista.");

            slot.Id = _nextId++;
            slot.Status ??= "Reservado"; //Por Defecto.
            _slots.Add(slot);
            return slot;
        }

        public Slot? Update(Slot slot, int id)
        {
            var existing = _slots.FirstOrDefault(s => s.Id == id);
            if (existing == null)
                return null;

            existing.Date = slot.Date;
            existing.Runway = slot.Runway;
            existing.Gate_id = slot.Gate_id;
            existing.Flight_id = slot.Flight_id;
            existing.Status = slot.Status ?? existing.Status;

            return existing;
        }

        public bool Delete(int id)
        {
            var slot = _slots.FirstOrDefault(s => s.Id == id);
            if (slot == null) return false;

            _slots.Remove(slot);
            return true;
        }

        // Métodos de integración Aerolínea
        public Slot ReserveSlot(Slot slot)
        {
            slot.Status = "Reservado";
            return Add(slot);
        }

        public Slot ConfirmSlot(int id)
        {
            var slot = _slots.FirstOrDefault(s => s.Id == id)
                ?? throw new KeyNotFoundException("Slot no encontrado.");
            slot.Status = "Confirmado";
            return slot;
        }

        public Slot CancelSlot(int id)
        {
            var slot = _slots.FirstOrDefault(s => s.Id == id)
                ?? throw new KeyNotFoundException("Slot no encontrado.");
            slot.Status = "Liberado";
            return slot;
        }
    }
}
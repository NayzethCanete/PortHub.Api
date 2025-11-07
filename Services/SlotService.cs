using PortHub.Api.Models;
using PortHub.Api.Interfaces;
using PortHub.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace PortHub.Api.Services
{
    public class SlotService : ISlotService
    {
        private readonly AppDbContext _context; 
       
        public SlotService(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Slot> GetAll()
        {
            // ANTES: return _slots;
            return _context.Slots.ToList(); // CORREGIDO
        }

        public Slot? GetById(int id)
        {
            // ANTES: return _slots.FirstOrDefault(s => s.Id == id);
            return _context.Slots.FirstOrDefault(s => s.Id == id); // CORREGIDO
        }

        public Slot Add(Slot slot)
        {
            
            if (_context.Slots.Any(s => s.ScheduleTime == slot.ScheduleTime && s.Runway == slot.Runway))
                throw new InvalidOperationException("Ya hay un Slot con el mismo Horario y Pista, por lo cual no se le puede asignar el mismo.");

            // ANTES: slot.Id = _nextId++;
            slot.Status ??= "Reservado";
            
            // ANTES: _slots.Add(slot);
            _context.Slots.Add(slot); // CORREGIDO
            _context.SaveChanges(); // CORREGIDO: Añadir guardado
            return slot;
        }

        public Slot? Update(Slot slot, int id)
        {
            // ANTES: var existing = _slots.FirstOrDefault(s => s.Id == id);
            var existing = _context.Slots.FirstOrDefault(s => s.Id == id); // CORREGIDO
            if (existing == null)
                return null;

            // ANTES: existing.ScheduledTime = slot.ScheduleTime; (Error de tipeo)
            existing.ScheduleTime = slot.ScheduleTime; // CORREGIDO (sin 'd')
            existing.Runway = slot.Runway;
            existing.GateId = slot.GateId;
            existing.FlightCode = slot.FlightCode;
            existing.Status = slot.Status ?? existing.Status;

            _context.SaveChanges(); // CORREGIDO: Añadir guardado
            return existing;
        }

        public bool Delete(int id)
        {
            // ANTES: var slot = _slots.FirstOrDefault(s => s.Id == id);
            var slot = _context.Slots.FirstOrDefault(s => s.Id == id); // CORREGIDO
            if (slot == null) return false;

            // ANTES: _slots.Remove(slot);
            _context.Slots.Remove(slot); // CORREGIDO
            _context.SaveChanges(); // CORREGIDO: Añadir guardado
            return true;
        }

        // Métodos de integración Aerolínea
        public Slot ReserveSlot(Slot slot)
        {
            slot.Status = "Reservado";
            // El método Add ya usa _context y SaveChanges
            return Add(slot);
        }

        public Slot ConfirmSlot(int id)
        {
            // ANTES: var slot = _slots.FirstOrDefault(s => s.Id == id)
            var slot = _context.Slots.FirstOrDefault(s => s.Id == id) // CORREGIDO
                ?? throw new KeyNotFoundException("Slot no encontrado.");
            
            slot.Status = "Confirmado";
            _context.SaveChanges(); // CORREGIDO: Añadir guardado
            return slot;
        }

        public Slot CancelSlot(int id)
        {
            // ANTES: var slot = _slots.FirstOrDefault(s => s.Id == id)
            var slot = _context.Slots.FirstOrDefault(s => s.Id == id) // CORREGIDO
                ?? throw new KeyNotFoundException("Slot no encontrado.");
            
            slot.Status = "Liberado";
            _context.SaveChanges(); // CORREGIDO: Añadir guardado
            return slot;
        }
    }
}
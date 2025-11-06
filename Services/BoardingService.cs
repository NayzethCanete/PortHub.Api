using PortHub.Api.Models;
using PortHub.Api.Interfaces;
using PortHub.Api.Data;
using PortHub.Api.Dtos;
using Microsoft.EntityFrameworkCore;

namespace PortHub.Api.Services
{
    public class BoardingService : IBoardingService
    {
        private readonly AppDbContext _context;
        private readonly ITicketService _ticketService;

        public BoardingService(AppDbContext context, ITicketService ticketService)
        {
            _context = context;
            _ticketService = ticketService;
        }

        public IEnumerable<Boarding> GetAll()
        {
            return _context.Boardings.Include(b => b.Slot).ToList();
        }

        public Boarding? GetById(int id)
        {
            return _context.Boardings
                .Include(b => b.Slot)
                .FirstOrDefault(b => b.BoardingId == id);
        }

        public Boarding Add(Boarding boarding)
        {
            _context.Boardings.Add(boarding);
            _context.SaveChanges();
            return boarding;
        }

        public Boarding? Update(Boarding boarding, int id)
        {
            var existing = _context.Boardings.FirstOrDefault(b => b.BoardingId == id);
            if (existing == null)
                return null;

            existing.TicketId = boarding.TicketId;
            existing.SlotId = boarding.SlotId;
            existing.AccessTime = boarding.AccessTime;
            existing.Validation = boarding.Validation;

            _context.SaveChanges();
            return existing;
        }

        public bool Delete(int id)
        {
            var boarding = _context.Boardings.FirstOrDefault(b => b.BoardingId == id);
            if (boarding == null) return false;

            _context.Boardings.Remove(boarding);
            _context.SaveChanges();
            return true;
        }

        // NUEVO: Validar y registrar embarque
        public async Task<(bool success, string message, Boarding? boarding)> ValidateAndRegisterBoardingAsync(
            int ticketId, 
            int slotId)
        {
            // 1. Verificar que el ticket existe
            var ticket = _context.Tickets.FirstOrDefault(t => t.Id == ticketId);
            if (ticket == null)
            {
                return (false, "Ticket no encontrado", null);
            }

            // 2. Verificar que el slot existe y está confirmado
            var slot = _context.Slots
                .Include(s => s.Gate)
                .FirstOrDefault(s => s.Id == slotId);
            
            if (slot == null)
            {
                return (false, "Slot no encontrado", null);
            }

            if (slot.Status != "Confirmado")
            {
                return (false, $"Slot no está confirmado. Estado actual: {slot.Status}", null);
            }

            // 3. Verificar que el código de vuelo coincide
            if (ticket.FlightCode != slot.FlightCode)
            {
                return (false, "El ticket no corresponde al vuelo del slot", null);
            }

            // 4. Obtener aerolínea para validar con su API
            var airline = _context.Airlines.FirstOrDefault(a => a.Code == slot.FlightCode.Substring(0, 2));
            if (airline == null)
            {
                return (false, "No se pudo identificar la aerolínea", null);
            }

            // 5. Validar con API de aerolínea
            var flightDate = slot.ScheduleTime.ToString("yyyy-MM-dd");
            var validationResult = await _ticketService.ValidateTicketAsync(
                ticketId, 
                flightDate, 
                airline.ApiUrl ?? "http://localhost:5241/api/airline"
            );

            if (!validationResult.IsValid)
            {
                return (false, $"Validación de aerolínea fallida: {validationResult.Message}", null);
            }

            // 6. Verificar que no haya embarque duplicado
            var existingBoarding = _context.Boardings
                .FirstOrDefault(b => b.TicketId == ticketId && b.SlotId == slotId);
            
            if (existingBoarding != null)
            {
                return (false, "Este ticket ya fue registrado para embarque", null);
            }

            // 7. Registrar embarque
            var boarding = new Boarding
            {
                TicketId = ticketId,
                SlotId = slotId,
                AccessTime = DateTime.UtcNow,
                Validation = true
            };

            _context.Boardings.Add(boarding);

            // 8. Actualizar estado del ticket
            ticket.Status = "Embarcado";
            _context.SaveChanges();

            return (true, "Embarque registrado exitosamente", boarding);
        }
    }
}
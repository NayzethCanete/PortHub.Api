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
        private readonly IAirlineIntegrationService _airlineIntegrationService; 

        public BoardingService(AppDbContext context, IAirlineIntegrationService airlineIntegrationService)
        {
            _context = context;
            _airlineIntegrationService = airlineIntegrationService;
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

            existing.TicketNumber = boarding.TicketNumber; 
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

        public async Task<(Boarding? boarding, bool success, string message)> ValidateAndRegisterBoardingAsync(
            string ticketNumber, 
            int slotId)
        {
            var slot = _context.Slots
                .Include(s => s.Gate) 
                .FirstOrDefault(s => s.Id == slotId);
            
            if (slot == null)
            {
                return (null, false, "Slot no encontrado");
            }

            if (slot.Status != "Confirmado")
            {
                return (null, false, $"El slot no está confirmado. Estado actual: {slot.Status}");
            }
            
            if (string.IsNullOrEmpty(slot.FlightCode) || slot.FlightCode.Length < 2)
            {
                 return (null, false, "El Slot no tiene un código de vuelo válido para identificar la aerolínea.");
            }

            var airlineCode = slot.FlightCode.Substring(0, 2);
            var airline = _context.Airlines.FirstOrDefault(a => a.Code == airlineCode);
            
            if (airline == null)
            {
                return (null, false, $"No se pudo identificar la aerolínea para el código de vuelo {slot.FlightCode}");
            }
            
            var boardingAttempt = new Boarding
            {
                TicketNumber = ticketNumber,
                SlotId = slotId,
                AccessTime = DateTime.UtcNow,
                Validation = false 
            };
            _context.Boardings.Add(boardingAttempt);
            await _context.SaveChangesAsync();
            
            
            if (!int.TryParse(ticketNumber, out int parsedTicketId))
            {
                boardingAttempt.Validation = false;
                await _context.SaveChangesAsync();
                return (boardingAttempt, false, "Formato de ticket inválido para la validación de la aerolínea.");
            }

            var flightDate = slot.ScheduleTime.ToString("yyyy-MM-dd");
            var validationResult = await _airlineIntegrationService.ValidateTicketWithAirlineAsync(
                parsedTicketId, 
                flightDate, 
                airline.ApiUrl!
            );
            
            if (!validationResult.IsValid)
            {
                return (boardingAttempt, false, $"Validación de aerolínea fallida: {validationResult.Message}");
            }
            
            boardingAttempt.Validation = true;
            await _context.SaveChangesAsync();

            return (boardingAttempt, true, "Embarque registrado exitosamente");
        }
    }
}
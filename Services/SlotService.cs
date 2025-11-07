using PortHub.Api.Models;
using PortHub.Api.Interfaces;
using PortHub.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace PortHub.Api.Services
{
    public class SlotService : ISlotService
    {
        private readonly AppDbContext _context;
        private readonly SlotReservationOptions _reservationOptions;
        private readonly ILogger<SlotService> _logger;

        public SlotService(
            AppDbContext context, 
            IOptions<SlotReservationOptions> reservationOptions,
            ILogger<SlotService> logger)
        {
            _context = context;
            _reservationOptions = reservationOptions.Value;
            _logger = logger;
        }

        public IEnumerable<Slot> GetAll()
        {
            // Limpiar expirados antes de devolver la lista
            CleanupExpiredReservations();
            return _context.Slots.ToList();
        }

        public Slot? GetById(int id)
        {
            var slot = _context.Slots.FirstOrDefault(s => s.Id == id);
            
            // Si el slot está reservado y expiró, liberarlo
            if (slot != null && IsReservationExpired(slot))
            {
                slot.Status = "Libre";
                slot.ReservationExpiresAt = null;
                _context.SaveChanges();
                
                _logger.LogInformation(
                    "Slot {SlotId} liberado por timeout al consultarlo",
                    slot.Id
                );
            }
            
            return slot;
        }

        /* * El método Add(Slot slot) se elimina según nuestra conversación anterior 
         * para usar ReserveSlot como el único método de creación.
         */

        public Slot? Update(Slot slot, int id)
        {
            var existing = _context.Slots.FirstOrDefault(s => s.Id == id);
            if (existing == null)
                return null;

            existing.ScheduleTime = slot.ScheduleTime;
            existing.Runway = slot.Runway;
            existing.GateId = slot.GateId;
            existing.FlightCode = slot.FlightCode;
            existing.Status = slot.Status ?? existing.Status;

            _context.SaveChanges();
            return existing;
        }

        public bool Delete(int id)
        {
            var slot = _context.Slots.FirstOrDefault(s => s.Id == id);
            if (slot == null) return false;

            _context.Slots.Remove(slot);
            _context.SaveChanges();
            return true;
        }

        /**
         * Lógica de Reserva actualizada:
         * 1. Busca si ya existe un slot para esa hora/pista.
         * 2. Si existe:
         * - Si está "Reservado" o "Confirmado", lanza error de conflicto.
         * - Si está "Libre", lo REUTILIZA y actualiza a "Reservado".
         * 3. Si no existe, CREA uno nuevo y lo pone como "Reservado".
         */
        public Slot ReserveSlot(Slot slot)
        {
            // 1. Buscar si ya existe CUALQUIER slot para esta combinación
            var existingSlot = _context.Slots.FirstOrDefault(s => 
                s.ScheduleTime == slot.ScheduleTime && 
                s.Runway == slot.Runway);

            if (existingSlot != null)
            {
                // 2. Si existe, revisar su estado
                if (existingSlot.Status == "Confirmado" || existingSlot.Status == "Reservado")
                {
                    // Conflicto real: Ya está ocupado
                    throw new InvalidOperationException(
                        "Ya existe un slot reservado o confirmado para ese horario y pista"
                    );
                }

                // 3. ¡El slot existe pero está "Libre"! Lo reutilizamos.
                existingSlot.Status = "Reservado";
                existingSlot.ReservationExpiresAt = DateTime.UtcNow.AddMinutes(_reservationOptions.TimeoutMinutes);
                existingSlot.GateId = slot.GateId; // Actualizar datos
                existingSlot.FlightCode = slot.FlightCode; // Actualizar datos
                
                _context.Slots.Update(existingSlot); // Usar Update, NO Add
                _context.SaveChanges();

                _logger.LogInformation(
                    "Slot {SlotId} REUTILIZADO y reservado. Expira en {Minutes} minutos ({ExpiryTime})",
                    existingSlot.Id,
                    _reservationOptions.TimeoutMinutes,
                    existingSlot.ReservationExpiresAt
                );
                
                return existingSlot;
            }
            else
            {
                // 4. El slot no existe en absoluto. Es uno nuevo.
                slot.Status = "Reservado";
                slot.ReservationExpiresAt = DateTime.UtcNow.AddMinutes(_reservationOptions.TimeoutMinutes);
                
                _context.Slots.Add(slot); // Usar Add
                _context.SaveChanges();

                _logger.LogInformation(
                    "Slot {SlotId} CREADO y reservado. Expira en {Minutes} minutos ({ExpiryTime})",
                    slot.Id,
                    _reservationOptions.TimeoutMinutes,
                    slot.ReservationExpiresAt
                );

                return slot;
            }
        }

        public Slot ConfirmSlot(int id)
        {
            var slot = _context.Slots.FirstOrDefault(s => s.Id == id)
                ?? throw new KeyNotFoundException($"Slot {id} no encontrado");

            // Verificar si expiró
            if (IsReservationExpired(slot))
            {
                slot.Status = "Libre";
                slot.ReservationExpiresAt = null;
                _context.SaveChanges();
                
                throw new InvalidOperationException(
                    $"El slot {id} expiró. Ya fue liberado automáticamente"
                );
            }

            if (slot.Status != "Reservado")
            {
                throw new InvalidOperationException(
                    $"Solo se pueden confirmar slots en estado 'Reservado'. Estado actual: {slot.Status}"
                );
            }

            slot.Status = "Confirmado";
            slot.ReservationExpiresAt = null; // Eliminar timeout
            
            _context.SaveChanges();

            _logger.LogInformation(
                "Slot {SlotId} confirmado exitosamente",
                slot.Id
            );

            return slot;
        }

        public Slot CancelSlot(int id)
        {
            var slot = _context.Slots.FirstOrDefault(s => s.Id == id)
                ?? throw new KeyNotFoundException($"Slot {id} no encontrado");

            slot.Status = "Libre";
            slot.ReservationExpiresAt = null;
            
            _context.SaveChanges();

            _logger.LogInformation(
                "Slot {SlotId} cancelado/liberado",
                slot.Id
            );

            return slot;
        }

        private bool IsReservationExpired(Slot slot)
        {
            return slot.Status == "Reservado" &&
                   slot.ReservationExpiresAt.HasValue &&
                   slot.ReservationExpiresAt.Value < DateTime.UtcNow;
        }

        private void CleanupExpiredReservations()
        {
            var now = DateTime.UtcNow;
            var expiredSlots = _context.Slots
                .Where(s => 
                    s.Status == "Reservado" && 
                    s.ReservationExpiresAt != null && 
                    s.ReservationExpiresAt < now
                )
                .ToList();

            if (expiredSlots.Any())
            {
                foreach (var slot in expiredSlots)
                {
                    slot.Status = "Libre";
                    slot.ReservationExpiresAt = null;
                }
                
                _context.SaveChanges();
                
                _logger.LogInformation(
                    "Limpiados {Count} slots expirados",
                    expiredSlots.Count
                );
            }
        }
    }
}
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

        // Lista de pistas válidas permitidas en el aeropuerto
        private static readonly HashSet<string> _validRunways = new(StringComparer.OrdinalIgnoreCase)
        {
            "Pista 1",
            "Pista 2",
            "Pista 3"
          
        };

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
            CleanupExpiredReservations();
            return _context.Slots.ToList();
        }

        public Slot? GetById(int id)
        {
            var slot = _context.Slots.FirstOrDefault(s => s.Id == id);

            if (slot != null && IsReservationExpired(slot))
            {
                slot.Status = "Libre";
                slot.ReservationExpiresAt = null;
                _context.SaveChanges();

                _logger.LogInformation("Slot {SlotId} liberado por timeout al consultarlo", slot.Id);
            }

            return slot;
        }

        public Slot? Update(Slot slot, int id)
        {
            var existing = _context.Slots.FirstOrDefault(s => s.Id == id);
            if (existing == null) return null;

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

        public Slot ReserveSlot(Slot slot)
        {
            // VALIDACIÓN DE PISTA
            if (!_validRunways.Contains(slot.Runway))
            {
                throw new ArgumentException($"La pista '{slot.Runway}' no es válida. Pistas permitidas: {string.Join(", ", _validRunways)}");
            }

            // ASIGNACIÓN AUTOMÁTICA DE GATE (si no se especifica)
            if (slot.GateId == null || slot.GateId == 0)
            {
                var autoGateId = FindAvailableGateId(slot.ScheduleTime);
                if (autoGateId == null)
                {
                    throw new InvalidOperationException("No hay gates disponibles automáticamente para este horario (+/- 60min).");
                }
                slot.GateId = autoGateId;
                _logger.LogInformation("Gate {GateId} asignada automáticamente al slot solicitado para {Time}", autoGateId, slot.ScheduleTime);
            }

            // 3. LÓGICA DE RESERVA (verificar duplicados)
            var existingSlot = _context.Slots.FirstOrDefault(s =>
                s.ScheduleTime == slot.ScheduleTime &&
                s.Runway == slot.Runway);

            if (existingSlot != null)
            {
                if (existingSlot.Status == "Confirmado" || existingSlot.Status == "Reservado")
                {
                    throw new InvalidOperationException("Ya existe un slot reservado o confirmado para ese horario y pista");
                }

                // Reutilizar slot Libre
                existingSlot.Status = "Reservado";
                existingSlot.ReservationExpiresAt = DateTime.UtcNow.AddMinutes(_reservationOptions.TimeoutMinutes);
                existingSlot.GateId = slot.GateId;
                existingSlot.FlightCode = slot.FlightCode;

                _context.Slots.Update(existingSlot);
                _context.SaveChanges();

                _logger.LogInformation("Slot {SlotId} REUTILIZADO y reservado con Gate {GateId}.", existingSlot.Id, existingSlot.GateId);
                return existingSlot;
            }
            else
            {
                // Crear nuevo slot
                slot.Status = "Reservado";
                slot.ReservationExpiresAt = DateTime.UtcNow.AddMinutes(_reservationOptions.TimeoutMinutes);

                _context.Slots.Add(slot);
                _context.SaveChanges();

                _logger.LogInformation("Slot {SlotId} CREADO y reservado con Gate {GateId}.", slot.Id, slot.GateId);
                return slot;
            }
        }

        public Slot ConfirmSlot(int id)
        {
            var slot = _context.Slots.FirstOrDefault(s => s.Id == id)
                ?? throw new KeyNotFoundException($"Slot {id} no encontrado");

            if (IsReservationExpired(slot))
            {
                slot.Status = "Libre";
                slot.ReservationExpiresAt = null;
                _context.SaveChanges();
                throw new InvalidOperationException($"El slot {id} expiró. Ya fue liberado automáticamente");
            }

            if (slot.Status != "Reservado")
            {
                throw new InvalidOperationException($"Solo se pueden confirmar slots en estado 'Reservado'. Estado actual: {slot.Status}");
            }

            slot.Status = "Confirmado";
            slot.ReservationExpiresAt = null;

            _context.SaveChanges();
            _logger.LogInformation("Slot {SlotId} confirmado exitosamente", slot.Id);

            return slot;
        }

        public Slot CancelSlot(int id)
        {
            var slot = _context.Slots.FirstOrDefault(s => s.Id == id)
                ?? throw new KeyNotFoundException($"Slot {id} no encontrado");

            slot.Status = "Libre";
            slot.ReservationExpiresAt = null;

            _context.SaveChanges();
            _logger.LogInformation("Slot {SlotId} cancelado/liberado", slot.Id);

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
                .Where(s => s.Status == "Reservado" && s.ReservationExpiresAt != null && s.ReservationExpiresAt < now)
                .ToList();

            if (expiredSlots.Any())
            {
                foreach (var slot in expiredSlots)
                {
                    slot.Status = "Libre";
                    slot.ReservationExpiresAt = null;
                }
                _context.SaveChanges();
                _logger.LogInformation("Limpiados {Count} slots expirados", expiredSlots.Count);
            }
        }

        private int? FindAvailableGateId(DateTime scheduleTime)
        {
            var buffer = TimeSpan.FromMinutes(60);
            var start = scheduleTime.Subtract(buffer);
            var end = scheduleTime.Add(buffer);

            var occupiedGateIds = _context.Slots
                .Where(s => (s.Status == "Reservado" || s.Status == "Confirmado")
                            && s.GateId != null
                            && s.ScheduleTime >= start && s.ScheduleTime <= end)
                .Select(s => s.GateId!.Value)
                .Distinct()
                .ToList();

            var availableGate = _context.Gates
                .Where(g => !occupiedGateIds.Contains(g.Id))
                .FirstOrDefault();

            return availableGate?.Id;
        }
    }
}
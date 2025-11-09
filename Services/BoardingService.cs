using Microsoft.EntityFrameworkCore;
using PortHub.Api.Data;
using PortHub.Api.Dtos;
using PortHub.Api.Interfaces;
using PortHub.Api.Models;

namespace PortHub.Api.Services
{
    public class BoardingService : IBoardingService
    {
        private readonly AppDbContext _context;
        private readonly IAirlineIntegrationService _airlineService;

        public BoardingService(AppDbContext context, IAirlineIntegrationService airlineService)
        {
            _context = context;
            _airlineService = airlineService;
        }

        public async Task<BoardingRegistrationResponse> RegisterBoardingAsync(BoardingRegistrationRequest request)
        {
            try
            {
                // 1. Buscar el SLOT ACTIVO para el FlightCode indicado.
                // Filtramos por:
                // - FlightCode: El vuelo que indica el pasajero.
                // - Status "Confirmado": Solo se puede abordar vuelos confirmados.
                // - Fecha de HOY: Para evitar abordar por error un vuelo de mañana con el mismo código.
                var activeSlot = await _context.Slots
                    .Include(s => s.Gate) // Incluimos el Gate para saber su nombre después
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s =>
                        s.FlightCode == request.FlightCode &&
                        s.Status == "Confirmado" &&
                        s.ScheduleTime.Date == DateTime.UtcNow.Date);

                // Validaciones del Slot
                if (activeSlot == null)
                {
                    return new BoardingRegistrationResponse(
                        false,
                        $"No hay un vuelo confirmado con código {request.FlightCode} programado para hoy.",
                        DateTime.UtcNow,
                        "N/A",
                        request.FlightCode
                    );
                }

                if (activeSlot.GateId == null || activeSlot.Gate == null)
                {
                    // Esto no debería pasar si el slot está "Confirmado", pero por seguridad lo validamos.
                    return new BoardingRegistrationResponse(
                        false,
                        $"El vuelo {request.FlightCode} está confirmado pero AÚN NO TIENE PUERTA asignada.",
                        DateTime.UtcNow,
                        "SIN ASIGNAR",
                        request.FlightCode
                    );
                }

                // 2. Validar Ticket con la API de la Aerolínea Externa
                var validationRequest = new TicketValidationRequest(request.TicketNumber, request.FlightCode);
                var validationResponse = await _airlineService.ValidateTicketAsync(validationRequest);

                if (!validationResponse.IsValid)
                {
                    return new BoardingRegistrationResponse(
                        false,
                        $"Aerolínea rechazó el embarque: {validationResponse.Message}",
                        DateTime.UtcNow,
                        activeSlot.Gate.Name, // Ya sabemos la puerta correcta gracias al slot
                        request.FlightCode
                    );
                }

                // 3. Validar que NO se haya embarcado previamente (Opcional pero recomendado)
                bool alreadyBoarded = await _context.Boardings.AnyAsync(b =>
                    b.TicketNumber == request.TicketNumber &&
                    b.SlotId == activeSlot.Id);

                if (alreadyBoarded)
                {
                     return new BoardingRegistrationResponse(
                        false,
                        $"Este ticket ya fue embarcado anteriormente para este vuelo.",
                        DateTime.UtcNow,
                        activeSlot.Gate.Name,
                        request.FlightCode
                    );
                }

                // 4. Registrar el Embarque
                // Usamos los datos que obtuvimos AUTOMÁTICAMENTE del slot activo.
                var boarding = new Boarding
                {
                    SlotId = activeSlot.Id,
                    GateId = activeSlot.GateId.Value, // ¡Aquí está la magia! Usamos el ID del Gate del slot.
                    TicketNumber = request.TicketNumber,
                    FlightCode = request.FlightCode,
                    BoardingTime = DateTime.UtcNow,
                    Status = "Embarcado",
                    PassengerName = "Pasajero Verificado",
                    Seat = "N/A"
                };

                _context.Boardings.Add(boarding);
                await _context.SaveChangesAsync();

                return new BoardingRegistrationResponse(
                    true,
                    "Embarque registrado exitosamente.",
                    boarding.BoardingTime,
                    activeSlot.Gate.Name, // Devolvemos el nombre de la puerta que el sistema asignó
                    request.FlightCode
                );
            }
            catch (Exception ex)
            {
                return new BoardingRegistrationResponse(
                    false,
                    $"Error interno: {ex.Message}",
                    DateTime.UtcNow,
                    "ERROR",
                    request.FlightCode
                );
            }
        }

        // ... (Resto del método GetAllBoardingsAsync igual)
        public async Task<IEnumerable<ResponseBoardingDto>> GetAllBoardingsAsync()
        {
            var boardings = await _context.Boardings.ToListAsync();
            return boardings.Select(b => new ResponseBoardingDto(
                b.BoardingId,
                b.FlightCode,
                b.PassengerName,
                b.Seat,
                b.Status
            ));
        }
    }
}
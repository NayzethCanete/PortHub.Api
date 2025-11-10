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
                // Buscar el SLOT ACTIVO para el FlightCode indicado.
                var activeSlot = await _context.Slots
                    .Include(s => s.Gate) // Incluimos el Gate para acceder a su nombre luego
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s =>
                        s.FlightCode == request.FlightCode &&
                        s.Status == "Confirmado" &&
                        s.ScheduleTime.Date == DateTime.UtcNow.Date);

                // --- Validaciones del Slot ---
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
                    return new BoardingRegistrationResponse(
                        false,
                        $"El vuelo {request.FlightCode} está confirmado pero AÚN NO TIENE PUERTA asignada.",
                        DateTime.UtcNow,
                        "SIN ASIGNAR",
                        request.FlightCode
                    );
                }

                // --- Validar Ticket con la API de la Aerolínea ---
                var validationRequest = new TicketValidationRequest(
                    request.TicketNumber,
                    request.FlightCode
                );

                var validationResponse = await _airlineService.ValidateTicketAsync(validationRequest);

                if (!validationResponse.IsValid)
                {
                    return new BoardingRegistrationResponse(
                        false,
                        $"Aerolínea rechazó el embarque: {validationResponse.Message}",
                        DateTime.UtcNow,
                        activeSlot.Gate.Name,
                        request.FlightCode
                    );
                }

                // --- Verificar que el ticket no haya embarcado ya ---
                bool alreadyBoarded = await _context.Boardings.AnyAsync(b =>
                    b.TicketNumber == request.TicketNumber.ToString() &&
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

                // --- Registrar el embarque ---
                var boarding = new Boarding
                {
                    SlotId = activeSlot.Id,
                    GateId = activeSlot.GateId.Value,
                    TicketNumber = request.TicketNumber.ToString(),
                    FlightCode = request.FlightCode,
                    BoardingTime = DateTime.UtcNow,
                    Status = "Embarcado",
                    PassengerName = "Pasajero Verificado", // podría venir de la API externa
                    Seat = "N/A"
                };

                _context.Boardings.Add(boarding);
                await _context.SaveChangesAsync();

                // --- Respuesta final exitosa ---
                return new BoardingRegistrationResponse(
                    true,
                    "Embarque registrado exitosamente.",
                    boarding.BoardingTime,
                    activeSlot.Gate.Name,
                    request.FlightCode
                );
            }
            catch (Exception ex)
            {
                // Manejo de errores controlado
                return new BoardingRegistrationResponse(
                    false,
                    $"Error interno: {ex.Message}",
                    DateTime.UtcNow,
                    "ERROR",
                    request.FlightCode
                );
            }
        }

        // --- Listado de embarques ---
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

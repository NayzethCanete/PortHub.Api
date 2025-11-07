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
                var gate = await _context.Gates
                    .FirstOrDefaultAsync(g => g.Id == request.GateId);

                if (gate == null)
                {
                    return new BoardingRegistrationResponse(false, "Gate no encontrado", DateTime.UtcNow, "", request.FlightCode);
                }

                var activeSlot = await _context.Slots
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => 
                        s.FlightCode == request.FlightCode && 
                        s.GateId == request.GateId &&
                        s.Status == "Confirmado"); 
                if (activeSlot == null)
                {
                    return new BoardingRegistrationResponse(
                        false,
                        $"No hay un slot confirmado para el vuelo {request.FlightCode} en el gate {gate.Name}.",
                        DateTime.UtcNow,
                        gate.Name,
                        request.FlightCode
                    );
                }

                var validationRequest = new TicketValidationRequest(
                    request.TicketNumber,
                    request.FlightCode,
                    DateTime.UtcNow
                );

                var validationResponse = await _airlineService.ValidateTicketAsync(validationRequest);

                if (!validationResponse.IsValid)
                {
                    return new BoardingRegistrationResponse(
                        false,
                        $"Aerolínea rechazó el ticket: {validationResponse.Message}",
                        DateTime.UtcNow,
                        gate.Name,
                        request.FlightCode
                    );
                }

                var boarding = new Boarding
                {
                    SlotId = activeSlot.Id, // <--- ¡CORRECCIÓN IMPORTANTE AQUÍ!
                    
                    TicketNumber = request.TicketNumber,
                    PassengerName = validationResponse.Details?.PassengerName ?? "Pasajero Verificado",
                    Seat = validationResponse.Details?.Seat ?? "SIN ASIGNAR",
                    Status = "Embarcado",
                    BoardingTime = DateTime.UtcNow,
                    GateId = gate.Id,
                    FlightCode = request.FlightCode // Si decidiste agregar FlightCode directo a Boarding para facilitar lecturas
                };

                _context.Boardings.Add(boarding);
                await _context.SaveChangesAsync();

                return new BoardingRegistrationResponse(
                    true,
                    "Embarque registrado exitosamente",
                    boarding.BoardingTime,
                    gate.Name,
                    request.FlightCode
                );
            }
            catch (Exception ex)
            {
                return new BoardingRegistrationResponse(
                    false,
                    $"Error interno procesando embarque: {ex.Message}",
                    DateTime.UtcNow,
                    "",
                    request.FlightCode
                );
            }
        }

        public async Task<IEnumerable<ResponseBoardingDto>> GetAllBoardingsAsync()
        {
            var boardings = await _context.Boardings.ToListAsync();
            return boardings.Select(b => new ResponseBoardingDto(
                b.BoardingId,
                0, 
                b.PassengerName,
                b.Seat,
                b.Status
            ));
        }

    }
}
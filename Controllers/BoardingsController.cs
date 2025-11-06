using Microsoft.AspNetCore.Mvc;
using PortHub.Api.Interfaces;
using PortHub.Api.Models;
using PortHub.Api.Dtos;
using PortHub.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace PortHub.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BoardingsController : ControllerBase
    {
        private readonly IBoardingService _boardingService;
        private readonly AppDbContext _context;

        public BoardingsController(IBoardingService boardingService, AppDbContext context)
        {
            _boardingService = boardingService;
            _context = context;
        }

        private static BoardingResponseDto ToDto(Boarding b)
        {
            long gateIdFromSlot = b.Slot?.GateId ?? 0;

            return new BoardingResponseDto(
                b.BoardingId,
                b.TicketId.ToString(),
                gateIdFromSlot,
                b.AccessTime,
                b.Validation
            );
        }

        /// <summary>
        /// Obtener todos los registros de embarque
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BoardingResponseDto>), 200)]
        public IActionResult GetAll()
        {
            var boardings = _context.Boardings
                .Include(b => b.Slot)
                .ToList();

            return Ok(boardings.Select(ToDto));
        }

        /// <summary>
        /// Obtener un embarque por ID
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(BoardingResponseDto), 200)]
        [ProducesResponseType(404)]
        public IActionResult GetById(int id)
        {
            var boarding = _context.Boardings
                .Include(b => b.Slot)
                .FirstOrDefault(b => b.BoardingId == id);

            if (boarding == null)
                return NotFound(new { code = "NOT_FOUND", message = "Registro de embarque no encontrado" });

            return Ok(ToDto(boarding));
        }

        /// <summary>
        /// ENDPOINT CRÍTICO: Registrar embarque con validación de aerolínea
        /// </summary>
        [HttpPost("validate")]
        [ProducesResponseType(typeof(BoardingResponseDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ValidateAndRegister([FromBody] BoardingValidateRequestDto dto)
        {
            var result = await _boardingService.ValidateAndRegisterBoardingAsync(dto.TicketId, dto.SlotId);

            if (!result.success)
            {
                return BadRequest(new
                {
                    code = "VALIDATION_FAILED",
                    message = result.message
                });
            }

            var boardingWithSlot = _context.Boardings
                .Include(b => b.Slot)
                .FirstOrDefault(b => b.BoardingId == result.boarding!.BoardingId);

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.boarding!.BoardingId },
                ToDto(boardingWithSlot ?? result.boarding)
            );
        }

        /// <summary>
        /// Registrar embarque manualmente (sin validación - solo para testing)
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(BoardingResponseDto), 201)]
        [ProducesResponseType(400)]
        public IActionResult Add([FromBody] BoardingRequestDto dto)
        {
            if (!int.TryParse(dto.TicketNumber, out int ticketId))
            {
                return BadRequest(new
                {
                    code = "VALIDATION_ERROR",
                    message = "El TicketNumber debe ser un número entero."
                });
            }

            var boarding = new Boarding
            {
                TicketId = ticketId,
                SlotId = dto.SlotId,
                AccessTime = DateTime.UtcNow,
                Validation = false
            };

            var created = _boardingService.Add(boarding);

            var createdWithSlot = _context.Boardings
                .Include(b => b.Slot)
                .FirstOrDefault(b => b.BoardingId == created.BoardingId);

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.BoardingId },
                ToDto(createdWithSlot ?? created)
            );
        }
    }
}
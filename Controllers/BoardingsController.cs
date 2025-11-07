using Microsoft.AspNetCore.Mvc;
using PortHub.Api.Interfaces;
using PortHub.Api.Models;
using PortHub.Api.Dtos;
using PortHub.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization; 

namespace PortHub.Api.Controllers
{
    [Authorize] 
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
                b.TicketNumber,
                gateIdFromSlot,
                b.AccessTime,
                b.Validation
            );
        }

       
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BoardingResponseDto>), 200)]
        public IActionResult GetAll()
        {
            var boardings = _context.Boardings
                .Include(b => b.Slot)
                .ToList();

            return Ok(boardings.Select(ToDto));
        }

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

      
        [HttpPost("validate")]
        [AllowAnonymous] 
        [ProducesResponseType(typeof(BoardingResponseDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ValidateAndRegister([FromBody] BoardingValidateRequestDto dto)
        {
            
            var result = await _boardingService.ValidateAndRegisterBoardingAsync(dto.TicketNumber, dto.SlotId);

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

     
        [HttpPost]
        [ProducesResponseType(typeof(BoardingResponseDto), 201)]
        [ProducesResponseType(400)]
        public IActionResult Add([FromBody] BoardingRequestDto dto)
        {
            
            if (string.IsNullOrEmpty(dto.TicketNumber))
            {
                return BadRequest(new
                {
                    code = "VALIDATION_ERROR",
                    message = "El TicketNumber no puede estar vacío."
                });
            }

            var boarding = new Boarding
            {
                
                TicketNumber = dto.TicketNumber,
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
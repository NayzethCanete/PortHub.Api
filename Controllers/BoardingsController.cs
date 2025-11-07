using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PortHub.Api.Interfaces;
using PortHub.Api.Dtos;
using PortHub.Api.Common;
using System;
using System.Threading.Tasks;

namespace PortHub.Api.Controllers
{




    [ApiController]
    [Route("api/[controller]")]
    public class BoardingsController : ControllerBase
    {
        private readonly IBoardingService _boardingService;

        public BoardingsController(IBoardingService boardingService)
        {
            _boardingService = boardingService;
        }

        [HttpPost("register")]
        [Authorize]
        public async Task<ActionResult<BoardingRegistrationResponse>> RegisterBoarding(
            [FromBody] BoardingRegistrationRequest request)
        {
            try
            {
                var result = await _boardingService.RegisterBoardingAsync(request);

                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse("Error interno registrando embarque", ex.Message));
            }
        }
    }
}
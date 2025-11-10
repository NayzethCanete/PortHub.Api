using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PortHub.Api.Interfaces;
using PortHub.Api.Dtos;
using PortHub.Api.Common;
using System;
using System.Threading.Tasks;


/*

Controlador para manejar operaciones relacionadas con embarques.
Valida pasajeros que pasan por las puertas de embarque y registra los embarques.
*/
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

        //Requiere el numero de ticket y el vuelo para validar el embarque

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
                    return BadRequest(result); // 400 Bad Request con el mensaje de error
                }

                return Ok(result);
            }

            catch (Exception ex)
            {
                //Error inesperado (conexion a API de aerolinea, BD, etc)
                return StatusCode(500, new ErrorResponse("Error interno registrando embarque", ex.Message));
            }
        }
    }
}
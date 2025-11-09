using Microsoft.AspNetCore.Mvc;
using PortHub.Api.Interfaces;
using PortHub.Api.Models;
using PortHub.Api.Dtos;
using Microsoft.AspNetCore.Authorization;


/*
Controlador que maneja la autenticación de usuarios.
Proporciona un endpoint para iniciar sesión y obtener un token JWT.
*/


namespace PortHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }


/*
Es el login del personal del aeropuerto 
Genera un token JWT si las credenciales son correctas, solo valido por 60 min 
*/
    [AllowAnonymous] // Sin autenticacion, pues es endpoint para obtener token
    [HttpPost("login")]
    public ActionResult<string> Login([FromBody] LoginDto loginDto)
    {
        //Valida username y password  hasheada con BCrypt   
        var token = _authService.Login(loginDto);
        if (token == null) return Unauthorized("Usuario o contraseña incorrectos");
        return Ok(token); //devuelve el token JWT
    }
}
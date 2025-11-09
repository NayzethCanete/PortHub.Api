using PortHub.Api.Interfaces;
using PortHub.Api.Models;
using Microsoft.AspNetCore.Mvc;
using PortHub.Api.Dtos;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;



/*
Controlador de usuarios
Permite obtener la lista de usuarios y registrar nuevos usuarios.
*/



namespace PortHub.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [AllowAnonymous] //Permite el registro sin autenticacion
    [HttpGet]
    public ActionResult<List<UserDto>> GetAll()
    {
        var users = _userService.GetAll();
        //NUNCA devolver el hash de la contraseña
        var usersDto = users.Select(u => new UserDto(u.Username)).ToList();
        return Ok(usersDto);
    }

    [AllowAnonymous]
    [HttpPost]
    public ActionResult<UserDto> Registrar([FromBody] RegistrarUserDto userDto)
    {

        //Hasheo de la contraseña con BCrypt
        var passHash = BCrypt.Net.BCrypt.HashPassword(userDto.PasswordHash);
        var user = new User
        {
            Username = userDto.Username,
            PasswordHash = passHash
        };
        user = _userService.AddUser(user);
        return CreatedAtAction(nameof(GetAll), new UserDto(user.Username));
    }
}
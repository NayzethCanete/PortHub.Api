using PortHub.Api.Interfaces;
using PortHub.Api.Models;
using Microsoft.AspNetCore.Mvc;
using PortHub.Api.Dto;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;

namespace PortHub.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService _userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public ActionResult<List<UserDto>> GetAll()
    {
        var users = _userService.GetAll();
        var usersDto = users.Select(u => new UserDto(u.Username)).ToList();
        return Ok(usersDto);
    }

    [AllowAnonymous]
    [HttpPost]
    public ActionResult<UserDto> Registrar([FromBody] RegistrarUserDto userDto)
    {
        var passHash = BCrypt.Net.BCrypt.HashPassword(userDto.PasswordHash);
        var user = new User
        {
            Username = userDto.Username,
            PasswordHash = passHash
        };
        user = _userService.Add(persona);
        return CreatedAtAction(nameof(GetAll), new UserDto(user.Username));
    }
}
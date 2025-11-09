
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PortHub.Api.Interfaces;
using PortHub.Api.Models;
using PortHub.Api.Data;
using PortHub.Api.Dtos;

namespace PortHub.Api.Services;

public class AuthService : IAuthService
{
    private readonly AuthOptions _options;
    private readonly IUserService _userService;
    public AuthService(IOptions<AuthOptions> options, IUserService userService)
    {
        _options = options.Value;
        _userService = userService;
    }

    public string CreateToken(CreateTokenDto createTokenDto)
    {
        var claims = new List<Claim>
        {
            // Id del usuario
            new(JwtRegisteredClaimNames.Sub, createTokenDto.Id.ToString()),
            // Nombre de login del usuario
            new(JwtRegisteredClaimNames.UniqueName, createTokenDto.Username)
        };


        // Se toma la "Key" del archivo de configuración, se pasa a bytes y se crea una SecurityKey.
        // "SymmetricSecurityKey" indica que se usará la misma clave para firmar y validar (HS256).
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));

       
        // Definen cómo se firmará el token (algoritmo y clave usada).
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // --- Establecer tiempos de validez ---
        var expires = DateTime.UtcNow.AddMinutes(_options.ExpMinutes); // Fecha de expiración
        var notBefore = DateTime.UtcNow;

     
        // Se define el token con los datos necesarios.
        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: expires,
            notBefore: notBefore,
            signingCredentials: creds
        );

        // Se convierte el token a formato JWT y se devuelve como string.
        return new JwtSecurityTokenHandler().WriteToken(token);

    }

    public string Login(LoginDto loginDto){
        var user = _userService.GetByUsername(loginDto.Username);

        // Verificar si la contraseña coincide con el hash almacenado
        if (user != null && BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
        {
            return CreateToken(new CreateTokenDto(user.Username, user.Id));
        }
        // Si la contraseña no coincide o el usuario no existe se retorna null
        return null;
    }
}
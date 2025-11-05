using System.ComponentModel.DataAnnotations;

namespace PortHub.Api.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; } // Almacena contraseñas encriptadas

        [Required]
        public string Role { get; set; } // Ejemplo: "Admin", "User"
    }
}
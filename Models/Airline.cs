using System.ComponentModel.DataAnnotations;
namespace PortHub.Api.Models
{
    public class Airline
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public string Code { get; set; } = string.Empty;
        
        public string? Country { get; set; }
        public string? BaseAddress { get; set; }
        public string? ApiUrl { get; set; }
        public string? ApiKey { get; set; }
        public virtual ICollection<Flight> Flights { get; set; } = new List<Flight>();
    }
}
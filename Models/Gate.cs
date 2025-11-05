using System.ComponentModel.DataAnnotations;
namespace PortHub.Api.Models
{
    public class Gate
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Location { get; set; }
        public bool IsAvailable { get; set; } = true;
        // Relación: Un gate tiene muchos slots
        public virtual ICollection<Slot> Slots { get; set; } = new List<Slot>();
    }
}
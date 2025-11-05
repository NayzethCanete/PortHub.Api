
namespace PortHub.Api.Models;
using System; 
namespace PortHub.Api.Models;
public class Gate
{
    [Required]
    public int Id { get; set; }

    [Required, MaxLength(20)]
    public string Name { get; set; } 
    
    [MaxLength(50)]
    public int GateId { get; set; }
    public string GateName { get; set; } 
    public string Location { get; set; }


}

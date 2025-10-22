using System.ComponentModel.DataAnnotations;

namespace PortHubApi.Models;
public class Gate
{
    public int GateId { get; set; }

    [Required, MaxLength(20)]
    public string GateName { get; set; } 
    
    [MaxLength(50)]
    public string Location { get; set; }


}

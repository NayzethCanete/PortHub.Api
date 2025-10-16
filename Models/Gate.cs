using System.ComponentModel.DataAnnotations;

namespace PortHubApi.Models;
public class Gate
{
    public int Id { get; set; }


    [Required, MaxLength(20)]
    public string Name { get; set; } 
    
    [MaxLength(50)]
    public string Location { get; set; }


}

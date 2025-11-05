<<<<<<< HEAD
using System; 
=======
>>>>>>> BD-setup
namespace PortHub.Api.Models;

public class Gate
{
<<<<<<< HEAD
    public int Id { get; set; }
    public string Name { get; set; } 
    public int GateId { get; set; }
    public string GateName { get; set; } 
    public string Location { get; set; }


}
=======
    public int GateId { get; set; }  
    public string GateName { get; set; }  
    public string Location { get; set; }  
    public bool Available { get; set; } = true; 
    
    public ICollection<Slot> Slots { get; set; } = new List<Slot>();
}
>>>>>>> BD-setup

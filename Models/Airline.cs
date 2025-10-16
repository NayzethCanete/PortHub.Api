using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PortHubApi.Models;
public class Airline
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; }

    public int Code { get; set; }

    [MaxLength(50)]
    public string Country { get; set; }

    [MaxLength(200)]
    public string Address { get; set; }

}

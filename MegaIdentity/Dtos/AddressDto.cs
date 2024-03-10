using System.ComponentModel.DataAnnotations;

namespace MegaIdentity.Dtos;

public record AddressDto
{ 
    [Required]
    public string Street { get; set; }
    [Required]
    public string City { get; set; }
    [Required]
    public string State { get; set; }
    [Required]
    public string ZipCode { get; set; }
}

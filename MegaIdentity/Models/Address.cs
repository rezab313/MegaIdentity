using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MegaIdentity.Models;

public class Address
{
    public int Id { get; set; } 
    public string Street { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string ZipCode { get; set; }

    [Required]
    public string UserId { get; set; }
    public IdentityUser User { get; set; }
}
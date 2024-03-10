using System.ComponentModel.DataAnnotations;

namespace MegaIdentity.ViewModels;

public class ChangePasswordViewModel
{ 
    [Required]
    [DataType(DataType.Password)]
    public string CurrentPassword { get; set; } = string.Empty;
    [Required]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; } = string.Empty;
}


using System.ComponentModel.DataAnnotations;

namespace ToDo.DTOs;

public class RegisterDto
{
    [Required]
    public string Email { get; set; } = "";

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = "";

    [Required]
    [Compare(nameof(Password))]
    public string ConfirmPassword { get; set; } = "";
}

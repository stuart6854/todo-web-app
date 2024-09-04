using System.ComponentModel.DataAnnotations;

namespace Domain.Models;

public class RegisterModel
{
    [Required]
    [Length(2, 18)]
    [RegularExpression(@"^[\w.\-]+$")]
    public string Username { get; set; }
    [Required]
    [Length(8, 32)]
    [RegularExpression(@"^(?=.*\d)(?=.*[A-Z])(?=.*[a-z])(?=.*[^\w\d\s:])([^\s])+$", ErrorMessage = "Password does not meet security rules.")]
    public string Password { get; set; }
}
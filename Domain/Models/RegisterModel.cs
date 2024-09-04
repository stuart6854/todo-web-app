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
    [RegularExpression(
        @"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).{8,}$",
        ErrorMessage = "Password must be between 8 and 32 characters, have a lowercase letter, a uppercase letter, a number and a special character."
    )]
    public string Password { get; set; }
}
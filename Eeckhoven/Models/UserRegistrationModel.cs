using Eeckhoven.Enums;

namespace Eeckhoven.Models;

public class UserRegistrationModel
{
    public string Email { get; set; }
    public string ConfirmEmail { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public RegistrationType Type { get; set; }
}
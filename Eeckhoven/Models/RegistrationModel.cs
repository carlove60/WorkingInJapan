using Eeckhoven.Enums;
using Microsoft.AspNetCore.Identity;

namespace Eeckhoven.Models;

public class RegistrationModel
{
    public string Email { get; set; }
    public string ConfirmEmail { get; set; }
    public string Password { get; set; }
    
    public string ConfirmPassword { get; set; }
    public RegistrationType Type { get; set; }
}
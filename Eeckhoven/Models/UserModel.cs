using Eeckhoven.Constants;
using Eeckhoven.Enums;
using Microsoft.AspNetCore.Identity;

namespace Eeckhoven.Models;

public class UserModel
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    
    public DateTime DateOfbirth { get; set; }
    public AddressModel Address { get; set; }
    public LanguageEnum Language { get; set; } = LanguageEnum.English;
    public string Role { get; set; } = RoleConstants.Guest;

    public bool IsLoggedIn { get; set; } = false;
    
    public IdentityUser IdentityUser { get; set; }
    
    public ResumeModel Resume { get; set; }
}
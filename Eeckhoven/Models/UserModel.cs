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
    
    public DateTime DateOfBirth { get; set; }
    public AddressModel Address { get; set; }
    public Language Language { get; set; } = Language.English;
    
    public string Role { get; set; } = RoleConstants.Guest;

    public bool IsLoggedIn { get; set; }
    
    public string Nationality { get; set; }

    public VisaStatus VisaStatus { get; set; }
    
    public JLPT JLPT { get; set; } = JLPT.None;

    public LanguageLevel JapaneseLevel { get; set; }

    public LanguageLevel EnglishLevel { get; set; }
    
    public string ProfilePicture { get; set; }
    
    public string Email
    {
        get { return IdentityUser.Email; }
        set { IdentityUser.Email = value; }
    }
    
    public IdentityUser IdentityUser { get; set; }
    
    public ResumeModel Resume { get; set; }
}
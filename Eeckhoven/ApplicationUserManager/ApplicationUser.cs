using Eeckhoven.Models;
using Microsoft.AspNetCore.Identity;

namespace Eeckhoven.ApplicationUserManager;

public class ApplicationUser : IdentityUser
{
    public UserModel UserModel { get; set; }
    
    public UserRegistrationModel UserRegistrationModel { get; set; }
    
    public string RefreshToken { get; set; }
    
    public DateTime RefreshTokenExpiryTime { get; set; }

    public static ApplicationUser FromUserModel(UserModel userModel)
    {
        var user = new ApplicationUser
        {
            UserModel = userModel,
            Email = userModel.Email,
            UserName = userModel.Email,
        };
        return user;
    }
}
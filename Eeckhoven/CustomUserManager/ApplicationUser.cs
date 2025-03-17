using Eeckhoven.Models;
using Microsoft.AspNetCore.Identity;

namespace Eeckhoven.CustomUserManager;

public class ApplicationUser : IdentityUser
{
    public UserModel UserModel { get; set; }
    
    public UserRegistrationModel UserRegistrationModel { get; set; }

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
using Eeckhoven.Models;
using Microsoft.AspNetCore.Identity;

namespace Eeckhoven.ApplicationUserManager;

/// <summary>
/// 
/// </summary>
public class ApplicationUser : IdentityUser
{
    public UserModel UserModel { get; set; }
    
    public RegistrationModel RegistrationModel { get; set; }
    
    public string Token { get; set; }
    
    public DateTime TokenExpiryTime { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userModel"></param>
    /// <returns></returns>
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
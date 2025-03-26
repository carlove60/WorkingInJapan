using Microsoft.AspNetCore.Identity;

namespace Eeckhoven.ApplicationUserManager;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TUser"></typeparam>
public class ApplicationUserValidator<TUser> : UserValidator<TUser>, IApplicationUserValidator<TUser>
    where TUser : ApplicationUser
{

    /// <summary>
    /// 
    /// </summary>
    /// <param name="manager"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    public override async Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user)
    {
        //Some Code
        //var result = await ValidateAsync(manager, user);
        

        return await base.ValidateAsync(manager, user);   
    }

    // private async Task ValidateAsync(UserManager<TUser> manager, TUser user, ICollection<IdentityError> errors)
    // {
    //     //Some Code
    //     return null;
    // }
}
using Microsoft.AspNetCore.Identity;

namespace Eeckhoven.CustomUserManager;

public class CustomUserValidator<TUser> : UserValidator<TUser>, ICustomUserValidator<TUser>
    where TUser : ApplicationUser
{

    public async Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user)
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
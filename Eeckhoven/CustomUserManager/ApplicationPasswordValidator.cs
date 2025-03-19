using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;

namespace Eeckhoven.CustomUserManager;

public class ApplicationPasswordValidator : PasswordValidator<ApplicationUser>
{
    public override Task<IdentityResult> ValidateAsync(UserManager<ApplicationUser> manager, ApplicationUser user, string? password)
    {
        var result = base.ValidateAsync(manager, user, password);
        var registrationModel = user.UserRegistrationModel;
        if (!result.Result.Succeeded && registrationModel.Email != registrationModel.ConfirmEmail)
        {
            var existingErrors = result.Result.Errors.ToList();
            existingErrors.Add(new IdentityError { Code = "EmailError", Description = "Emails do not match" });

            var newResult = IdentityResult.Failed(existingErrors.ToArray());
            return Task.FromResult(newResult);        
        }

        return result;
    }
}
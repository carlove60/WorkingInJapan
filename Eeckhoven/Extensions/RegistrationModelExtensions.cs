using Eeckhoven.ApplicationUserManager;
using Eeckhoven.Models;
using Eeckhoven.Repositories;
using Microsoft.AspNetCore.Identity;

namespace Eeckhoven.Extensions;

public static class RegistrationModelExtensions
{
    public static ApplicationUser ToApplicationUser(this RegistrationModel model)
    {
        var result = new ApplicationUser
        {
            Email = model.Email,
            UserName = model.Email
        };
        return result;
    }
}
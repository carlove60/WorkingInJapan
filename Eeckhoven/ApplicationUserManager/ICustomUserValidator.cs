using Microsoft.AspNetCore.Identity;

namespace Eeckhoven.ApplicationUserManager;

public interface ICustomUserValidator<TUser> : IUserValidator<TUser> where TUser : ApplicationUser
{
}
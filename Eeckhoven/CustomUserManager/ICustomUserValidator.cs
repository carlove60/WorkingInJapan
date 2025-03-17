using Eeckhoven.Models;
using Microsoft.AspNetCore.Identity;

namespace Eeckhoven.CustomUserManager;

public interface ICustomUserValidator<TUser> : IUserValidator<TUser> where TUser : ApplicationUser
{
}
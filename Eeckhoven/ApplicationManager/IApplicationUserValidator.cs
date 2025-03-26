using Microsoft.AspNetCore.Identity;

namespace Eeckhoven.ApplicationUserManager;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TUser"></typeparam>
public interface IApplicationUserValidator<TUser> : IUserValidator<TUser> where TUser : ApplicationUser
{
}
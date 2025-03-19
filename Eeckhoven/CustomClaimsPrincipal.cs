using System.Security.Claims;
using System.Security.Principal;
using Eeckhoven.Models;

namespace Eeckhoven;

public class CustomClaimsPrincipal(IPrincipal principal) : ClaimsPrincipal(principal)
{
    public UserModel CurrentUser { get;  set; } = new UserModel();

    public override bool IsInRole(string role)
    {
        var currentRole = CurrentUser.Role;
        
        return base.IsInRole(role);
    }
}
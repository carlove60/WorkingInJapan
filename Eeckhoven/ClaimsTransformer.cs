using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace Eeckhoven;

public class ClaimsTransformer: IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var customPrincipal = new CustomClaimsPrincipal(principal) as ClaimsPrincipal;
        return Task.FromResult(customPrincipal);
    }
    
}
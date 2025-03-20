using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Eeckhoven.Services;
using Microsoft.IdentityModel.Tokens;

namespace Eeckhoven.Middleware;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;
    private readonly IServiceScopeFactory _scopeFactory;
    
    public JwtMiddleware(RequestDelegate next, IConfiguration configuration, IServiceScopeFactory scopeFactory)
    {
        _next = next;
        _configuration = configuration;
        _scopeFactory = scopeFactory;
    }

    public async Task Invoke(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        context.Response.Headers.Append("New-Token", "12321321");

        if (token != null)
        {
            var principal = GetPrincipalFromExpiredToken(token);
            if (principal != null)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    // Get the scoped ApplicationUserManager inside the request scope
                    var userManager = scope.ServiceProvider.GetRequiredService<ApplicationUserManager.ApplicationUserManager>();

                    // Use userManager as needed

                    var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    var user = await userManager.FindByIdAsync(userId);

                    if (user != null && user.RefreshTokenExpiryTime > DateTime.UtcNow)
                    {
                        var newToken = await new AuthService(userManager, _configuration).GenerateJwtToken(user);
                        context.Response.Headers.Append("New-Token", newToken.Token);
                    }
                }
            }
        }

        await _next(context);
    }

    private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]);

        try
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false // Ignore expiration time
            };

            return tokenHandler.ValidateToken(token, validationParameters, out _);
        }
        catch
        {
            return null;
        }
    }
}

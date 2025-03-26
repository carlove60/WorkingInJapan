using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Eeckhoven.ApplicationUserManager;
using Eeckhoven.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Eeckhoven.Services;

/// <summary>
/// 
/// </summary>
public class AuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userManager"></param>
    /// <param name="configuration"></param>
    public AuthService(UserManager<ApplicationUser> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public async Task<AuthResponse> GenerateJwtToken(ApplicationUser user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: null,
            audience: null,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(_configuration["JwtSettings:TokenLifetimeMinutes"]!)),
            signingCredentials: creds
        );
        
        var writtenToken = new JwtSecurityTokenHandler().WriteToken(token);
        user.Token = writtenToken;
        user.TokenExpiryTime = DateTime.UtcNow.AddDays(int.Parse(_configuration["JwtSettings:RefreshTokenLifetimeDays"]!));

        await _userManager.UpdateAsync(user);

        return new AuthResponse(writtenToken);
    }
}

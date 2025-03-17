using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Eeckhoven.CustomSignInManager;
using Eeckhoven.CustomUserManager;
using Eeckhoven.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Eeckhoven.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ApplicatinUserManager _userManager;
    private readonly ApplicationSignInManager _signInManager;
    private readonly IConfiguration _configuration;
    private readonly ApplicationPasswordValidator _passwordValidator;

    public AuthController(ApplicatinUserManager userManager, ApplicationSignInManager signInManager, IConfiguration configuration, ApplicationPasswordValidator passwordValidator)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _passwordValidator = passwordValidator;
    }
    
    [HttpPost("register")]
    public async Task<ResultObject<string>> Register([FromBody] UserRegistrationModel model)
    {
        var applicationUser = new ApplicationUser();
        applicationUser.UserRegistrationModel = model;
        var validationResult = await _passwordValidator.ValidateAsync(_userManager, applicationUser, model.Password);

        var resultObject = new ResultObject<string>();
        if (!validationResult.Succeeded)
        {

            resultObject.IsError = true;
            resultObject.BadRequest("No", 403);
            return resultObject;
        }

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user != null)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var token = GenerateJwtToken(user.UserModel, roles);

            resultObject.AddResult( token);
        }
        else
        {
            resultObject.BadRequest("", 403);
        }

        return resultObject;
    }

    [HttpPost("login")]
    public async Task<ResultObject<IActionResult>> Login([FromBody] LoginModel model)
    {
        var resultObject = new ResultObject<IActionResult>();
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
        {
            var roles = await _userManager.GetRolesAsync(user);
            var token = GenerateJwtToken(user.UserModel, roles);
            resultObject.AddResult(Ok(new { token }));
            return resultObject;
        }
        resultObject.AddResult(Unauthorized());

        return resultObject;
    }

    private string GenerateJwtToken(UserModel user, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.IdentityUser.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Eeckhoven.ApplicationManager;
using Eeckhoven.ApplicationUserManager;
using Eeckhoven.Models;
using Eeckhoven.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Namotion.Reflection;
using NSwag.Annotations;

namespace Eeckhoven.Controllers;

/// <summary>
/// 
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ApplicationManager.ApplicationUserManager _userManager;
    private readonly ApplicationSignInManager _signInManager;
    private readonly IConfiguration _configuration;
    private readonly ApplicationPasswordValidator _passwordValidator;
    private readonly AuthService _authService;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userManager"></param>
    /// <param name="signInManager"></param>
    /// <param name="configuration"></param>
    /// <param name="passwordValidator"></param>
    /// <param name="authService"></param>
    public AuthController(ApplicationManager.ApplicationUserManager userManager, ApplicationSignInManager signInManager, IConfiguration configuration, ApplicationPasswordValidator passwordValidator, AuthService authService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _passwordValidator = passwordValidator;
        _authService = authService;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("login")]
    public async Task<ResultObject<IActionResult>> Login([FromBody] LoginModel model)
    {
        var resultObject = new ResultObject<IActionResult>();
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
        {
            var token = _authService.GenerateJwtToken(user);
            resultObject.AddResult(Ok(new { token.Result.Token }));
            return resultObject;
        }
        resultObject.AddResult(Unauthorized());

        return resultObject;
    }
    
    [HttpGet("check-session")]
    public ActionResult<string> CheckSession()   
    {
        if (!User.Identity?.IsAuthenticated ?? false)
        {
            return BadRequest("Session expired");
        }
        return Ok("Session is active");
    }
}

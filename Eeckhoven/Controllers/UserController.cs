using Eeckhoven.Constants;
using Eeckhoven.Models;
using Eeckhoven.Repositories;
using Microsoft.AspNetCore.Mvc;
using Eeckhoven.Enums;
using Eeckhoven.Extensions;
using Microsoft.AspNetCore.Identity;

namespace Eeckhoven.Controllers;

[ApiController]
[Route("api/user")]
public class UserController(
    UserRepository userRepository,
    LanguageRepository languageRepository,
    UserManager<IdentityUser> userManager,
    ILogger<UserController> logger)
    : ControllerBase
{
    [HttpPost("register")]
    public ActionResult<ResultObject<MessageList>> Register(UserModel user)
    {
        if (user is null)
        {
            return BadRequest("The user model is null.");
        }
        
        var resultObject = new ResultObject<MessageList>();
        var validatedUser = user.ValidateForRegistration(userRepository);
        resultObject.Records.Add(validatedUser);
        if (!validatedUser.Any())
        {
            resultObject.IsError = true;
        }

        return Ok(resultObject);
    }

    [HttpPost("login")]
    public ActionResult<ResultObject<ValidationMessage>> Login(string email, string password)
    {
        if (HttpContext.User is not CustomClaimsPrincipal currentUser)
        {
            logger.LogWarning("UserController::Login called with invalid user");
            return BadRequest("The current user is not the excepted type of principal");
        }

        var userModel = userRepository.GetUserByEmailAndPassword(email, password);
        if (userModel == null)
        {
            logger.LogWarning("UserController::Login called with invalid email or password");
            return BadRequest("Invalid email or password");
        }
        
        userModel.IsLoggedIn = true;
        currentUser.CurrentUser = userModel;
        HttpContext.User = currentUser;
        return Ok(userModel);
    }

    [HttpPost("changeLanguage")]
    public ActionResult<ResultObject<bool>> ChangeLanguage(LanguageEnum language)
    {
        if (HttpContext.User is not CustomClaimsPrincipal currentUser)
        {
            logger.LogWarning("UserController::ChangeLanguage called with invalid user");
            return BadRequest("The current user is not the excepted type of principal");
        }
        var userModel = userRepository.GetUserById(currentUser.CurrentUser.Id);
        currentUser.CurrentUser.Language = language;
        var result = new ResultObject<UserModel>();
        if (userModel == null)
        {
            result = languageRepository.ChangeLanguage(currentUser.CurrentUser);
        }

        result.IsError = false;
        result.Records.Add(currentUser.CurrentUser);
        return Ok(result);
    }

    [HttpGet("getUser")]
    public ActionResult<UserModel> GetUser(string id)
    {
        var resul2t = userManager.CreateAsync(new IdentityUser() {UserName = "dqwdqwdwqd", Email = "dqwdqwdwqd@gmail.com" }, "Hsg626^2hsdsad(82");
        return Ok(resul2t);
        if (!Guid.TryParse(id, out var userId))
        {
            logger.LogWarning("UserController::GetUser called with invalid id");
            return BadRequest("ID cannot be null or empty");
        }

        var result = userRepository.GetUserById(userId);
        if (result != null)
        {
            logger.LogInformation($"UserController::GetUser called for user {result.FirstName} {result.LastName}");
        }
        else
        {
            logger.LogWarning("UserController::GetUser no user found");
        }

        return Ok(result);
    }
    
    

    [HttpPost]
    public ActionResult<ResultObject<UserModel>> Post(UserModel userModel)
    {
        if (userModel.Id != Guid.Empty)
        {
            return BadRequest("ID cannot be null or empty");
        }

        var result = userRepository.Save(userModel);
        return Ok(result);
    }
}
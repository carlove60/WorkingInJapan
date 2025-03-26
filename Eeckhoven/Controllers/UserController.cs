using Eeckhoven.ApplicationUserManager;
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
    ApplicationManager.ApplicationUserManager userManager,
    ILogger<UserController> logger)
    : ControllerBase
{
    [HttpPost("register")]
    public ActionResult<ResultObject<UserModel>> Register(RegistrationModel? user)
    {
        if (user is null)
        {
            return BadRequest("The user model is null");
        }
        
        var resultObject = new ResultObject<UserModel>();
        var validatedUser = userManager.ValidateForRegistration(user, userRepository);
        if (!validatedUser.Any())
        {
            resultObject.IsError = true;
            resultObject.UserMessages.AddRange(validatedUser);
        }
        else
        {        
            var createResult = userManager.CreateAsync(user);
            createResult.Wait();
            resultObject = createResult.Result;
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
    public ActionResult<ResultObject<bool>> ChangeLanguage(Language language)
    {
        if (HttpContext.User is not CustomClaimsPrincipal currentUser)
        {
            logger.LogWarning("UserController::ChangeLanguage called with invalid user");
            return BadRequest("The current user is not the excepted type of principal");
        }

        currentUser.CurrentUser.Language = language;
        var result = new ResultObject<UserModel>();
        if (currentUser.CurrentUser.IsLoggedIn)
        {
            result = languageRepository.ChangeLanguage(currentUser.CurrentUser);
        }

        result.IsError = false;
        result.Records.Add(currentUser.CurrentUser);
        return Ok(result);
    }

    [HttpGet("getUser")]
    public ResultObject<UserModel> GetUser(string id)
    {
        var result = new ResultObject<UserModel>();
        
        if (!Guid.TryParse(id, out var userId))
        {
            logger.LogWarning("UserController::GetUser called with invalid id");
            result.SystemMessages.Add("ID cannot be null or empty");
        }

        var model = userRepository.GetUserById(userId);
        if (model != null)
        {
            result.AddResult(model);
            logger.LogInformation($"UserController::GetUser called for user {model.FirstName} {model.LastName}");
        }
        else
        {
            logger.LogWarning("UserController::GetUser no user found");
        }

        return result;
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
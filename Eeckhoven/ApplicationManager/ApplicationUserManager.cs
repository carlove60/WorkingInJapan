using Eeckhoven.ApplicationUserManager;
using Eeckhoven.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Eeckhoven.Extensions;
using Eeckhoven.Models;

namespace Eeckhoven.ApplicationManager;

/// <inheritdoc />
public class ApplicationUserManager(
    IUserStore<ApplicationUser> store,
    IOptions<IdentityOptions> optionsAccessor,
    IPasswordHasher<ApplicationUser> passwordHasher,
    IEnumerable<IApplicationUserValidator<ApplicationUser>> userValidators,
    IEnumerable<ApplicationPasswordValidator> passwordValidators,
    ILookupNormalizer keyNormalizer,
    IdentityErrorDescriber errors,
    IServiceProvider tokenProviders,
    ILogger<UserManager<ApplicationUser>> logger,
    UserRepository userRepository)
    : UserManager<ApplicationUser>(store, optionsAccessor, passwordHasher, userValidators, passwordValidators,
        keyNormalizer,
        errors,
        tokenProviders, logger)
{
    public async Task<ResultObject<UserModel>> CreateAsync(RegistrationModel user)
    {
        var resultObject = new ResultObject<UserModel>();
        var validationMessages = ValidateForRegistration(user, userRepository);
        if (!validationMessages.Any())
        {
            resultObject.IsError = true;
            resultObject.UserMessages.AddRange(validationMessages);
        }
        else
        {
            var createResult = await base.CreateAsync(user.ToApplicationUser(), user.Password);
            var createdUser = Users.SingleOrDefault(u => u.Email == user.Email);
            var userModel = new UserModel
            {
                IdentityUser = createdUser
            };
            var userModelSaveResult = userRepository.Save(userModel);

            resultObject.UserMessages.AddRange(userModelSaveResult.UserMessages);
            resultObject.UserMessages.AddRange(createResult.Errors.ToMessageList());
            resultObject.AddResult(userModelSaveResult.Records.Single());
        }

        return resultObject;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    /// <param name="userRepository"></param>
    /// <returns></returns>
    public MessageList ValidateForRegistration(RegistrationModel model, UserRepository userRepository)
    {
        var result = new MessageList();
        if (model.Email.Trim() != model.ConfirmEmail.Trim())
        {
            result.AddError("The two e-mail addresses are not the same", "2つのメールアドレスは同じではありません");
            return result;
        }
        var userExists = userRepository.GetUserByEmail(model.Email);
        if (userExists is not null)
        {
            result.AddError("There is already a user registered with this email.", "このメールアドレスで登録されたユーザーが既に存在します");
            return result;
        }

        if (model.Password.Trim() != model.ConfirmPassword.Trim())
        {
            result.AddError("Passwords do not match", "<UNK>");
        }

        var identiyModel = model.ToApplicationUser();
        if (!identiyModel.Email.IsValidEmail())
        {
            result.AddError("Email is not a valid email", "<UNK>");
        }
        
        var validator = new PasswordValidator<ApplicationUser>();
        var validationResult = validator.ValidateAsync(this, model.ToApplicationUser(), model.Password);
        if (validationResult.Result.Errors.Any())
        {
            result.AddRange(validationResult.Result.Errors.ToMessageList());
        }
        return result;
    }
}
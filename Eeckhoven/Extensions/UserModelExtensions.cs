using Eeckhoven.Models;
using Eeckhoven.Repositories;

namespace Eeckhoven.Extensions;

public static class UserModelExtensions
{
    public static MessageList ValidateForRegistration(this UserModel user, UserRepository userRepository)
    {
        var validatedUserModel = user.Validate();
        if (validatedUserModel.Any())
        {
            return validatedUserModel;
        }
        var userExists = userRepository.GetUserByEmail(user.IdentityUser.Email);
        var result = new MessageList();
        if (userExists is not null)
        {
            result.AddError("There is already a user registered with this email.", "このメールアドレスで登録されたユーザーが既に存在します");
        }
        return result;
    }

    public static MessageList Validate(this UserModel model)
    {
        var result = new MessageList();
        
        if (string.IsNullOrWhiteSpace(model.FirstName))
        {
            result.AddError("First name is required", "名を入力してください");
        }

        if (string.IsNullOrWhiteSpace(model.LastName))
        {
            result.AddError("Last name is required","姓を入力してください");
        }
        
        if (string.IsNullOrWhiteSpace(model.IdentityUser.Email))
        {
            result.AddError("Email is required","メールアドレスを入力してください");
        }
        
        if (!model.IdentityUser.Email.IsValidEmail())
        {
            result.AddError("Email is not a valid","メールアドレスが無効です");
        }

        if (!model.IdentityUser.PasswordHash.IsValidPassword())
        {
            result.AddError("Password must be 8 characters long, contain 1 symbol and 1 capital letter.","パスワードは 8 文字で、1 つの記号と 1 つの大文字を含める必要があります。");
        }

        return result;
    }
}
using Eeckhoven.ApplicationUserManager;
using Eeckhoven.Models;
using Eeckhoven.Repositories;

namespace Eeckhoven.Extensions;

/// <summary>
/// 
/// </summary>
public static class UserModelExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public static MessageList Validate(this UserModel model)
    {
        var result = new MessageList();
        
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
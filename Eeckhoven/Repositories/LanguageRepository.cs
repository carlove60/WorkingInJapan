using Eeckhoven.Database;
using Eeckhoven.Models;

namespace Eeckhoven.Repositories;

public class LanguageRepository(ApplicationDbContext applicationDbContext) : BaseRepository(applicationDbContext)
{
    private readonly ApplicationDbContext _applicationDbContext = applicationDbContext;

    public ResultObject<UserModel> ChangeLanguage(UserModel userModel)
    {
        var result = new ResultObject<UserModel>();
        var user = _applicationDbContext.Users.FirstOrDefault();
        if (user is null)
        {
            result.Messages.AddError("User not found", "ユーザーが見つかりませんでした");
            result.IsError = true;
        }
        else
        {
            user.Language = userModel.Language;
            _applicationDbContext.SaveChanges();
            result.IsError = false;
            result.Records.Add(user);
        }
        
        return result;
    }
}
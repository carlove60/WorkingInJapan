using Eeckhoven.Constants;
using Eeckhoven.Database;
using Eeckhoven.Enums;
using Eeckhoven.Models;
using Eeckhoven.Extensions;

namespace Eeckhoven.Repositories;

public class UserRepository : BaseRepository
{
    private readonly ApplicationDbContext _applicationDbContext;
    public UserRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public UserModel? GetUserByEmailAndPassword(string email, string password)
    {
        if (email.IsEmptyOrWhiteSpace() || password.IsEmptyOrWhiteSpace())
        {
            return null;
        }
        
        return _applicationDbContext.Users.FirstOrDefault(u => u.IdentityUser.Email == email && u.IdentityUser.PasswordHash == password.Hash());
    }
    
    public UserModel? GetUserByEmail(string email)
    {
        if (email.IsEmptyOrWhiteSpace())
        {
            return null;
        }

        return _applicationDbContext.Users.FirstOrDefault(u => u.IdentityUser.Email == email);
    }

    public List<UserModel> GetEmployers()
    {
        return _applicationDbContext.Users.Where(u => u.Role == RoleConstants.Employer).ToList();
    }
    
    public List<UserModel> GetEmployees()
    {
        return _applicationDbContext.Users.Where(u => u.Role == RoleConstants.Employee).ToList();
    }
    
    public List<UserModel> GetSearchees()
    {
        return _applicationDbContext.Users.Where(u => u.Role == RoleConstants.Searcher).ToList();
    }

    public UserModel? GetUserById(Guid id)
    {
        return _applicationDbContext.Users.FirstOrDefault((u) => u.Id == id);
    }
    
    public ResultObject<bool> ChangeLanguage(UserModel userModel, LanguageEnum language)
    {
        ResultObject<bool> result = new ResultObject<bool>();
        var user = _applicationDbContext.Users.FirstOrDefault();
        if (user == null)
        {
            result.Messages.AddError("User not found", "ユーザーが見つかりませんでした");
            result.IsError = true;
        }
        else
        {
            user.Language = language;
            _applicationDbContext.SaveChanges();
            result.IsError = false;
        }
        
        return result;
    }

    public ResultObject<UserModel> Save(UserModel model)
    {
        var resultObject = new ResultObject<UserModel>();
        var validationMessages = model.Validate();
        if (validationMessages.Any())
        {
            resultObject.Messages.AddRange(validationMessages);
            return resultObject;
        }
        var userEntity = _applicationDbContext.Users.FirstOrDefault((user) => user.Id == model.Id);
        if (userEntity == null)
        {
            _applicationDbContext.Users.Add(model);
        }
        else
        {
            userEntity.IdentityUser.Email = model.IdentityUser.Email;
            userEntity.Role = model.Role;
            userEntity.Address = model.Address;
            userEntity.PhoneNumber = model.PhoneNumber;
            userEntity.FirstName = model.FirstName;
            userEntity.LastName = model.LastName;
            userEntity.IdentityUser.PasswordHash = model.IdentityUser.PasswordHash;
            _applicationDbContext.Users.Update(userEntity);
        }
        resultObject.Records.Add(model);
        resultObject.IsError = false;
        return resultObject;
    }
}
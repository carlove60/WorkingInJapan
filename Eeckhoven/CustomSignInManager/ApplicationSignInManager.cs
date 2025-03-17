using Eeckhoven.CustomUserManager;
using Eeckhoven.Database;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Eeckhoven.CustomSignInManager;

public class ApplicationSignInManager(
    UserManager<ApplicationUser> userManager,
    IHttpContextAccessor contextAccessor,
    IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory,
    IOptions<IdentityOptions> optionsAccessor,
    ILogger<SignInManager<ApplicationUser>> logger,
    IAuthenticationSchemeProvider schemes,
    IUserConfirmation<ApplicationUser> confirmation,
    ApplicationDbContext dbContext)
    : SignInManager<ApplicationUser>(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes,
        confirmation)
{
    private readonly UserManager<ApplicationUser> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    private readonly ApplicationDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    private readonly IHttpContextAccessor _contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));

    public override async Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
    {
        // Custom pre-sign-in logic (e.g., logging, additional checks)
        Console.WriteLine($"Attempting login for {userName}");

        var user = await UserManager.FindByNameAsync(userName);
        if (user == null)
        {
            return SignInResult.Failed;
        }

        // Call the base method to handle normal sign-in logic
        var result = await base.PasswordSignInAsync(userName, password, isPersistent, lockoutOnFailure);

        // Custom post-sign-in logic
        if (result.Succeeded)
        {
            Console.WriteLine($"{userName} successfully signed in.");
            var userModel = _dbContext.Users.FirstOrDefault((model => model.IdentityUser == user));
            user.UserModel = userModel;
        }

        return result;
    }
}

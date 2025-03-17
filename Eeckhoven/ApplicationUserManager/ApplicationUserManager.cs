using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Eeckhoven.ApplicationUserManager;

public class ApplicationUserManager(
    IUserStore<ApplicationUser> store,
    IOptions<IdentityOptions> optionsAccessor,
    IPasswordHasher<ApplicationUser> passwordHasher,
    IEnumerable<ICustomUserValidator<ApplicationUser>> userValidators,
    IEnumerable<ApplicationPasswordValidator> passwordValidators,
    ILookupNormalizer keyNormalizer,
    IdentityErrorDescriber errors,
    IServiceProvider tokenProviders,
    ILogger<UserManager<ApplicationUser>> logger)
    : UserManager<ApplicationUser>(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer,
        errors,
        tokenProviders, logger);
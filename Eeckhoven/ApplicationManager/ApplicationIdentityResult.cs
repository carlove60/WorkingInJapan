using Eeckhoven.Enums;
using Eeckhoven.Models;
using Microsoft.AspNetCore.Identity;

namespace Eeckhoven.ApplicationUserManager;

public static class IdentityResultExtentions
{
    public static MessageList ToMessageList(this IEnumerable<IdentityError> errors)
    {
        var result = new MessageList();
        foreach (var error in errors)
        {
            var errorMessage = error.Description;
            var message = new ValidationMessage
            {
                MessageEnglish = errorMessage,
                MessageJapanese = errorMessage,
                Type = MessageType.Error
            };
            result.Add(message);
        }
        
        return result;
    }

}
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Org.BouncyCastle.Utilities;

namespace Eeckhoven.Extensions;

public static class StringExtensions
{
    private const string PasswordSalt = "WhereIsMyStrongZero";
    public static bool IsValidEmail(this string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return true;
        }
        
        MailAddress.TryCreate(email, out var result);
        return result != null;
    }

    public static bool IsValidPassword(this string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        { 
            return false;
        }

        // Requires at least 8 characters, one uppercase letter, and one symbol
        var regex = new Regex(@"^(?=.*[A-Z])(?=.*[^a-zA-Z0-9]).{8,}$");
        return regex.IsMatch(password);
    }

    public static string Hash(this string password)
    {
        var hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password!,
            salt: Encoding.ASCII.GetBytes(PasswordSalt),
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 256 / 8));
        
        return hashed;
    }

    public static bool IsEmptyOrWhiteSpace(this string input)
    {
        return string.IsNullOrWhiteSpace(input);
    }
    
    public static string SafeReplace(this string input, string find, string replace, bool matchWholeWord)
    {
        var textToFind = matchWholeWord ? $@"\b{find}\b" : find;
        return Regex.Replace(input, textToFind, replace);
    }
}
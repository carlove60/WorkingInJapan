using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Org.BouncyCastle.Utilities;

namespace Eeckhoven.Extensions;

/// <summary>
/// 
/// </summary>
public static class StringExtensions
{
    private const string PasswordSalt = "WhereIsMyStrongZero";
    /// <summary>
    /// 
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public static bool IsValidEmail(this string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return true;
        }
        
        MailAddress.TryCreate(email, out var result);
        return result != null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool IsEmptyOrWhiteSpace(this string input)
    {
        return string.IsNullOrWhiteSpace(input);
    }
}
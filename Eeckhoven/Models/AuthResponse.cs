namespace Eeckhoven.Models;

/// <summary>
/// 
/// </summary>
public class AuthResponse
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="token"></param>
    public AuthResponse(string token)
    {
        Token = token;
    }

    /// <summary>
    /// 
    /// </summary>
    public string Token { get; set; }
}
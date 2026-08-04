using System.Security.Cryptography;
using System.Text;

namespace thefirey33_backend.Services;

public interface IAuthorizationCodeService
{
    /// <summary>
    ///     Create the authorization code specified.
    /// </summary>
    public void CreateAuthorizationCode();

    /// <summary>
    ///     Check if the authorization code is OK.
    /// </summary>
    /// <param name="code">The code provided to check.</param>
    public bool CheckAuthorizationCode(string code);
}

public class AuthorizationCodeService(ILogger<AuthorizationCodeService> logger) : IAuthorizationCodeService
{
    /// <summary>
    ///     The length of the randomly generated code.
    /// </summary>
    private const int CodeLength = 32;

    /// <summary>
    ///     This is content that the code generator will use to create a code.
    /// </summary>
    private const string CodeGeneratorChars = "QWERTYUIOPASDFGHJKLZXCVBNM123456789";

    /// <summary>
    ///     The authorization code.
    /// </summary>
    private string _code = string.Empty;

    public void CreateAuthorizationCode()
    {
        if (_code != string.Empty)
            return;

        var stringBuilder = new StringBuilder();
        for (var i = 0; i < CodeLength; i++)
        {
            var codeUpRange = RandomNumberGenerator.GetInt32(CodeGeneratorChars.Length);
            var @char = CodeGeneratorChars[codeUpRange];
            var isLower = RandomNumberGenerator.GetInt32(0, 2) == 0;

            stringBuilder.Append(isLower
                ? char.ToLower(@char)
                : @char);
        }

        _code = stringBuilder.ToString();
        logger.LogInformation("Created Auth Code: {Code}... Use this code when logging in!", _code);
    }

    /// <summary>
    ///     Check if the Auth code is correct.
    /// </summary>
    /// <param name="code">The code to check.</param>
    public bool CheckAuthorizationCode(string code)
    {
        if (_code != code) return false;
        _code = string.Empty;
        return true;
    }
}
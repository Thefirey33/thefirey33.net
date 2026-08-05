using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using thefirey33_backend.Services;
using thefirey33_backend.Types;

namespace thefirey33_backend.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(
    IConfiguration configuration,
    IAuthorizationCodeService authorizationCodeService)
    : ControllerBase
{
    private readonly PasswordHasher<string> _passwordHasher = new();

    /// <summary>
    ///     This will automatically create a hashed password for the only admin user.
    /// </summary>
    /// <exception cref="NullReferenceException">If there's no admin password provided.</exception>
    private string HashedPassword
    {
        get
        {
            field ??= _passwordHasher.HashPassword(string.Empty,
                Environment.GetEnvironmentVariable("ADMIN_PASSWORD") ??
                throw new NullReferenceException("Password not provided!"));

            return field;
        }
    }

    /// <summary>
    ///     The JWT Key of the authorization system.
    /// </summary>
    /// <exception cref="NullReferenceException">When a key is not provided.</exception>
    private SymmetricSecurityKey JwtKey =>
        new(Encoding.UTF8.GetBytes(
            configuration["Jwt:Key"] ?? throw new NullReferenceException("JWT key not provided!")));

    /// <summary>
    ///     The JWT Issuer of the authorization system.
    /// </summary>
    /// <exception cref="NullReferenceException">When an issuer is not provided.</exception>
    private string JwtIssuer =>
        configuration["Jwt:Issuer"] ?? throw new NullReferenceException("Issuer is not provided!");

    /// <summary>
    ///     The JWT Audience of the authorization system.
    /// </summary>
    /// <exception cref="NullReferenceException">When an audience is not provided.</exception>
    private string JwtAudience =>
        configuration["Jwt:Audience"] ?? throw new NullReferenceException("Audience is not provided!");

    /// <summary>
    ///     Attempt to verify the admin password.
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    /// <exception cref="NullReferenceException"></exception>
    public bool VerifyPassword(string password)
    {
        return _passwordHasher.VerifyHashedPassword(string.Empty,
            HashedPassword, password) == PasswordVerificationResult.Success;
    }

    /// <summary>
    ///     Attempt to log in as an admin.
    /// </summary>
    /// <param name="userType">The userType, with the username and password details.</param>
    [HttpPost("login")]
    public IActionResult Login([FromBody] UserTypeRequest userType)
    {
        if (userType.Username != Environment.GetEnvironmentVariable("ADMIN_USERNAME") ||
            !VerifyPassword(userType.Password) ||
            !authorizationCodeService.CheckAuthorizationCode(userType.AuthorizationCode)) return Unauthorized();

        var password = GenerateJwtToken(userType);
        Response.Cookies.Append("Token", password);

        // Return the specified cookie with the Token.
        return Ok(new
        {
            Token = password
        });
    }

    /// <summary>
    ///     Create the specified auth code.
    /// </summary>
    [HttpPost("code")]
    public IActionResult CreateAuthCode()
    {
        // Create the specified auth code.
        authorizationCodeService.CreateAuthorizationCode();

        return Ok();
    }

    /// <summary>
    /// Check if the user is authenticated or not.
    /// </summary>
    /// <returns></returns>
    [HttpGet("check")]
    public IActionResult CheckJwtToken()
    {
        if (User.Identity == null)
            return Unauthorized();

        // Check if the specified user is authenticated or not.
        return User.Identity.IsAuthenticated ? Ok() : Unauthorized();
    }


    /// <summary>
    ///     This function generates the Jwt Token that the user will use.
    /// </summary>
    /// <param name="userType">The UserType data type.</param>
    /// <exception cref="NullReferenceException">When there's no Key, Issuer or Audience provided, this exception is thrown.</exception>
    public string GenerateJwtToken(UserType userType)
    {
        var creds = new SigningCredentials(JwtKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userType.Username),
            new Claim(ClaimTypes.Role, "Administrator")
        };

        var token = new JwtSecurityToken(
            JwtIssuer,
            JwtAudience,
            claims,
            expires: DateTime.UtcNow.AddHours(5),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
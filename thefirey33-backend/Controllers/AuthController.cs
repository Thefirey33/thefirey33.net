using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using thefirey33_backend.Types;

namespace thefirey33_backend.Controllers;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class AuthController(IConfiguration configuration): ControllerBase
{
    private readonly PasswordHasher<string> _passwordHasher = new();

    /// <summary>
    /// This will automatically create a hashed password for the only admin user.
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
    /// The JWT Key of the authorization system.
    /// </summary>
    /// <exception cref="NullReferenceException">When a key is not provided.</exception>
    private SymmetricSecurityKey JwtKey => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? throw new NullReferenceException("JWT key not provided!")));
    
    /// <summary>
    /// The JWT Issuer of the authorization system.
    /// </summary>
    /// <exception cref="NullReferenceException">When an issuer is not provided.</exception>
    private string JwtIssuer => configuration["Jwt:Issuer"] ?? throw new NullReferenceException("Issuer is not provided!");
    
    /// <summary>
    /// The JWT Audience of the authorization system.
    /// </summary>
    /// <exception cref="NullReferenceException">When an audience is not provided.</exception>
    private string JwtAudience => configuration["Jwt:Audience"] ?? throw new NullReferenceException("Audience is not provided!");

    /// <summary>
    /// Attempt to verify the admin password.
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
    /// Attempt to log in as an admin.
    /// </summary>
    /// <param name="userType">The userType, with the username and password details.</param>
    [HttpPost("login")]
    public IActionResult Login(UserType userType)
    {
        if (userType.Username != Environment.GetEnvironmentVariable("ADMIN_USERNAME") ||
            !VerifyPassword(userType.Password)) return Unauthorized();
        
        var password = GenerateJwtToken(userType);
        Response.Cookies.Append("Token", password);
        
        return Ok(new
        {
            Token = password,
        });
    }

    [HttpGet("check")]
    public async Task<IActionResult> CheckJwtToken()
    {
        var tokenCookie = Request.Cookies["Token"];

        var jwtValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = JwtIssuer,
            ValidateAudience = true,
            ValidAudience = JwtAudience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = JwtKey,
            ValidateLifetime = true,
        };
        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        
        if (!jwtSecurityTokenHandler.CanReadToken(tokenCookie)) return Unauthorized();
        var result = await jwtSecurityTokenHandler.ValidateTokenAsync(tokenCookie, jwtValidationParameters);
        
        if (result.IsValid)
            return Ok();

        return Unauthorized();
    }
    
    
    /// <summary>
    /// This function generates the Jwt Token that the user will use.
    /// </summary>
    /// <param name="userType">The UserType data type.</param>
    /// <exception cref="NullReferenceException">When there's no Key, Issuer or Audience provided, this exception is thrown.</exception>
    public string GenerateJwtToken(UserType userType)
    {
        var creds = new SigningCredentials(JwtKey, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userType.Username),
        };
        
        var token = new JwtSecurityToken(
            issuer: JwtIssuer,
            audience: JwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(5),
            signingCredentials: creds);
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
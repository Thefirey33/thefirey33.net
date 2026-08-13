using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using thefirey33.catpetterzBackend.AuthenticationHandler;
using thefirey33.catpetterzBackend.Types;

namespace thefirey33.catpetterzBackend.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class AuthManagerController(IHttpClientFactory httpClientFactory) : ControllerBase
{
    /// <summary>
    ///     The HTTPClient that is connected to the filtering service for easy authentication.
    /// </summary>
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("FilteringServiceAPI");


    /// <summary>
    ///     Wrapper for the FilteringService API's redirect link service.
    /// </summary>
    /// <returns>The redirection link.</returns>
    [HttpGet("link")]
    public async Task<DiscordAuthLinkResponse?> GetRedirectLink([FromQuery] string path)
    {
        var link = await _httpClient.GetFromJsonAsync<DiscordAuthLinkResponse>(
            $"/auth/login?redirect_uri={path}");

        return link;
    }

    [HttpGet("auth")]
    [Authorize]
    public IActionResult CheckAuth()
    {
        // Attempt to find all the claims that this user has.
        var nameClaim = User.FindFirst(ClaimTypes.Name);
        var emailClaim = User.FindFirst(ClaimTypes.Email);
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var profileClaim = User.FindFirst(DiscordAuthenticationChallengeHandler.AvatarUrlClaim);

        // If the specified claims do not exist for this user,
        // Mark them as unauthorized.
        if (nameClaim == null || emailClaim == null || profileClaim == null || userIdClaim == null)
            return Unauthorized();

        // Return the specified authorized response.
        return Ok(
            new DiscordAuthResponse
            {
                Id = userIdClaim.Value,
                Email = emailClaim.Value,
                Username = nameClaim.Value,
                AvatarUrl = profileClaim.Value
            }
        );
    }
}
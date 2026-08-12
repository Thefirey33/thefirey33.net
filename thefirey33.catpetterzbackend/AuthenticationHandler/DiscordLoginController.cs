using Microsoft.AspNetCore.Mvc;
using thefirey33.catpetterzBackend.Types;

namespace thefirey33.catpetterzBackend.AuthenticationHandler;

[ApiController]
[Route("/api/[controller]")]
public class DiscordLoginController(IHttpClientFactory httpClientFactory) : ControllerBase
{
    /// <summary>
    ///     This is the HTTPClient for the filtering service API's auth check.
    /// </summary>
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("FilteringServiceAPI");

    /// <summary>
    ///     Attempt to authenticate with Discord Async.
    /// </summary>
    /// <param name="code">The Authentication Code that Discord returns.</param>
    /// <exception cref="NullReferenceException">If the redirection is not specified.</exception>
    [HttpGet]
    public async Task<IActionResult> AuthenticateAsync([FromQuery] string code)
    {
        var result = await _httpClient.GetFromJsonAsync<DiscordAuthTokenResponse>($"/auth/callback?code={code}");
        if (result == null)
            return Unauthorized();

        // Add the Cookie that the browser will interpret.
        Response.Cookies.Append("discordtoken", result.AccessToken, new CookieOptions
        {
            Secure = true,
            HttpOnly = true,
            Expires = DateTimeOffset.Now.AddHours(5),
            Path = "/"
        });

        return RedirectPermanent(Environment.GetEnvironmentVariable("REDIRECT_URI")
                                 ??
                                 throw new NullReferenceException("Redirection not specified!")
        );
    }
}
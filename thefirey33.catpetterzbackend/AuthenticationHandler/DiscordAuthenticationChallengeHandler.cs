using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using thefirey33.catpetterzBackend.Types;

namespace thefirey33.catpetterzBackend.AuthenticationHandler;

public class DiscordAuthenticationChallengeHandler(
    IOptionsMonitor<DiscordAuthenticationOptions> options,
    ILoggerFactory logger,
    IHttpClientFactory httpClientFactory,
    UrlEncoder encoder)
    : AuthenticationHandler<DiscordAuthenticationOptions>(options, logger, encoder)
{
    /// <summary>
    ///     This is the claim of the AvatarUrl.
    /// </summary>
    public const string AvatarUrlClaim = "AvatarUrl";

    /// <summary>
    ///     This is the claim of the AvatarUrl.
    /// </summary>
    public const string UserIdClaim = "UserId";

    /// <summary>
    ///     This is the HTTPClient for the filtering service API's auth check.
    /// </summary>
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("FilteringServiceAPI");

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        string? authToken;

        // Attempt to grab it via the Authorization Header or the Cookie Storage.
        if (Request.Headers.TryGetValue(HeaderNames.Authorization, out var authorization))
            authToken = authorization.FirstOrDefault(auth => auth != null && auth.Contains("Bearer"));
        else if (Request.Cookies.TryGetValue("discordtoken", out var cookie))
            authToken = cookie;
        else
            return AuthenticateResult.Fail("No Authorization Token or Header Provided!");

        // Attempt to get a JWT Bearer token from the values list.
        if (authToken == null)
            return AuthenticateResult.Fail("No Authorization Token Provided!");

        var authorizationHeader = new AuthenticationHeaderValue("Bearer", authToken.Replace("Bearer ", string.Empty));
        var authGetRequest = new HttpRequestMessage(HttpMethod.Get, "/auth/authenticated")
        {
            Headers =
            {
                Authorization = authorizationHeader
            }
        };

        var authCheck = await _httpClient.SendAsync(authGetRequest);
        if (!authCheck.IsSuccessStatusCode) return AuthenticateResult.Fail("Authentication Failed!");

        var authDetailsRequest = new HttpRequestMessage(HttpMethod.Get, "/auth/user")
        {
            Headers =
            {
                Authorization = authorizationHeader
            }
        };
        var authDetails = await _httpClient.SendAsync(authDetailsRequest);
        authDetails.EnsureSuccessStatusCode();

        // Attempt to get the information about the user.
        var response = await authDetails.Content.ReadFromJsonAsync<DiscordAuthResponse>();
        if (response == null)
            return AuthenticateResult.Fail("No Auth Response from the Discord API.");

        var claims = new[]
        {
            new Claim(UserIdClaim, response.Id),
            new Claim(ClaimTypes.Name, response.Username),
            new Claim(ClaimTypes.Email, response.Email),
            new Claim(AvatarUrlClaim, response.AvatarUrl ?? string.Empty)
        };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        return AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name));
    }
}
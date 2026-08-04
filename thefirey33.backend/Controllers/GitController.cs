using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using thefirey33_backend.Types;

namespace thefirey33_backend.Controllers;

[ApiController]
[Route("[controller]")]
public class GitController(IHttpClientFactory httpClientFactory)
    : ControllerBase
{
    /// <summary>
    ///     The duration of how long the data is cached for.
    /// </summary>
    private const int CacheDuration = 120 * 60;

    /// <summary>
    ///     The HTTP client that will make requests.
    /// </summary>
    private readonly HttpClient _client = httpClientFactory.CreateClient("GitHubAPI");

    /// <summary>
    ///     The user agent that will be parsed.
    /// </summary>
    private static ProductInfoHeaderValue ProductInfoHeaderValue => ProductInfoHeaderValue.Parse("FireServer");

    /// <summary>
    ///     Attempted to get the tentrillion commit history.
    /// </summary>
    [HttpGet]
    [OutputCache(Duration = CacheDuration)]
    public async Task<IActionResult> Get()
    {
        var requestMessage = new HttpRequestMessage
        {
            RequestUri =
                new Uri($"{_client.BaseAddress}repos/tentrillion-game-engine/tentrillion-game-engine/commits"),
            Headers =
            {
                UserAgent = { ProductInfoHeaderValue }
            }
        };

        var result =
            await _client.SendAsync(requestMessage);
        if (result.StatusCode != HttpStatusCode.OK)
            return StatusCode(Convert.ToInt32(result.StatusCode), result.Content);

        return Ok(await result.Content.ReadFromJsonAsync<List<TenTrillionGitType>>());
    }

    /// <summary>
    ///     Attempt to get all of my repositories.
    /// </summary>
    [HttpGet("repositories")]
    [OutputCache(Duration = CacheDuration)]
    public async Task<IActionResult> GetRepositories()
    {
        var requestMessage = new HttpRequestMessage
        {
            RequestUri = new Uri($"{_client.BaseAddress}users/thefirey33/repos"),
            Headers =
            {
                UserAgent = { ProductInfoHeaderValue }
            }
        };

        var result = await _client.SendAsync(requestMessage);
        if (result.StatusCode != HttpStatusCode.OK)
            return StatusCode(Convert.ToInt32(result.StatusCode), result.Content);

        var content = await result.Content.ReadFromJsonAsync<List<RepositoryGitType>>();
        if (content == null) return NotFound();

        var orderedResult = content
            .OrderBy(type => type.CreatedAt)
            .Reverse();

        return Ok(orderedResult);
    }
}
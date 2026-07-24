using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using StackExchange.Redis;
using thefirey33_backend.Types;

namespace thefirey33_backend.Services;


public class GitService(IHttpClientFactory clientFactory, IConnectionMultiplexer connectionMultiplexer, ILogger<GitService> logger): BackgroundService
{
    /// <summary>
    /// The amount of time in intervals that the backend will update it's internal git data.
    /// </summary>
    private readonly TimeSpan _updatePollyTime = TimeSpan.FromHours(3);

    /// <summary>
    /// The Redis Key for the storage of this specified GitHub data.
    /// </summary>
    public const string GitDataRedisKey = "git-data";

    private async Task ReloadData()
    {
        // Get the current Redis database, for fast storage of the current Git data.
        var database = connectionMultiplexer.GetDatabase();

        string? redisData = database.StringGet(GitDataRedisKey);
        
        // Request to the GitHub API for the tracked commits.
        var request = new HttpRequestMessage(HttpMethod.Get, "/repos/tentrillion-game-engine/tentrillion-game-engine/commits")
        {
            Headers =
            {
                UserAgent = { ProductInfoHeaderValue.Parse("Thefirey33WebsiteServer") }
            }
        };

        // If the redis successfully returned a value, then use that ETag to check for changes.
        if (redisData != null)
        {
            var beforeData = JsonSerializer.Deserialize<GitWrapper>(redisData);
            if (beforeData?.EntityTagHeaderValue != null)
                request.Headers.IfNoneMatch.Add(new EntityTagHeaderValue(beforeData.EntityTagHeaderValue, true));
        }
        
        
        var result = await _client.SendAsync(request);
        
        // If the result is null, show the warning.
        if (result.StatusCode is HttpStatusCode.TooManyRequests or HttpStatusCode.NotModified)
        {
            logger.LogWarning("Hit API request limit or content the same... skipping");
            return;
        }
        
        var serializedContent = await result.Content.ReadFromJsonAsync<List<GitType>>();
        
        // If there's no content returned, return and do not omit the data.
        if (serializedContent == null)
            return;
        
        var wrappedContent = new GitWrapper
        {
            EntityTagHeaderValue = result.Headers.ETag?.Tag,
            GitData = serializedContent
        };

        // Finally set the value of it inside the redis.
        database.StringSet(GitDataRedisKey, JsonSerializer.Serialize(wrappedContent));
    }
    
    private readonly HttpClient _client =
        clientFactory.CreateClient("GitHubAPI");
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(_updatePollyTime);
        
        // Execute the initial reloading of the data.
        await ReloadData();
        
        // Every day, the GitHub API will receive the specified request.
        // Using the HTTP Header IfNoneMatch, it will use conditional requesting.
        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            await ReloadData();
        }
    }
}
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using thefirey33_backend.Services;
using thefirey33_backend.Types;

namespace thefirey33_backend.Controllers;

[ApiController]
[Route("[controller]")]
public class GitController(IConnectionMultiplexer connectionMultiplexer) : ControllerBase
{
    /// <summary>
    ///     Attempt to get the specified GitHub data from the GitHub API, which is cached in Redis.
    /// </summary>
    [HttpGet]
    public IActionResult Get()
    {
        var database = connectionMultiplexer.GetDatabase();

        // Attempt to receive the specified data from the Redis Cache.
        string? receivedData = database.StringGet(GitService.GitDataRedisKey);

        // If the received data from the Redis is NULL, 
        // // Then return 404.
        if (receivedData == null)
            return NotFound();

        var data = JsonSerializer.Deserialize<GitWrapper>(receivedData);

        // If the JSON wasn't able to be parsed or is empty,
        // Then return 404.
        if (data == null)
            return NotFound();

        // Finally return the specified data.
        return Ok(data.GitData);
    }
}
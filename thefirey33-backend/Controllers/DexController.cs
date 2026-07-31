using System.IO.Compression;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using thefirey33_backend.Services;
using thefirey33_backend.Types.Database.Context;
using thefirey33_backend.Types.Database.Dex;

namespace thefirey33_backend.Controllers;

[ApiController]
[Route("[controller]")]
public class DexController(
    NikoDexRecoveryContext nikoDexRecoveryContext,
    IDexDataService dexDataService)
    : ControllerBase
{
    /// <summary>
    ///     The amount of pagination that the API will do.
    /// </summary>
    private const int PaginationAmount = 30;

    /// <summary>
    ///     The amount of time this request will stay cached in redis.
    /// </summary>
    private const int CacheTime = 60;

    /// <summary>
    ///     JSON Serialization Options.
    /// </summary>
    private readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };


    /// <summary>
    ///     Attempt to get the date of the latest backup.
    /// </summary>
    /// <returns>Date of the backup. 404 if not found.</returns>
    [HttpGet]
    public async Task<IActionResult> GetBackupDate()
    {
        var itemCount = await nikoDexRecoveryContext.NikoTypeRecoveryDb.CountAsync() / PaginationAmount;
        var firstResult = await nikoDexRecoveryContext.NikoDexRecovery.FirstOrDefaultAsync();

        if (firstResult == null) return NotFound();

        return Ok(new NikoRecoveryResponse
        {
            AmountPages = itemCount,
            BackupTime = firstResult.Date
        });
    }

    [HttpGet("leaderboard")]
    [OutputCache(Duration = CacheTime)]
    public async Task<List<LeaderboardResponse>> GetLeaderboard()
    {
        var results = await nikoDexRecoveryContext
            .NikoTypeRecoveryDb
            .GroupBy(db => db.AuthorName ?? "NoAuthor")
            .Select(dbs => new LeaderboardResponse
            {
                Author = dbs.Key,
                Count = dbs.Count()
            })
            .ToListAsync();

        return results.OrderByDescending(response => response.Count).ToList();
    }

    /// <summary>
    ///     Get the nikos with a range. (Pagination.)
    /// </summary>
    /// <param name="pageStart">The start of the page, where it will be added with the pagination amount.</param>
    /// <returns>The list of Nikos.</returns>
    [HttpGet("page/{pageStart:int}")]
    [OutputCache(Duration = CacheTime)]
    public async Task<List<NikoTypeRecoveryDbResponse>> GetNikos(int pageStart)
    {
        var results = await nikoDexRecoveryContext.NikoTypeRecoveryDb
            .Include(type => type.Abilities)
            .OrderBy(response => response.Id)
            .Skip(pageStart * PaginationAmount)
            .Take(PaginationAmount)
            .Select(db => NikoTypeRecoveryDbResponse.FromRecoveryDb(db))
            .ToListAsync();

        return results;
    }

    /// <summary>
    ///     Get the niko specified.
    /// </summary>
    /// <param name="id">ID.</param>
    [HttpGet("niko/{id:int}")]
    [OutputCache(Duration = CacheTime)]
    public async Task<IActionResult> GetNiko(int id)
    {
        var result = await nikoDexRecoveryContext.NikoTypeRecoveryDb
            .Include(nikoTypeRecoveryDbResponse => nikoTypeRecoveryDbResponse.Abilities)
            .FirstOrDefaultAsync(db => db.Id == id);
        if (result == null) return NotFound();

        return Ok(JsonSerializer.Serialize(NikoTypeRecoveryDbResponse.FromRecoveryDb(result), _options));
    }

    /// <summary>
    ///     This function creates a zip for the whole backup.
    /// </summary>
    /// <returns>Zip file.</returns>
    [HttpGet("zip")]
    [OutputCache(Duration = CacheTime)]
    public async Task<IActionResult> GetZip()
    {
        var archiveTime = DateTime.UtcNow;
        using var memoryStream = new MemoryStream();
        await using (var zipFile = new ZipArchive(memoryStream, ZipArchiveMode.Create, true, Encoding.UTF8))
        {
            zipFile.Comment = $"This is an archive of the NikoDex taken on {archiveTime}.";

            var result = await nikoDexRecoveryContext.NikoTypeRecoveryDb.Include(type => type.Abilities)
                .ToListAsync();

            foreach (var niko in result)
            {
                var basePath = niko.Name;

                var entry = zipFile.CreateEntry(Path.Combine(basePath, "data.json"));
                await using (var stream = await entry.OpenAsync())
                {
                    // Write to the specified stream with the JSON data of the Niko.
                    await using var jsonStream = new StreamWriter(stream);
                    await jsonStream.WriteAsync(
                        JsonSerializer.Serialize(NikoTypeRecoveryDbResponse.FromRecoveryDb(niko), _options));
                    await jsonStream.FlushAsync();
                }

                var fileData = await dexDataService.GetNikoImage(niko.Id);
                if (fileData == null)
                    continue;

                // Write to the specified image stream to save the image.
                var imageEntry = zipFile.CreateEntry(Path.Combine(basePath, "image.png"));

                await using (var stream = await imageEntry.OpenAsync())
                {
                    await using var imageStream = stream;
                    await imageStream.WriteAsync(fileData);
                    await imageStream.FlushAsync();
                }
            }
        }

        memoryStream.Position = 0;
        return File(memoryStream.ToArray(), "application/zip", $"dex-recovery-{archiveTime}.zip");
    }

    [HttpGet("image/{id:int}")]
    [OutputCache(Duration = CacheTime)]
    public async Task<IActionResult> GetNikoImage(int id)
    {
        // Retrieve the data of the file and return it as an image.
        var fileData = await dexDataService.GetNikoImage(id);
        if (fileData == null)
            return NotFound();

        return File(fileData, "image/png");
    }
}
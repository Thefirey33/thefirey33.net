using System.Net;
using Polly.CircuitBreaker;
using thefirey33_backend.Types.Database;
using thefirey33_backend.Types.Database.Context;
using thefirey33_backend.Types.Database.Dex;

namespace thefirey33_backend.Services;

public interface IDexDataService
{
    /// <summary>
    ///     Create a backup of the current dex instance.
    /// </summary>
    public Task CreateBackup();

    /// <summary>
    ///     Get the current image of the Nikosona.
    /// </summary>
    /// <param name="id">ID.</param>
    /// <returns>Data of the image.</returns>
    public Task<byte[]?> GetNikoImage(int id);
}

public class DexDataService(
    IWebHostEnvironment webHostEnvironment,
    IHttpClientFactory httpClientFactory,
    ILogger<DexDataService> logger,
    NikoDexRecoveryContext nikoDexRecoveryContext) : IDexDataService
{
    /// <summary>
    ///     The timespan between each backup.
    /// </summary>
    public const int HoursTimeSpan = 5;

    /**
    * The HTTP Client that will connect to the NikoDex API.
    */
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("NikoDexAPI");

    // The path where all the files of the server are stored.
    private string StoragePath
    {
        get
        {
            var path = Path.Combine(webHostEnvironment.IsDevelopment() ? webHostEnvironment.ContentRootPath : "data",
                "DexStorage");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }
    }

    /// <summary>
    ///     Get the image data of the Niko with the specified ID.
    /// </summary>
    /// <param name="id">ID of the Niko.</param>
    /// <returns>Image Data.</returns>
    public async Task<byte[]?> GetNikoImage(int id)
    {
        var filePath = Path.Combine(StoragePath, CreateNikoFileString(id));
        if (!File.Exists(filePath)) return null;

        var file = await File.ReadAllBytesAsync(filePath);

        return file;
    }

    /// <summary>
    ///     This will create a backup of the dex entirely.
    /// </summary>
    public async Task CreateBackup()
    {
        var lastElement = nikoDexRecoveryContext.NikoDexRecovery
            .OrderBy(type => type.Id)
            .LastOrDefault();

        if (lastElement != null)
        {
            var timeSpan = DateTime.UtcNow - lastElement.Date;

            // If the backup timespan is smaller than the specified days, do not take a backup!
            if (timeSpan.Days < HoursTimeSpan)
                return;
        }

        // Last API stability checks before the processing.
        var apiCheck = await _httpClient.GetAsync("ping");
        if (apiCheck.StatusCode != HttpStatusCode.OK)
        {
            logger.LogWarning("API is unhealthy! Skipping backup...");
            return;
        }

        // Finally, take all the data of the dex and start processing it.
        var dexData = await _httpClient.GetFromJsonAsync<List<NikoTypeRecoveryDb>>("data");
        if (dexData == null)
        {
            logger.LogWarning("NikoDex API returned null, skipping...");
            return;
        }

        Directory.Delete(StoragePath, true);

        // Create the NikoDex Image Downloading Tasks.
        var tasks = dexData.Select(DownloadNikoImage).ToList();

        // Send all the specified requests at once.
        // On failure, tell Polly to WAIT until the breaker re-opens so the Request can be re-sent.
        await Task.WhenAll(tasks);

        // Update all the Nikos in the list.
        if (lastElement != null)
        {
            lastElement.Nikos = dexData;
            lastElement.Date = DateTime.UtcNow;
            nikoDexRecoveryContext.NikoDexRecovery.Update(lastElement);
        }
        else
        {
            nikoDexRecoveryContext.NikoDexRecovery.Add(new NikoDexRecoveryDbType
            {
                Date = DateTime.UtcNow,
                Nikos = dexData
            });
        }

        await nikoDexRecoveryContext.SaveChangesAsync();
        logger.LogInformation("Successfully backed up {Date} instance of NikoDex.", DateTime.UtcNow);
    }


    private async Task DownloadNikoImage(NikoTypeRecoveryDb db)
    {
        try
        {
            var data = await _httpClient.GetByteArrayAsync($"image?id={db.Id}");

            // Fetch the specified image.
            var path = Path.Combine(StoragePath, CreateNikoFileString(db.Id));
            await File.WriteAllBytesAsync(path, data);
            db.ImagePath = path;
        }
        catch (BrokenCircuitException e)
        {
            // Retry after the circuit is open again.
            if (e.RetryAfter == null) logger.LogError("Retry failure! Couldn't execute.");
            else await Task.Delay(e.RetryAfter.Value.Milliseconds);
            await DownloadNikoImage(db);
        }
    }

    /// <summary>
    ///     Create the filename and extension for the Niko images.
    /// </summary>
    /// <param name="id">The ID of the niko.</param>
    /// <returns>The filename and extension. "niko-{id}.png"</returns>
    private static string CreateNikoFileString(int id)
    {
        return $"niko-{id}.png";
    }
}
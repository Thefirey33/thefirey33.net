using System.Net;
using Microsoft.EntityFrameworkCore;
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
    public const int DayTimeSpan = 15;


    /**
    * The HTTP Client that will connect to the NikoDex API.
    */
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("NikoDexAPI");

    // The path where all the files of the server are stored.
    private string StoragePath
    {
        get
        {
            var path = Path.Combine(webHostEnvironment.ContentRootPath, "DexStorage");

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
            if (timeSpan.Days < DayTimeSpan)
                return;
        }

        var apiCheck = await _httpClient.GetAsync("ping");
        if (apiCheck.StatusCode != HttpStatusCode.OK)
        {
            logger.LogWarning("API is unhealthy! Skipping backup...");
            return;
        }


        var dexData = await _httpClient.GetFromJsonAsync<List<NikoTypeRecoveryDb>>("data");

        if (dexData == null)
        {
            logger.LogWarning("NikoDex API returned null, skipping...");
            return;
        }

        var first = await nikoDexRecoveryContext.NikoDexRecovery.FirstOrDefaultAsync();

        // If a backup already exists, delete it.
        if (first != null)
        {
            nikoDexRecoveryContext.NikoDexRecovery.Remove(first);
            await nikoDexRecoveryContext.SaveChangesAsync();
        }

        // Delete all the images in the directory.
        Directory.Delete(StoragePath, true);

        // Fetch each image one by one from the NikoDex API to create a full backup.
        // After that, set the ImagePath.
        logger.LogInformation("Gettting ready to fetch all images...");
        foreach (var db in dexData)
        {
            var data = await _httpClient.GetByteArrayAsync($"image?id={db.Id}");

            // Fetch the specified image.
            var path = Path.Combine(StoragePath, CreateNikoFileString(db.Id));
            await File.WriteAllBytesAsync(path, data);
            db.ImagePath = path;
        }


        await nikoDexRecoveryContext.NikoDexRecovery.AddAsync(new NikoDexRecoveryDbType
        {
            Date = DateTime.UtcNow,
            Nikos = dexData
        });

        // Save the changes to the database async.
        await nikoDexRecoveryContext.SaveChangesAsync();

        logger.LogInformation("Successfully backed up {Date} instance of NikoDex.", DateTime.UtcNow);
    }

    /// <summary>
    ///     Create the filename and extension for the Niko images.
    /// </summary>
    /// <param name="id">The ID of the niko.</param>
    /// <returns>The filename and extension. "niko-{id}.png"</returns>
    public static string CreateNikoFileString(int id)
    {
        return $"niko-{id}.png";
    }
}
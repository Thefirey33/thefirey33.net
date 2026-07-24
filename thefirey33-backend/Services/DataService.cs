namespace thefirey33_backend.Services;

/// <summary>
///     This service stores all the files via a random UUID string.
/// </summary>
/// <param name="webHostEnvironment">The current environment of the server.</param>
public class DataService(IWebHostEnvironment webHostEnvironment)
{
    // The path where all the files of the server are stored.
    public string StoragePath
    {
        get
        {
            var path = Path.Combine(webHostEnvironment.ContentRootPath, "ServerStorage");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }
    }

    /// <summary>
    ///     Attempt to read the specified file.
    /// </summary>
    /// <param name="path">The path of the file to read.</param>
    /// <returns>Readed bytes.</returns>
    /// <exception cref="FileNotFoundException">When the file isn't found in the storage.</exception>
    public async Task<byte[]> ReadBytes(string path)
    {
        if (!File.Exists(path)) throw new FileNotFoundException("The specified file wasn't found!");
        return await File.ReadAllBytesAsync(path);
    }

    /// <summary>
    ///     Attempts to delete the file in the specified path.
    /// </summary>
    /// <param name="path">THe path of the file to delete.</param>
    /// <returns>Returns TRUE on deletion, Returns FALSE on failure.</returns>
    public bool DeleteFile(string path)
    {
        if (!File.Exists(path)) return false;
        File.Delete(path);
        return true;
    }

    /// <summary>
    ///     This writes some data a file, which is saved with a Guid ID.
    /// </summary>
    /// <param name="pathExtension">The path extension that the file has.</param>
    /// <param name="content">The content of the file in bytes.</param>
    public async Task<(string, string)> WriteBytes(string pathExtension, byte[] content)
    {
        var guid = GetUuidString();
        var fileWritePath = Path.Combine(StoragePath, Path.ChangeExtension(guid, pathExtension));
        // Write to the specified file.
        await File.WriteAllBytesAsync(fileWritePath, content);
        return (fileWritePath, guid);
    }

    /// <summary>
    ///     This creates a new UUID string.
    /// </summary>
    /// <returns>GUID/UUID</returns>
    private static string GetUuidString()
    {
        var uuid = Guid.NewGuid();
        return uuid.ToString();
    }
}
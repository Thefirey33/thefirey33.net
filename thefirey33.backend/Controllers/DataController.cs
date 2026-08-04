using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.StaticFiles;
using thefirey33_backend.Services;
using Path = System.IO.Path;

namespace thefirey33_backend.Controllers;

[ApiController]
[Route("[controller]")]
public class DataController(
    DataService dataService,
    IWebHostEnvironment webHostEnvironment,
    ILogger<DataController> logger) : ControllerBase
{
    /// <summary>
    ///     The image for the art watermark.
    /// </summary>
    private string WatermarkImage =>
        Path.Combine(webHostEnvironment.ContentRootPath, "Images", "Disclaimer.png");

    /// <summary>
    ///     Attempt to get a file by its specified uuid.
    /// </summary>
    /// <param name="uuid">The uuid of the file.</param>
    /// <param name="protected">If the image is protected or not?</param>
    [HttpGet("{uuid}")]
    [OutputCache(Duration = 24 * 3600)]
    public async Task<IActionResult> Get(string uuid, [FromQuery(Name = "pr")] bool @protected = false)
    {
        var file = Directory.GetFiles(dataService.StoragePath)
            .FirstOrDefault(f => Path.GetFileNameWithoutExtension(f).Contains(uuid));

        if (file == null) return NotFound();
        var fileExtension = new FileExtensionContentTypeProvider();

        if (!fileExtension.TryGetContentType(file, out var contentType)) return StatusCode(500);

        // If the file is an image, then do some processing.
        var byteData = await DataService.ReadBytes(file);

        return File(byteData, contentType);
    }
}
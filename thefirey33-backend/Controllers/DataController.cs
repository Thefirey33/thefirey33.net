using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.StaticFiles;
using SkiaSharp;
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
    ///     The color of the watermark image.
    /// </summary>
    private readonly SKPaint _skWatermarkOverlayPaint = new()
    {
        Color = SKColors.White.WithAlpha(30)
    };

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
        // If the user is unauthorized, do not allow it to show the unprotected image.
        var userIdentity = HttpContext.User.Identity;
        if (userIdentity is { IsAuthenticated: false } && !@protected)
            return Unauthorized();

        var file = Directory.GetFiles(dataService.StoragePath)
            .FirstOrDefault(f => Path.GetFileNameWithoutExtension(f).Contains(uuid));

        if (file == null) return NotFound();
        var fileExtension = new FileExtensionContentTypeProvider();

        if (!fileExtension.TryGetContentType(file, out var contentType)) return StatusCode(500);

        // If the file is an image, then do some processing.
        var byteData = await DataService.ReadBytes(file);
        var isImageFile = contentType.StartsWith("image");

        if (!isImageFile || !@protected)
            return File(byteData, contentType);

        logger.LogInformation("Processing image with watermark...");

        // Process the image with the watermark.
        using var skBitmap = SKBitmap.Decode(byteData);
        using var skCanvas = new SKCanvas(skBitmap);
        using var skWatermarkBitmap = SKBitmap.Decode(WatermarkImage);
        using var skWatermarkImage = SKImage.FromBitmap(skWatermarkBitmap);

        skCanvas.DrawImage(skWatermarkImage,
            new SKRect(0, 0, skBitmap.Width,
                skBitmap.Height / (skWatermarkBitmap.Height * 3.0f) * skWatermarkImage.Height),
            new SKSamplingOptions(SKFilterMode.Nearest), _skWatermarkOverlayPaint);
        skCanvas.Flush();

        using var imgData = SKImage.FromBitmap(skBitmap);
        using var skData = imgData.Encode(SKEncodedImageFormat.Png, 0);

        // After the processing of the image is done, send the data.
        return File(skData.ToArray(), contentType);
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using thefirey33_backend.Services;
using thefirey33_backend.Types.Database;
using thefirey33_backend.Types.Database.Context;
using thefirey33_backend.Types.Database.Request;
using thefirey33_backend.Types.Database.Response;

namespace thefirey33_backend.Controllers;

[ApiController]
[Route("[controller]")]
public class ArtController(ArtsContext artsContext, DataService dataService) : ControllerBase
{
    /// <summary>
    ///     Get all the arts in the database.
    /// </summary>
    [HttpGet]
    [OutputCache(Duration = 60)]
    public async Task<IActionResult> GetAll()
    {
        var result = await artsContext.Arts.ToListAsync();
        // Mask the filepath so it's not accidentally sent.
        return Ok(result.Select(type => new ArtDbResponse
        {
            Description = type.Description,
            Id = type.Id,
            Author = type.Author,
            Category = type.Category,
            Title = type.Title,
            Uuid = type.Uuid
        }));
    }

    /// <summary>
    ///     Get the categories that the API contains.
    /// </summary>
    [HttpGet("categories")]
    [OutputCache(Duration = 60)]
    public async Task<IActionResult> GetCategories()
    {
        var resultInitial = await artsContext.Arts
            .Select(type => type.Category).ToListAsync();

        var filteredHashSet = new HashSet<string>();
        resultInitial.ForEach(s =>
        {
            if (s != null)
                filteredHashSet.Add(s);
        });

        return Ok(filteredHashSet);
    }

    /// <summary>
    ///     Deletes the specified art with an ID.
    /// </summary>
    /// <param name="id">ID.</param>
    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await artsContext.Arts.FirstOrDefaultAsync(x => x.Id == id);
        if (result == null)
            return NotFound();

        artsContext.Arts.Remove(result);
        DataService.DeleteFile(result.FilePath);

        await artsContext.SaveChangesAsync();
        return Ok();
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> PostArt([FromForm] ArtDbRequest artDbRequest,
        IFormFile file)
    {
        if (!file.ContentType.StartsWith("image")) return BadRequest("Only image files are supported!");

        var stream = file.OpenReadStream();

        // Read all the content of the file and save it to PostgresSQL.
        using var fileBytes = new MemoryStream();
        await stream.CopyToAsync(fileBytes);

        var fileType = $".{file.ContentType[(file.ContentType.LastIndexOf('/') + 1)..]}";

        var filePath = await dataService.WriteBytes(fileType, fileBytes.ToArray());
        await artsContext.Arts.AddAsync(new ArtDbType
        {
            Uuid = filePath.Item2,
            Author = artDbRequest.Author,
            Title = artDbRequest.Title,
            Description = artDbRequest.Description,
            Category = artDbRequest.Category,
            FilePath = filePath.Item1
        });

        await artsContext.SaveChangesAsync();
        return Ok();
    }

    /// <summary>
    ///     Get arts via the category.
    /// </summary>
    /// <param name="category">The category to reference.</param>
    /// <returns>All the arts filtered by the category.</returns>
    [HttpGet("category/{category}")]
    public async Task<IActionResult> GetArtViaCategory(string category)
    {
        var objects = await artsContext.Arts.Where(type => type.Category == category).ToListAsync();
        return Ok(objects);
    }

    [HttpGet("{uuid}")]
    [OutputCache(Duration = 60)]
    public async Task<IActionResult> GetArtViaUuid(string uuid)
    {
        var obj = await artsContext.Arts.FirstOrDefaultAsync(x => x.Uuid == uuid);
        if (obj == null)
            return NotFound();

        // Mask the filepath to somewhere else.
        return Ok(new ArtDbResponse
        {
            Description = obj.Description,
            Uuid = obj.Uuid,
            Author = obj.Author,
            Id = obj.Id,
            Category = obj.Category,
            Title = obj.Title
        });
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using thefirey33_backend.Services;
using thefirey33_backend.Types.Database;
using thefirey33_backend.Types.Database.Context;
using thefirey33_backend.Types.Database.Request;
using thefirey33_backend.Types.Database.Response;

namespace thefirey33_backend.Controllers;

[ApiController]
[Route("[controller]")]
public class QuestionController(
    QuestionContext questionContext,
    IHttpClientFactory clientFactory,
    ILogger<QuestionController> logger,
    DataService dataService) : ControllerBase
{
    /// <summary>
    ///     The amount of time that the website will not allow any posts for.
    /// </summary>
    private const int RatelimitingSeconds = 20;

    /// <summary>
    ///     The client for the filtering service's HTTP requests.
    /// </summary>
    private readonly HttpClient _filteringServiceClient = clientFactory.CreateClient("FilteringServiceAPI");

    /// <summary>
    ///     Get all the current questions that aren't pending.
    /// </summary>
    /// <returns>All questions that aren't pending.</returns>
    [HttpGet]
    public async Task<List<QuestionDbType>> GetAllAvailable()
    {
        return await questionContext
            .Questions
            .Where(predicate => predicate.Response != null)
            .ToListAsync();
    }


    /// <summary>
    ///     Get all the questions.
    /// </summary>
    /// <returns>All questions.</returns>
    [Authorize]
    [HttpGet("all")]
    public async Task<List<QuestionDbType>> GetAll()
    {
        return await questionContext.Questions
            .OrderBy(db => db.Id)
            .ToListAsync();
    }

    /// <summary>
    ///     Attempt to post a question to the website.
    /// </summary>
    /// <param name="questionDbRequest">The QuestionDbRequest that has the specified question in it.</param>
    /// <param name="file">The file that users can attach.</param>
    [HttpPost]
    public async Task<IActionResult> Post([FromForm] QuestionDbRequest questionDbRequest, IFormFile? file)
    {
        var firstElement = await questionContext.Questions
            .OrderBy(desc => desc.QuestionPostTime)
            .LastOrDefaultAsync();

        // Some people really want to spam the shit out of your database and ruin everything.
        // This is why ratelimits exist!
        if (firstElement != null && DateTime.UtcNow < firstElement.QuestionPostTime.AddSeconds(RatelimitingSeconds))
            return StatusCode(
                StatusCodes.Status429TooManyRequests,
                new QuestionPostResponse
                {
                    Message = "Posting grace period, Try again later!",
                    Success = false
                });

        var formData = new MultipartFormDataContent();
        formData.Add(new StringContent(questionDbRequest.Question), "description");

        var stream = file?.OpenReadStream();
        byte[]? fileBytes = null;

        if (stream != null && file != null)
        {
            using var fileStream = new MemoryStream();
            await stream.CopyToAsync(fileStream);

            // Add the specified file to the form content.
            fileBytes = fileStream.ToArray();
            formData.Add(new ByteArrayContent(fileBytes), "file", file.FileName);
        }

        var contentCheck = await _filteringServiceClient.PostAsync("content_check", formData);

        // Ensure the service is operational before continuing.
        if (!contentCheck.IsSuccessStatusCode)
            return BadRequest(new QuestionPostResponse
            {
                Message = "This is not an image file!",
                Success = false
            });

        var contentCheckResult = await contentCheck.Content.ReadFromJsonAsync<bool>();
        if (contentCheckResult)
            return Ok(new QuestionPostResponse
            {
                Message = "This content was flagged!",
                Success = false
            });

        string? attachment = null;

        // If the file provided isn't null,
        // Basically, an attachment WAS provided,
        // Then upload the file.

        if (file != null && fileBytes != null)
        {
            // Write the bytes to the file.
            var fileType = $".{file.ContentType[(file.ContentType.LastIndexOf('/') + 1)..]}";
            var result = await dataService.WriteBytes(fileType, fileBytes);

            attachment = result.Item2;
        }

        await questionContext.Questions.AddAsync(new QuestionDbType
        {
            Question = questionDbRequest.Question,
            QuestionPostTime = DateTime.UtcNow,
            Attachment = attachment,
            AuthorName = questionDbRequest.AuthorName,
            UserId = questionDbRequest.UserId
        });
        await questionContext.SaveChangesAsync();

        return Ok(new QuestionPostResponse
        {
            Message = "Successfully posted question",
            Success = true
        });
    }

    /// <summary>
    ///     This allows to edit the response to a question.
    /// </summary>
    /// <param name="id">The ID of the question.</param>
    /// <param name="response">The response.</param>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, [FromQuery] string response)
    {
        var element = await questionContext.Questions.FirstOrDefaultAsync(db => db.Id == id);

        if (element == null)
            return NotFound();

        element.Response = response;
        questionContext.Questions.Update(element);
        await questionContext.SaveChangesAsync();

        return Ok();
    }

    /// <summary>
    ///     This deletes a question.
    /// </summary>
    /// <param name="id">The ID of the question.</param>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var element = await questionContext.Questions.FirstOrDefaultAsync(db => db.Id == id);
        if (element == null)
            return NotFound();

        // Delete the specified attachment if it exists.
        if (element.Attachment != null)
            dataService.DeleteFileUuid(element.Attachment);

        questionContext.Questions.Remove(element);
        await questionContext.SaveChangesAsync();
        return Ok();
    }
}
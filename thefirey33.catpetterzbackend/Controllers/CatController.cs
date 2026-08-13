using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using thefirey33.catpetterzBackend.Types.Database;

namespace thefirey33.catpetterzBackend.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class CatController(CatPetterzDbContext catPetterzDbContext) : ControllerBase
{
    /// <summary>
    ///     Check if the user has any cats.
    ///     This is for the beginning flow of the game.
    /// </summary>
    /// <returns>If the current authorized user has any cats.</returns>
    [HttpGet("any")]
    [Authorize]
    public async Task<IActionResult> CheckUserAnyCats()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
            return NotFound();

        var seqContainsAny = await catPetterzDbContext.Cats.AnyAsync(db => db.OwnerUserId == userIdClaim.Value);
        return seqContainsAny ? Ok() : NotFound();
    }
}
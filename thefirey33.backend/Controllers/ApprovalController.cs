using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using thefirey33_backend.Types.Database;
using thefirey33_backend.Types.Database.Context;
using thefirey33_backend.Types.Database.Request;
using thefirey33_backend.Types.Database.Response;

namespace thefirey33_backend.Controllers;

[ApiController]
[Route("[controller]")]
public class ApprovalController(ApprovalContext approvalContext) : ControllerBase
{
    /// <summary>
    ///     This will insert an approval request to the Approval Database.
    /// </summary>
    /// <param name="request">The ApprovalDb Request.</param>
    [HttpPost]
    public async Task<IActionResult> InsertApproval([FromBody] ApprovalDbRequest request)
    {
        if (await approvalContext.Approvals.AnyAsync(x => x.Uuid == request.Uuid))
            return StatusCode(StatusCodes.Status304NotModified);

        await approvalContext.Approvals.AddAsync(new ApprovalDbType
        {
            Approved = request.Approved,
            Uuid = request.Uuid,
            Username = request.Username
        });

        await approvalContext.SaveChangesAsync();
        return Ok();
    }

    [HttpPut("{uuid}")]
    [Authorize]
    public async Task<IActionResult> UpdateApproval(string uuid, [FromQuery] bool approved)
    {
        var result = await approvalContext.Approvals.FirstOrDefaultAsync(type => type.Uuid == uuid);
        if (result == null)
            return NotFound();

        // Change the approved state
        result.Approved = approved;
        approvalContext.Approvals.Update(result);

        await approvalContext.SaveChangesAsync();
        return Ok();
    }

    /// <summary>
    ///     Get all the approvals waiting in line.
    /// </summary>
    [HttpGet]
    [Authorize]
    public async Task<List<ApprovalDbType>> ListApprovals()
    {
        var result = await approvalContext.Approvals
            .OrderBy(db => db.Id)
            .ToListAsync();
        return result;
    }


    [HttpGet("{uuid}")]
    public async Task<ApprovalDbResponse> IsApproved(string uuid)
    {
        var result = await approvalContext.Approvals.AnyAsync(type => type.Uuid == uuid && type.Approved);

        return new ApprovalDbResponse
        {
            IsApproved = result
        };
    }
}
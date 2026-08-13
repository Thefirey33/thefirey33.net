using System.Net.WebSockets;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using thefirey33.catpetterzBackend.Service;
using thefirey33.catpetterzBackend.Types.Database;

namespace thefirey33.catpetterzBackend.WebSockets;

public class CatPetterzWebSocketGateway(CatPetterzDbContext catPetterzDbContext) : ControllerBase
{
    [Route("/updategateway")]
    [Authorize]
    public async Task HandleWebSocketConnection()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var websocketConnection = await HttpContext.WebSockets.AcceptWebSocketAsync();
            var periodicTimer = new PeriodicTimer(UpdateCatStateService.CatStatsUpdateTime);
            var identity = User.FindFirst(ClaimTypes.NameIdentifier);

            if (identity == null)
            {
                await websocketConnection.CloseAsync(WebSocketCloseStatus.PolicyViolation, "No Claim Found",
                    CancellationToken.None);
                return;
            }

            while (websocketConnection.State == WebSocketState.Open && await periodicTimer.WaitForNextTickAsync())
            {
                if (websocketConnection.State == WebSocketState.CloseReceived)
                {
                    await websocketConnection.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing",
                        CancellationToken.None);
                    break;
                }

                // Send the specified data down the websocket client connection.
                // This system sends the updated state of each cat every specified amount in the timespan.

                var cats =
                    await catPetterzDbContext.Cats.Where(cat => cat.OwnerUserId == identity.Value)
                        .ToListAsync();

                // Do not send useless data to the clients,
                // For example, completely empty data that they clearly don't need.
                if (cats.Count <= 0) continue;

                var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(cats));
                var segment = new ArraySegment<byte>(bytes, 0, bytes.Length);

                // Send the data with the new updated cat data.
                await websocketConnection.SendAsync(
                    segment,
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None);
            }
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }
}
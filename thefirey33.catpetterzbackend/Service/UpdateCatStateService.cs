using Microsoft.EntityFrameworkCore;
using thefirey33.catpetterzBackend.Types.Database;

namespace thefirey33.catpetterzBackend.Service;

public class UpdateCatStateService(
    IServiceProvider serviceProvider,
    ILogger<UpdateCatStateService> logger)
    : BackgroundService
{
    /// <summary>
    ///     The amount of hunger that the cat will get applied with.
    /// </summary>
    private const float HungerPenalty = 1.0f;

    /// <summary>
    ///     The amount of thirst that the cat will get applied with.
    /// </summary>
    private const float ThirstPenalty = 2.0f;

    /// <summary>
    ///     The amount of maximum thirst/hunger penalty.
    /// </summary>
    public const float MaximumPenaltySize = 1000.0f;


    /// <summary>
    ///     The amount of time that it will update.
    ///     Basically the stats of every cat.
    ///     Along with hunger and thirst increases.
    /// </summary>
    public static readonly TimeSpan CatStatsUpdateTime = TimeSpan.FromSeconds(10);


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var periodicTimer = new PeriodicTimer(CatStatsUpdateTime);

        while (!stoppingToken.IsCancellationRequested && await periodicTimer.WaitForNextTickAsync(stoppingToken))
        {
            using var scope = serviceProvider.CreateScope();
            var catpetterzDbContext = scope.ServiceProvider.GetRequiredService<CatPetterzDbContext>();
            await catpetterzDbContext.Cats.ForEachAsync(async void (cat) =>
            {
                try
                {
                    // The hunger and other penalties are added every timespan.
                    // This makes the user take care of the specified cats, and also reminding them if they died or not.

                    cat.Hunger += HungerPenalty;
                    cat.Thirst += ThirstPenalty;

                    // Unfortunately, if they don't take care of their cats well...
                    // Well.
                    // The Grim Reaper catches up with them.
                    if (cat.Hunger > MaximumPenaltySize || cat.Thirst > MaximumPenaltySize)
                        cat.TheCatWentOnSomeAdventures = true; // (death)

                    await catpetterzDbContext.SaveChangesAsync(stoppingToken);
                }
                catch (Exception e)
                {
                    logger.LogError("Couldn't update Cat state! Error: {Error}", e.Message);
                }
            }, stoppingToken);
        }
    }
}
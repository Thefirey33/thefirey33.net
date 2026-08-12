namespace thefirey33_backend.Services;

public class NikoDexRecoveryService(
    ILogger<NikoDexRecoveryService> logger,
    IServiceProvider serviceProvider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var periodicTimer = new PeriodicTimer(TimeSpan.FromHours(DexDataService.HoursTimeSpan));
        using var scope = serviceProvider.CreateScope();
        var nikoDexRecoveryContext = scope.ServiceProvider.GetService<IDexDataService>();

        // If the scope wasn't able to get created, stop the service.
        if (nikoDexRecoveryContext == null)
        {
            logger.LogCritical("Couldn't create scope for NikoDex recovery service, halting service!");
            return;
        }

        // Check if a backup can be created immediately.
        await nikoDexRecoveryContext.CreateBackup();

        while (!stoppingToken.IsCancellationRequested && await periodicTimer.WaitForNextTickAsync(stoppingToken))
            await nikoDexRecoveryContext.CreateBackup();
    }
}
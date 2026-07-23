package net.thefirey33.thefirey33Fireserver;

import net.thefirey33.thefirey33Fireserver.handlers.LagCleanupHandler;
import net.thefirey33.thefirey33Fireserver.handlers.ServerStatusHandler;
import net.thefirey33.thefirey33Fireserver.server.ApiServer;
import org.bukkit.plugin.java.JavaPlugin;
import org.bukkit.scheduler.BukkitScheduler;

import java.util.concurrent.TimeUnit;

public final class Thefirey33Fireserver extends JavaPlugin {

    /**
     * The time interval between triggers.
     */
    public static final long MinutesDuration = 10;

    /**
     * This is for code that triggers every hour.
     */
    private final long MinutesTicks = TimeUnit.MINUTES.toSeconds(MinutesDuration) * 20;


    @Override
    public void onEnable() {
        // Register the handlers of the plugin.
        // These include the cleanup, status handlers that report the status of the server.
        RegisterHandlers();
    }

    public void RegisterHandlers() {
        BukkitScheduler scheduler = this
                .getServer()
                .getScheduler();

        // Register the server cleanup handler, that cleans up left over items on the floor.
        scheduler.runTaskTimer(this, new LagCleanupHandler(this), 0, MinutesTicks);
        // Register the server status handler, which reports the status of the server.
        scheduler.runTaskTimer(this, new ServerStatusHandler(this), MinutesTicks, MinutesTicks);

        scheduler.runTaskAsynchronously(this, () -> {
            // Run the
            Thread.currentThread().setContextClassLoader(this.getClassLoader());
            ApiServer.Start(this, new String[]{"--server.port=%s".formatted(System.getenv("SPRINGBOOT_PORT"))});
        });
    }
}

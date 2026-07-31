package net.firey.fireserver;

import net.firey.fireserver.apiserver.ApiServer;
import net.firey.fireserver.authorization.PlayerAuthorizedCheckingEvent;
import net.firey.fireserver.events.ChatMessageStatusEvent;
import net.firey.fireserver.events.ServerPingEvent;
import org.bukkit.Server;
import org.bukkit.plugin.PluginManager;
import org.bukkit.plugin.java.JavaPlugin;
import org.bukkit.scheduler.BukkitScheduler;

public final class Fireserver extends JavaPlugin {

    @Override
    public void onEnable() {
        Server server = this.getServer();
        BukkitScheduler scheduler = server.getScheduler();
        PluginManager pluginManager = server.getPluginManager();

        // Register the event listeners.
        pluginManager.registerEvents(new ServerPingEvent(), this);
        pluginManager.registerEvents(new PlayerAuthorizedCheckingEvent(), this);
        pluginManager.registerEvents(new ChatMessageStatusEvent(), this);

        // Run the task timer that displays the current status of the server.
        scheduler.runTaskTimer(this, () -> server.getOnlinePlayers().forEach(ServerDisplayStatsHandler::DisplayStats), 0, 20);
        scheduler.runTaskAsynchronously(this, new ApiServer(this));
    }
}

package net.firey.fireserver;

import net.firey.fireserver.events.PlayerListener;
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

        pluginManager.registerEvents(new PlayerListener(), this);
        pluginManager.registerEvents(new ServerPingEvent(), this);
        
        scheduler.runTaskTimer(this, () -> {
            server.getOnlinePlayers().forEach(ServerDisplayStatsHandler::DisplayStats);
        }, 0, 20);
    }
}

package net.firey.fireserver.events;

import net.firey.fireserver.ServerMotdHandler;
import net.firey.fireserver.records.ServerMotdData;
import net.kyori.adventure.text.Component;
import org.bukkit.event.EventHandler;
import org.bukkit.event.Listener;
import org.bukkit.event.server.ServerListPingEvent;

public class ServerPingEvent implements Listener {

    /**
     * The server message of the day handler.
     */
    public static ServerMotdHandler serverMotdHandler = new ServerMotdHandler();


    @EventHandler
    public void onServerListPing(ServerListPingEvent event) {
        // Show a random icon for the game to display it.


        ServerMotdData randomMotd = serverMotdHandler.getRandomMotd();

        event.motd(Component.text(randomMotd.motdData()));
        event.setServerIcon(randomMotd.cachedServerIcon());
    }
}

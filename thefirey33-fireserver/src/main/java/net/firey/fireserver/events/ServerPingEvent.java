package net.firey.fireserver.events;

import net.kyori.adventure.text.Component;
import org.bukkit.event.EventHandler;
import org.bukkit.event.Listener;
import org.bukkit.event.server.ServerListPingEvent;

import java.lang.management.ManagementFactory;
import java.lang.management.RuntimeMXBean;
import java.util.concurrent.TimeUnit;

public class ServerPingEvent implements Listener {
    @EventHandler
    public void onServerListPing(ServerListPingEvent event) {
        RuntimeMXBean runtimeMXBean = ManagementFactory.getRuntimeMXBean();

        long uptimeMs = runtimeMXBean.getUptime();

        long hrs = TimeUnit.MILLISECONDS.toHours(uptimeMs);
        long mins = TimeUnit.MILLISECONDS.toMinutes(uptimeMs) % 60;
        long secs = TimeUnit.MILLISECONDS.toSeconds(uptimeMs) % 60;

        // Set the MOTD to the server uptime.
        event.motd(
                Component.text("the fireserver that's been running for ")
                        .append(Component.text("%02d:%02d:%02d".formatted(hrs, mins, secs))));
    }
}

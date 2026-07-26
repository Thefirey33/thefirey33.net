package net.firey.fireserver.events;

import net.firey.fireserver.ServerDisplayStatsHandler;
import net.kyori.adventure.text.Component;
import net.kyori.adventure.text.format.NamedTextColor;
import net.kyori.adventure.text.format.TextDecoration;
import org.bukkit.event.EventHandler;
import org.bukkit.event.Listener;
import org.bukkit.event.player.PlayerJoinEvent;

public class PlayerListener implements Listener {
    @EventHandler
    public void onPlayerJoin(PlayerJoinEvent event) {
        var player = event.getPlayer();

        // Send the join message to all players.
        event.joinMessage(
                player.displayName()
                        .appendSpace()
                        .append(Component.text("joined!"))
                        .appendNewline()
                        .append(Component.text("Remember: You can use TAB to view the current status of the server!").decoration(TextDecoration.BOLD, true).color(NamedTextColor.YELLOW))
        );

        ServerDisplayStatsHandler.DisplayStats(player);
    }
}

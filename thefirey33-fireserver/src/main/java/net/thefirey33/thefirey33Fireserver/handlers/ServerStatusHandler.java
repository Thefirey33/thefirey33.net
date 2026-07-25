package net.thefirey33.thefirey33Fireserver.handlers;

import net.kyori.adventure.text.Component;
import net.kyori.adventure.text.format.Style;
import net.kyori.adventure.text.format.TextColor;
import net.kyori.adventure.text.format.TextDecoration;
import net.thefirey33.thefirey33Fireserver.Thefirey33Fireserver;
import org.bukkit.Server;
import org.bukkit.entity.Player;

import java.util.Collection;

public class ServerStatusHandler extends BaseHandler {

    /**
     * The creation of this handler.
     *
     * @param fireServer the fireServer plugin.
     */
    public ServerStatusHandler(Thefirey33Fireserver fireServer) {
        super(fireServer);
    }

    @Override
    public void run() {
        Server server = fireServer.getServer();
        Runtime runtime = Runtime.getRuntime();

        long bytesToMb = 1024 * 1024;
        long totalMemoryUsage = (runtime.totalMemory() - runtime.freeMemory()) / bytesToMb;
        long totalMemory = runtime.totalMemory() / bytesToMb;

        Collection<? extends Player> onlinePlayers = server.getOnlinePlayers();
        onlinePlayers.forEach(player -> {
            player.sendPlayerListHeaderAndFooter(
                    Component.text("THEFIREY33 FIRESERVER")
                            .style(Style.style(TextColor.color(255, 0, 0))
                                    .decorate(TextDecoration.BOLD)),
                    Component.text("Server Memory Usage: %d/%dMB".formatted(totalMemoryUsage, totalMemory)));
        });
    }
}

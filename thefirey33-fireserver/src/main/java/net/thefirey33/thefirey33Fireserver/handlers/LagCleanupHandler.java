package net.thefirey33.thefirey33Fireserver.handlers;

import net.kyori.adventure.text.Component;
import net.kyori.adventure.text.format.Style;
import net.kyori.adventure.text.format.TextDecoration;
import net.thefirey33.thefirey33Fireserver.Thefirey33Fireserver;
import org.bukkit.Server;
import org.bukkit.entity.Entity;
import org.bukkit.entity.EntityType;

import java.util.List;

public class LagCleanupHandler extends BaseHandler {

    /**
     * The normal amount of time left before the countdown begins again.
     */
    private final int BaseTimeLeft = 6;

    /**
     * The current time left before the deletion is triggered.
     * It's every 10 minutes.
     */
    private int TimeLeft = BaseTimeLeft;

    /**
     * The creation of this handler.
     *
     * @param fireServer the fireServer plugin.
     */
    public LagCleanupHandler(Thefirey33Fireserver fireServer) {
        super(fireServer);
    }

    @Override
    public void run() {
        Server server = fireServer.getServer();

        // Send the message containing the time left before all the items are deleted.
        TimeLeft--;

        if (TimeLeft > 0) {
            server.sendMessage(Component.text("ATTENTION! %d minutes left before all items are deleted!".formatted(TimeLeft * Thefirey33Fireserver.MinutesDuration), Style.style(TextDecoration.BOLD)));
            return;
        }

        server.getWorlds().forEach(world -> {

            // Get all the items in the server.
            List<Entity> entities = world
                    .getEntities().stream()
                    .filter(entity -> entity.getType() == EntityType.ITEM).toList();

            entities.forEach(Entity::remove);

            // Alert the removed items.
            server.sendMessage(Component.text("ATTENTION! Removed %d from world: %s".formatted(entities.size(), world.getName()), Style.style(TextDecoration.BOLD)));
        });

        // Reset the time back to normal.
        TimeLeft = BaseTimeLeft;
    }
}

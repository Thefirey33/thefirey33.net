package net.firey.fireserver.events;

import io.papermc.paper.chat.ChatRenderer;
import io.papermc.paper.event.player.AsyncChatEvent;
import net.kyori.adventure.audience.Audience;
import net.kyori.adventure.text.Component;
import net.kyori.adventure.text.format.NamedTextColor;
import org.bukkit.entity.Player;
import org.bukkit.event.EventHandler;
import org.bukkit.event.Listener;
import org.jetbrains.annotations.NotNull;

import java.net.InetAddress;
import java.net.UnknownHostException;
import java.time.ZonedDateTime;
import java.time.format.DateTimeFormatter;

public class ChatMessageStatusEvent implements Listener, ChatRenderer {

    /**
     * The localhost data.
     */
    public static final InetAddress inetAddress;

    /**
     * systemd like formatter.
     */
    private static final DateTimeFormatter formatter = DateTimeFormatter.ofPattern("EEE yyyy-MM-dd HH:mm:ss");

    static {
        try {
            inetAddress = InetAddress.getLocalHost();
        } catch (UnknownHostException e) {
            throw new RuntimeException(e);
        }
    }

    @EventHandler
    public void onChat(AsyncChatEvent event) {
        event.renderer(this);
    }

    @Override
    public @NotNull Component render(@NotNull Player source, @NotNull Component sourceDisplayName, @NotNull Component message, @NotNull Audience viewer) {
        // Render the specified chat in this context.
        return Component
                .text("%s %s %s[%s]: ".formatted(
                                ZonedDateTime.now().format(formatter),
                                inetAddress.getHostName(),
                                source.getName().toLowerCase(),
                                source.getEntityId()
                        )
                ).color(source.isOp() ? NamedTextColor.RED : NamedTextColor.WHITE)
                .appendSpace()
                .append(message.color(NamedTextColor.GRAY));
    }
}

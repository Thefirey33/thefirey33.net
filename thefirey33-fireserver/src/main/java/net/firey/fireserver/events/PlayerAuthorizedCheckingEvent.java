package net.firey.fireserver.events;

import org.bukkit.entity.Player;
import org.bukkit.event.EventHandler;
import org.bukkit.event.Listener;
import org.bukkit.event.player.PlayerJoinEvent;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.util.Objects;

public class PlayerAuthorizedCheckingEvent implements Listener {
    public static final Logger LOGGER = LoggerFactory.getLogger(PlayerAuthorizedCheckingEvent.class.getName());

    @EventHandler
    public void onPlayerJoin(PlayerJoinEvent event) {
        Player player = event.getPlayer();

        LOGGER.info("Checking for operator privileges for user: {}", player.getUniqueId());
        player.setOp(Objects.equals(player.getUniqueId().toString(), System.getenv("TRUSTED_OPERATOR_UUID")));
    }
}

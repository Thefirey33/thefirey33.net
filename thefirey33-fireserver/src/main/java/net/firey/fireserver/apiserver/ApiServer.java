package net.firey.fireserver.apiserver;

import io.javalin.Javalin;
import io.javalin.websocket.WsMessageContext;
import net.firey.fireserver.Fireserver;
import net.firey.fireserver.apiserver.records.ServerInformation;
import org.apache.commons.lang3.math.NumberUtils;
import org.bukkit.Server;
import org.bukkit.entity.Player;

import java.lang.management.ManagementFactory;
import java.lang.management.RuntimeMXBean;
import java.util.HashSet;
import java.util.Set;
import java.util.function.Function;

public class ApiServer implements Runnable {

    private static final Set<WsMessageContext> context = new HashSet<>();
    /**
     * The fireserver plugin reference.
     */
    private final Fireserver fireserver;

    public ApiServer(Fireserver fireserver) {
        this.fireserver = fireserver;
    }

    @Override
    public void run() {
        String endPointVariable = System.getenv("SERVER_ENDPOINT");

        // The base port that is defined is 7000, but if a different one is provided, use that instead.
        int port = 7000;

        if (NumberUtils.isCreatable(endPointVariable)) {
            port = Integer.parseInt(endPointVariable);
        }

        Javalin.create(javalinConfig -> {
            RuntimeMXBean runtimeMXBean = ManagementFactory.getRuntimeMXBean();
            Server server = fireserver.getServer();

            javalinConfig.routes.get("/", ctx -> {
                // Create the specified server information for the website to display.

                ServerInformation serverInformation = new ServerInformation(
                        server.getOnlinePlayers().stream().map((Function<Player, String>) Player::getName).toList(),
                        runtimeMXBean.getUptime()
                );

                ctx.json(serverInformation);
            });
        }).start(port);
    }
}

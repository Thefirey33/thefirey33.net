package net.thefirey33.thefirey33Fireserver.handlers;

import net.kyori.adventure.text.Component;
import net.kyori.adventure.text.format.Style;
import net.kyori.adventure.text.format.TextDecoration;
import net.thefirey33.thefirey33Fireserver.Thefirey33Fireserver;
import org.bukkit.Server;

import java.lang.management.ManagementFactory;
import java.lang.management.OperatingSystemMXBean;

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
        OperatingSystemMXBean operatingSystemMXBean = ManagementFactory.getOperatingSystemMXBean();

        long bytesToMb = 1024 * 1024;
        long totalMemoryUsage = (runtime.totalMemory() - runtime.freeMemory()) / bytesToMb;
        long totalMemory = runtime.totalMemory() / bytesToMb;

        // Send the current OS information.
        server.sendMessage(Component.text("[%s %s]".formatted(operatingSystemMXBean.getName(), operatingSystemMXBean.getVersion()))
                .style(Style.style(TextDecoration.BOLD))
        );

        // Send the current memory status of the server.
        server.sendMessage(Component.text("Server Memory Usage: %d/%dMB".formatted(totalMemoryUsage, totalMemory)));

        // Send the current usage of the system for the server.
        double usageAverage = (operatingSystemMXBean.getSystemLoadAverage() / operatingSystemMXBean.getAvailableProcessors()) * 100.0f;
        server.sendMessage(Component.text("Server System Load: %.2f".formatted(usageAverage) + "%"));
    }
}

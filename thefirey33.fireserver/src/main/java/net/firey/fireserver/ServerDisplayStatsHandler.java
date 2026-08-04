package net.firey.fireserver;

import net.kyori.adventure.text.Component;
import net.kyori.adventure.text.format.NamedTextColor;
import net.kyori.adventure.text.format.Style;
import net.kyori.adventure.text.format.TextDecoration;
import org.bukkit.entity.Player;

import java.lang.management.ManagementFactory;
import java.lang.management.OperatingSystemMXBean;
import java.net.InetAddress;
import java.net.UnknownHostException;

public class ServerDisplayStatsHandler {

    /**
     * Bytes to Mb conversion.
     */
    public static final long BytesToMb = 1024 * 1024;

    /**
     * The localhost data.
     */
    public static final InetAddress inetAddress;

    /**
     * The data about the operating system.
     */
    public static final OperatingSystemMXBean operatingSystemMXBean = ManagementFactory.getOperatingSystemMXBean();

    /**
     * The current Java runtime.
     */
    public static final Runtime runtime = Runtime.getRuntime();

    static {
        try {
            inetAddress = InetAddress.getLocalHost();
        } catch (UnknownHostException e) {
            throw new RuntimeException(e);
        }
    }

    public static void DisplayStats(Player player) {

        long usedMemory = (runtime.totalMemory() - runtime.freeMemory()) / BytesToMb;
        long totalMemory = runtime.totalMemory() / BytesToMb;
        double averagePercent = (operatingSystemMXBean.getSystemLoadAverage() / operatingSystemMXBean.getAvailableProcessors()) * 100.0;

        player.sendPlayerListHeader(
                Component.text(inetAddress.getHostName())
                        .style(Style.style(TextDecoration.BOLD))
        );

        // This code has been broken into parts,
        // So it's easier to read.
        // It just displays information about the server.

        player.sendPlayerListFooter(
                Component.text("%sMB".formatted(usedMemory)).color(NamedTextColor.RED)
                        // Displays the current memory usage.
                        .append(Component.text("/").color(NamedTextColor.WHITE)
                                // Separator.
                                .append(Component.text(String.valueOf(totalMemory)).color(NamedTextColor.GREEN)
                                        // Displays the total memory.
                                        .append(Component.text("MB")))
                        )
                        .appendNewline()
                        .append(Component.text("System load: ").color(NamedTextColor.WHITE))
                        .append(Component.text("%.2f".formatted(averagePercent)).color(NamedTextColor.RED))
                        // Displays the current system load.
                        .append(Component.text("%").color(NamedTextColor.WHITE))
        );

    }
}

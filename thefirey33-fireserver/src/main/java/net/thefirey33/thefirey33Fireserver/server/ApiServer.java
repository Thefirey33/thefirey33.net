package net.thefirey33.thefirey33Fireserver.server;

import io.javalin.Javalin;
import net.thefirey33.thefirey33Fireserver.Thefirey33Fireserver;
import org.bukkit.Location;
import org.bukkit.Server;
import org.bukkit.World;
import org.bukkit.block.Block;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import javax.imageio.ImageIO;
import java.awt.image.BufferedImage;
import java.awt.image.DataBufferInt;
import java.io.ByteArrayOutputStream;
import java.util.Optional;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.stream.IntStream;

public class ApiServer {

    /**
     * The total amount of pixels in width will the map be.
     */
    private static final int Width = 1920;

    /**
     * The total amount of pixels in height will the map be.
     */
    private static final int Height = 1080;

    /**
     * The logger for the API server.
     */
    private static final Logger logger = LoggerFactory.getLogger(ApiServer.class);
    /**
     * This is the generated world map image.
     */
    private static BufferedImage bufferedImage;

    private static void CreateMap(World world) {
        logger.info("Creating map for world {}, this might take a while...", world.getName());

        bufferedImage = new BufferedImage(Width, Height, BufferedImage.TYPE_INT_RGB);
        int[] pixels = ((DataBufferInt) bufferedImage.getRaster().getDataBuffer()).getData();
        Location location = world.getSpawnLocation();

        // The map creation loop.
        // This allows the creation of the map in the world.
        ExecutorService executorService = Executors.newCachedThreadPool();

        // NOTE: Java ran out of threads, so remember to use this!
        executorService.submit(() -> IntStream.range(0, Width * Height)
                .parallel()
                .forEach(pos -> {
                    var x = pos % Width;
                    var z = pos / Height;

                    // This will individually get the highest block at the location and create the map accordingly.
                    Block block = world.getHighestBlockAt(location.getBlockX() + x, location.getBlockZ() + z);
                    pixels[pos] = block.getBlockData().getMapColor().asARGB();
                }));

        logger.info("Finished creating map!");
        executorService.shutdown();
    }

    public static void Start(Thefirey33Fireserver server, String[] args) {
        Server minecraftServer = server.getServer();

        Optional<World> result = minecraftServer.getWorlds()
                .stream()
                .filter(world -> world.getEnvironment() == World.Environment.NORMAL)
                .findFirst();

        if (result.isEmpty()) {
            throw new NullPointerException("There is no world with that name!");
        }

        // If a world is detected, execute the task.
        World world = result.get();
        CreateMap(world);

        Javalin.create(javalinConfig -> {

            // The amount of players in the server can be retrieved with this endpoint.
            javalinConfig.routes.get("/count", ctx -> {
                int count = minecraftServer.getOnlinePlayers().size();
                ctx.result(String.valueOf(count));
            });
            javalinConfig.routes.get("/chunks", ctx -> {
                ctx.contentType("image/png");

                try (ByteArrayOutputStream byteArrayOutputStream = new ByteArrayOutputStream()) {
                    ImageIO.write(bufferedImage, "png", byteArrayOutputStream);

                    ctx.result(byteArrayOutputStream.toByteArray());
                }
            });
        }).start(Integer.parseInt(System.getenv("SPRINGBOOT_PORT")));
    }
}

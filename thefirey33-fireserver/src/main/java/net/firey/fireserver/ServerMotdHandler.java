package net.firey.fireserver;


import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;
import com.google.gson.stream.JsonReader;
import net.firey.fireserver.records.ServerMotdData;
import net.firey.fireserver.records.ServerMotdLoadingData;
import org.bukkit.Bukkit;
import org.bukkit.util.CachedServerIcon;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.io.File;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.nio.file.Files;
import java.nio.file.StandardCopyOption;
import java.util.List;

public class ServerMotdHandler {

    private static final Logger LOGGER = LoggerFactory.getLogger(ServerMotdHandler.class);

    /**
     * The server motd data that is stored.
     */
    private static List<ServerMotdData> serverMotdData;

    public ServerMotdHandler() {
        ClassLoader classLoader = ServerMotdHandler.class.getClassLoader();
        try (InputStream inputStream = classLoader.getResourceAsStream("serverMotd.json")) {
            assert inputStream != null;
            Gson gson = new Gson();
            TypeToken<List<ServerMotdLoadingData>> typeToken = new TypeToken<>() {
            };

            // Each server icon and motd will be parsed, then converted into something that the ServerPingEvent can use.
            JsonReader jsonReader = new JsonReader(new InputStreamReader(inputStream));
            List<ServerMotdLoadingData> serverMotdDataList = gson.fromJson(jsonReader, typeToken);

            serverMotdData = serverMotdDataList.stream().map(serverMotdLoadingData -> {
                String s = serverMotdLoadingData.serverIconPath();
                InputStream resource = classLoader.getResourceAsStream(s);

                LOGGER.info("Loading Icon: {} for server message of the day usage.", s);

                if (resource == null) {
                    throw new NullPointerException(s);
                }

                try {
                    // Load the icon with a temporary file loader.

                    File file = File.createTempFile("serverIcon", ".png");
                    file.deleteOnExit();

                    Files.copy(resource, file.toPath(), StandardCopyOption.REPLACE_EXISTING);

                    CachedServerIcon cachedServerIcon = Bukkit.loadServerIcon(file);
                    return new ServerMotdData(serverMotdLoadingData.motdMessage(), cachedServerIcon);
                } catch (Exception e) {
                    throw new RuntimeException(e);
                }
            }).toList();

        } catch (IOException e) {
            throw new RuntimeException(e);
        }
    }

    public ServerMotdData getRandomMotd() {
        return serverMotdData.get((int) (Math.random() * serverMotdData.size()));
    }
}

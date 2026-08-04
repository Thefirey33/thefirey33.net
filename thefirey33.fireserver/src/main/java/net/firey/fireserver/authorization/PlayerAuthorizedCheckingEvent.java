package net.firey.fireserver.authorization;

import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;
import net.firey.fireserver.authorization.models.ApprovalRequest;
import net.firey.fireserver.authorization.models.ApprovalResponse;
import net.kyori.adventure.text.Component;
import org.bukkit.entity.Player;
import org.bukkit.event.EventHandler;
import org.bukkit.event.Listener;
import org.bukkit.event.player.PlayerJoinEvent;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.io.IOException;
import java.net.URI;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;
import java.util.Objects;

public class PlayerAuthorizedCheckingEvent implements Listener {

    public static final Logger LOGGER = LoggerFactory.getLogger(PlayerAuthorizedCheckingEvent.class.getName());
    private static final HttpClient httpClient = HttpClient.newHttpClient();

    private static final URI BackendUri = URI.create(System.getenv("FIREYBACKEND_API"));

    @EventHandler
    public void onPlayerJoin(PlayerJoinEvent event) {
        Player player = event.getPlayer();
        Gson gson = new Gson();

        TypeToken<ApprovalResponse> responseToken = new TypeToken<>() {
        };

        boolean isOperator = Objects.equals(player.getUniqueId().toString(), System.getenv("TRUSTED_OPERATOR_UUID"));
        // Firstly, set the Operator state of the user if it's equal to the trusted operator.
        LOGGER.info("Checking for operator privileges for user: {}", player.getUniqueId());
        player.setOp(isOperator);

        HttpRequest httpRequest = HttpRequest.newBuilder()
                .uri(BackendUri.resolve("/Approval/%s".formatted(player.getUniqueId().toString())))
                .GET()
                .build();

        try {
            HttpResponse<String> getResponse = httpClient.send(httpRequest, HttpResponse.BodyHandlers.ofString());
            LOGGER.info("Received approval response for user. [{}]", getResponse.body());
            ApprovalResponse response = gson.fromJson(getResponse.body(), responseToken.getType());

            if (response.is_approval())
                return;

            player.kick(Component
                    .text("You must be approved to join this server!")
                    .appendNewline()
                    .append(Component.text("Don't worry, your request to join has been sent. You will be able to join when your request is approved.")));

            TypeToken<ApprovalRequest> requestToken = new TypeToken<>() {
            };
            String payload = gson.toJson(new ApprovalRequest(player.getUniqueId().toString(), player.getName(), false), requestToken.getType());

            HttpRequest sendDatabaseData = HttpRequest.newBuilder()
                    .uri(BackendUri.resolve("/Approval"))
                    .header("Content-Type", "application/json")
                    .POST(HttpRequest.BodyPublishers.ofString(payload))
                    .build();

            HttpResponse<String> approvalRequest = httpClient.send(sendDatabaseData, HttpResponse.BodyHandlers.ofString());
            LOGGER.info("Send approval request for user. [{}]", approvalRequest.statusCode());
        } catch (IOException | InterruptedException e) {
            throw new RuntimeException(e);
        }


    }
}

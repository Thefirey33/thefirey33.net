package net.firey.fireserver.authorization;

import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;
import net.firey.fireserver.authorization.exception.UnauthorizedException;
import net.firey.fireserver.authorization.models.AuthorizationPayload;
import net.firey.fireserver.authorization.models.AuthorizedTokenResponse;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.io.IOException;
import java.net.HttpURLConnection;
import java.net.URI;
import java.net.URISyntaxException;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;

public class AuthorizationManager {

    /**
     * The admin username that will be used for the connection to the backend.
     */
    private static final String adminUsername = System.getenv("ADMIN_USERNAME");

    /**
     * The admin password that will be used for the connection to the backend.
     */
    private static final String adminPassword = System.getenv("ADMIN_PASSWORD");

    /**
     * The backend API url that the java plugin will make.
     */
    private static final URI backendUrl;

    /**
     * The HTTPClient that will make the requests.
     */
    private static final HttpClient client = HttpClient.newHttpClient();

    /**
     * For logging operations, this shall be used.
     */
    private static final Logger LOGGER = LoggerFactory.getLogger(AuthorizationManager.class);

    /**
     * The authorization token, that is the JWT token for the authorization.
     */
    private static String AuthorizationToken;

    static {
        try {
            backendUrl = new URI(System.getenv("FIREYBACKEND_API"));
        } catch (URISyntaxException e) {
            throw new RuntimeException(e);
        }
    }

    /**
     * Get the current authorization token.
     *
     * @return Token
     */
    public static String getAuthorizationToken() {
        return AuthorizationToken;
    }


    public void Authorize() {
        Gson gson = new Gson();
        TypeToken<AuthorizedTokenResponse> responseToken = new TypeToken<>() {
        };
        TypeToken<AuthorizationPayload> payloadToken = new TypeToken<>() {
        };
        AuthorizationPayload authorizationPayload = new AuthorizationPayload(adminUsername, adminPassword);
        String payload = gson.toJson(authorizationPayload, payloadToken.getType());

        HttpRequest request = HttpRequest.newBuilder()
                .uri(backendUrl.resolve("/Auth/login"))
                .header("Content-Type", "application/json; charset=utf-8")
                .header("Accept", "application/json")
                .POST(HttpRequest.BodyPublishers.ofString(payload))
                .build();

        try {
            HttpResponse<String> response = client.send(request, HttpResponse.BodyHandlers.ofString());
            LOGGER.info("Backend responded with {} authorization response!", response.statusCode());

            if (response.statusCode() != HttpURLConnection.HTTP_OK) {
                throw new UnauthorizedException("Backend denied authorization.");
            }

            AuthorizedTokenResponse tokenResponse = gson.fromJson(response.body(), responseToken);
            AuthorizationToken = tokenResponse.getToken();
        } catch (IOException | InterruptedException e) {
            throw new RuntimeException(e);
        }

    }
}

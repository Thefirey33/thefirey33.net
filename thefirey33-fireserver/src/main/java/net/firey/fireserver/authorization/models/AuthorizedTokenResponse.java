package net.firey.fireserver.authorization.models;

public class AuthorizedTokenResponse {
    String token;

    public AuthorizedTokenResponse(String token) {
        this.token = token;
    }

    public String getToken() {
        return token;
    }
}

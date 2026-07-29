package net.firey.fireserver.authorization.models;

public class AuthorizationPayload {
    String name;
    String password;

    public AuthorizationPayload(String name, String password) {
        this.name = name;
        this.password = password;
    }
}

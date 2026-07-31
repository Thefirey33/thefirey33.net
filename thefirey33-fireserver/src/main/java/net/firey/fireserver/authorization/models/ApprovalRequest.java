package net.firey.fireserver.authorization.models;

public record ApprovalRequest(String uuid, String username, boolean approved) {
}

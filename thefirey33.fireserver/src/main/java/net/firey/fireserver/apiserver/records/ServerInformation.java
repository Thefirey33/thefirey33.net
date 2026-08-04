package net.firey.fireserver.apiserver.records;

import java.util.List;

public record ServerInformation(List<String> currentPlayers, long serverUptime) {
}

#include <iostream>
#include <httplib.h>
#include <spdlog/spdlog.h>

#include "request_handler.h"
#include "routes_handler.h"


int main() {
    // Extremely simple old frontend for old computers.
    // This is made for fun only.

    httplib::Server srv{};
    routes_handler routes_handler{};
    request_handler request_handler{srv, routes_handler};

    // Mount static files to the directory.
    srv.set_mount_point("/", "./static");

    srv.set_logger([](const httplib::Request &req, const httplib::Response &res) {
        spdlog::info("[{} {}] {} -> {}", req.method, req.path, req.version, res.status);
    });

    // Check if the environment variable for the PORT was set, if not, throw a runtime exception.
    if (const char *environment_variable = std::getenv("PORT"); !environment_variable) {
        throw std::runtime_error("Environment variable not set");
    } else {
        const int port = std::stoi(environment_variable);
        srv.listen("0.0.0.0", port);
    }

    return 0;
}

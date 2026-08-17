#include "request_handler.h"
#include <nlohmann/json.hpp>
#include <fstream>

#include "spdlog/spdlog.h"

constexpr auto LAYOUT_FILE_LOCATION = "./html/layout.html";

request_handler::request_handler(httplib::Server &server, routes_handler &routes) : routes(routes), server(server),
    api_client(std::getenv("FIREYBACKEND_API")) {
    this->load_layout_data();


    // All of the general static routes are served here.
    // Compared to configured routes below, these are automatically handled via /routes.
    for (const std::string &route: routes.get_routes()) {
        std::stringstream ss;

        ss << "/";
        if (route != "main")
            ss << route;

        server.Get(ss.str(), [this, &routes, route](const httplib::Request &, httplib::Response &res) {
            this->configure_response(res, routes.get_route(route));
        });
    }

    server.Get("/arts", [this](const httplib::Request &, httplib::Response &res) {
        auto response = this->api_client.Get("/Art");
        spdlog::info("Requested Arts. Response {}", response->status);

        auto response_json = nlohmann::json::parse(response->body);
        std::string body{};

        for (const auto &json: response_json) {
            auto uuid = json["uuid"].get<std::string>();
            auto author = json["author"].get<std::string>();
            auto title = json["title"].get<std::string>();
            auto description = json["description"].get<std::string>();

            std::stringstream ss;

            // Piece together the HTML content for the art section.
            ss << "<center>" << std::endl; // Begin the centering section.

            ss << "<img src=\"" << "/data/" << uuid << "\"/>" << std::endl;
            ss << "<br/>" << std::endl;
            // The very cool seperator element. This website is designed to look like shit, so why not?

            ss << "<h2>" << title << "</h2>" << std::endl;
            ss << "<strong> By: " << author << "</strong>" << std::endl;
            ss << "<p>" << description << "</p>" << std::endl;

            ss << "</center>" << std::endl; // End the centering section.


            body.append(ss.str());
        }

        this->configure_response(res, body);
    });

    server.Get("/data/:uuid", [this](const httplib::Request &req, httplib::Response &res) {
        const auto uuid = req.path_params.at("uuid");

        std::stringstream ss;
        ss << "/Data/" << uuid;
        auto url = ss.str();

        auto response = this->api_client.Get(url);
        spdlog::info("Attempting to fetch image: {} -> {}", url, response->status);

        res.set_content(response->body, response->get_header_value("Content-Type"));
    });
}


void request_handler::configure_response(httplib::Response &res, const std::string &body) {
    res.set_header("Content-Type", "text/html");

    constexpr std::string_view body_tag = "@body";
    if (const std::size_t &data = this->layout_data.find(body_tag);
        data != std::string::npos) {
        auto response_content = this->layout_data;

        response_content.replace(data, body_tag.size(), body);
        res.set_content(response_content, "text/html");
    }
}


void request_handler::load_layout_data() {
    std::ifstream layout_file(LAYOUT_FILE_LOCATION);
    std::ostringstream ss;

    ss << layout_file.rdbuf();
    this->layout_data = ss.str();

    layout_file.close();
}

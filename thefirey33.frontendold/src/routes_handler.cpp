#include "routes_handler.h"

#include <filesystem>
#include <fstream>

constexpr auto ROUTES_DIRECTORY = "./html/routes";

routes_handler::routes_handler() {
    for (const auto &file: std::filesystem::directory_iterator(ROUTES_DIRECTORY)) {
        const auto &filepath = file.path();
        std::ifstream route_file_stream(filepath);

        std::stringstream ss;
        ss << route_file_stream.rdbuf();

        this->routes.insert(std::make_pair(filepath.stem(), ss.str()));
    }
}


bool routes_handler::route_exists(const std::string &identifier) {
    return this->routes.find(identifier) != this->routes.end();
}

std::string &routes_handler::get_route(const std::string &identifier) {
    return this->routes.at(identifier);
}


std::vector<std::string> routes_handler::get_routes() {
    std::vector<std::string> m_routes{};
    for (const auto &[fst, snd]: this->routes)
        m_routes.push_back(fst);

    return m_routes;
}

#ifndef OLD_WEB_ROUTES_HANDLER_H
#define OLD_WEB_ROUTES_HANDLER_H
#include <map>
#include <string>
#include <vector>


class routes_handler {
public:
    routes_handler();

    /**
     * Checks if a route/path exists.
     * @param identifier The path's identifier.
     * @return If the route exists.
     */
    bool route_exists(const std::string &identifier);

    /**
     * Get the data for the route.
     * @param identifier The path's identifier.
     * @return The route file's data.
     */
    std::string &get_route(const std::string &identifier);

    std::vector<std::string> get_routes();

private:
    /**
     * The current route files.
     * Each route file is saved as an std::string, since they are just string files.
     */
    std::map<std::string, std::string> routes{};
};


#endif //OLD_WEB_ROUTES_HANDLER_H

#ifndef OLD_WEB_BASE_WEBSITE_REQUEST_HANDLER_H
#define OLD_WEB_BASE_WEBSITE_REQUEST_HANDLER_H
#include "httplib.h"
#include "routes_handler.h"


class request_handler {
public:
    explicit request_handler(httplib::Server &server, routes_handler &routes);

private:
    /**
     * The layout.html's data.
     */
    std::string layout_data{};

    /**
     * The handler for the route files.
     */
    routes_handler &routes;

    /**
     * The current HTTP Server reference.
     */
    httplib::Server &server;

    /**
     * API HTTP Client communicator.
     */
    httplib::Client api_client;

    /**
     * Appends default layout content to the HTML response.
     * @param res Response reference.
     * @param body The body of the HTML response.
     */
    void configure_response(httplib::Response &res, const std::string &body);

    /**
     * Loads the specified layout.html file to memory.
     */
    void load_layout_data();
};
#endif //OLD_WEB_BASE_WEBSITE_REQUEST_HANDLER_H

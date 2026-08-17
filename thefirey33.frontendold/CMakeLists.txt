# This whole section is for fun.
# This is the "OLD" section of the website, managed by the cpp-httplib library.
# The whole purpose of it, is to allow extremely old retro browsers to display the Thefirey33 Website.

cmake_minimum_required(VERSION 3.10.6)
set(PROJECT_NAME old_web)
project(${PROJECT_NAME})

set(CMAKE_CXX_STANDARD 17)

set(WEB_SERVER_SOURCES
        src/main.cpp
        src/request_handler.cpp
        src/routes_handler.cpp
        include/routes_handler.h
)

# Add the specified libraries.

# The CPP HTTPLib. This is what manages the server.
add_subdirectory(submodules/cpp-httplib)

# The SPDLog library, this is for logging.
add_subdirectory(submodules/spdlog)

# JSON Library for parsing the HTTP Responses.
add_subdirectory(submodules/json)

add_executable(${PROJECT_NAME} ${WEB_SERVER_SOURCES})
target_link_libraries(${PROJECT_NAME} PRIVATE httplib::httplib spdlog::spdlog nlohmann_json::nlohmann_json)
target_include_directories(${PROJECT_NAME} PRIVATE include)
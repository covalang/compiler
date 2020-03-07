cmake_minimum_required(VERSION 3.10)

# set the project name
project(covac VERSION 0.0.1)

# specify the C++ standard
set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED True)
set(CMAKE_BUILD_TYPE "Debug")

# add the executable
add_executable(covac main.cpp ${CMAKE_SOURCE_DIR}/.antlr/CovaLexer.cpp ${CMAKE_SOURCE_DIR}/.antlr/CovaParser.cpp)

target_include_directories(covac PUBLIC "." ".antlr" "deps/antlr4-cpp-runtime-4.8-macos/antlr4-runtime")

target_link_libraries(covac ${CMAKE_SOURCE_DIR}/deps/antlr4-cpp-runtime-4.8-macos/lib/libantlr4-runtime.a)
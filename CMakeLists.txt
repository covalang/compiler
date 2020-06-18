cmake_minimum_required(VERSION 3.10)

# set the project name
project(covac VERSION 0.0.1)

# specify the C++ standard
set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED True)
set(CMAKE_BUILD_TYPE "Debug")

# add the executable
add_executable(covac main.cpp ${CMAKE_SOURCE_DIR}/.antlr/CovaLexer.cpp ${CMAKE_SOURCE_DIR}/.antlr/CovaParser.cpp)

target_include_directories(covac PUBLIC "." ".antlr")
target_include_directories(covac PUBLIC "deps/antlr4/4.8/include")

if (APPLE)
	target_link_libraries(covac "${CMAKE_SOURCE_DIR}/deps/antlr4/4.8/lib/macos/libantlr4-runtime.a")
endif()

if(WIN32)
	target_link_libraries(covac "${CMAKE_SOURCE_DIR}/deps/antlr4/4.8/lib/windows/clang_cl_x64/antlr4-runtime.lib")
endif()
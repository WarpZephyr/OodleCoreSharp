cmake_minimum_required(VERSION 3.16)
project(oodleinterop VERSION 1.0.0 LANGUAGES CXX)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

add_library(oodleinterop SHARED oodleinterop.cpp oodleinterop.h)

set_target_properties(oodleinterop PROPERTIES
    CXX_VISIBILITY_PRESET hidden
    VISIBILITY_INLINES_HIDDEN ON
)
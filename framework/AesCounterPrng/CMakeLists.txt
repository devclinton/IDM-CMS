cmake_minimum_required (VERSION 3.10)
project (PrngLib)
set(__OPTIMIZE__ TRUE)
SET(AESCOUNTERPRNG_EXPORTS 1)
if(NOT CMAKE_BUILD_TYPE)
    set(CMAKE_BUILD_TYPE Release)
endif()

set(CMAKE_CXX_FLAGS "-Wall -Wextra")

set(CMAKE_CXX_FLAGS_DEBUG "-O -msse4.1 -maes")
set(CMAKE_CXX_FLAGS_RELEASE "-O -msse4.1 -maes")
add_library(PrngLib SHARED
        AesCounterPrng.cpp
        ctrFunction.cpp
        PseudoDesPrng.cpp
        )
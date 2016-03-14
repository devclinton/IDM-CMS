#pragma once

#include "IRandom.h"

enum PRNG_TYPE {
    PRNG_EMODLCG  = 0,
    PRNG_EMODPDES = 1,
    PRNG_MT64     = 2,
    PRNG_SFMT     = 3,
    PRNG_AESCTR   = 4,
    PRNG_RANDLIB  = 5
};

class RandomFactory
{
public:
    static IRandom *CreatePRNG(PRNG_TYPE prngType, void *seedData, size_t cSeedBytes);
};
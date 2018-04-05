/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

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
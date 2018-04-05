/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

#include "stdafx.h"

#include "RandLib.h"
#include "randlib\ranlib.h"

RandLib *RandLib::CreatePRNG(void *seedData, size_t seedSize)
{
    RandLib *prng = nullptr;

    if (seedSize >= (2 * sizeof(uint32_t))) {
        uint32_t *pSeed = (uint32_t *)seedData;
        uint32_t seed1 = *pSeed++;
        uint32_t seed2 = *pSeed;
        prng = new RandLib(seed1, seed2);
    }
    else {
        throw "RandLib generator needs at least 64 bits of seed data.";
    }

    return prng;
}

RandLib::RandLib(uint32_t seed1, uint32_t seed2)
{
    Common::SetInitialSeed(seed1, seed2);
}

RandLib::~RandLib()
{
}

void RandLib::GetBits(void *buffer, size_t cBytes)
{
    if ((cBytes & 1) == 1) {
        throw "RandLib generates bits 16 at a time.";
    }

    uint16_t *dest = (uint16_t *)buffer;
    size_t count = cBytes / 2;
    for (size_t i = 0; i < count; i++) {
        *dest++ = (uint16_t)(Common::GenerateLargeInteger() >> 15);
    }
}

void RandLib::GetFloats(float *buffer, size_t cFloats)
{
    for (size_t i = 0; i < cFloats; i++) {
        *buffer++ = RNG::GenerateUniformFloat01();
    }
}

uint32_t RandLib::GetNext32()
{
    uint32_t bits;
    uint16_t *pBits = (uint16_t *)&bits;

    *pBits++ = (uint16_t)(Common::GenerateLargeInteger() >> 15);
    *pBits   = (uint16_t)(Common::GenerateLargeInteger() >> 15);

    return bits;
}

float RandLib::GetNextFloat()
{
    return RNG::GenerateUniformFloat01();
}

char *RandLib::GetName() { return "RandLib"; }

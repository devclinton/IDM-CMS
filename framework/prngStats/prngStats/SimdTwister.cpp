/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

#include "stdafx.h"

#include "SimdTwister.h"

SimdTwister *SimdTwister::CreatePRNG(void *seedData, size_t seedSize)
{
    SimdTwister *prng = nullptr;

    if (seedSize >= sizeof(uint32_t)) {
        prng = new SimdTwister((uint32_t *)seedData, seedSize / 4);
    }
    else {
        throw "SimdTwister needs one or more 32-bit seed values.";
    }

    return prng;
}

SimdTwister::SimdTwister(uint32_t *seedData, size_t seedCount)
{
    init_by_array(seedData, (int)seedCount);
}

SimdTwister::~SimdTwister()
{
}

void SimdTwister::GetBits(void *buffer, size_t cBytes)
{
    if (cBytes % 8 != 0) {
        throw "SimdTwister generates bits 64 at a time.";
    }

    fill_array64((uint64_t *)buffer, (int)(cBytes / 8));
}

void SimdTwister::GetFloats(float *buffer, size_t cFloats)
{
    if (cFloats % 2 != 0) {
        throw "SimdTwister generates floats 2 at a time.";
    }

    GetBits(buffer, cFloats * sizeof(float));
    size_t count    = cFloats / 2;
    float *pFloats = (float *)buffer;
    for (size_t i = 0; i < count; i++) {
        *pFloats++ = FloatFromBits(*(uint32_t*)pFloats);
        *pFloats++ = FloatFromBits(*(uint32_t*)pFloats);
    }
}

uint32_t SimdTwister::GetNext32()
{
    return gen_rand32();
}

float SimdTwister::GetNextFloat()
{
    return FloatFromBits(GetNext32());
}

char *SimdTwister::GetName() { return "SimdTwister"; }

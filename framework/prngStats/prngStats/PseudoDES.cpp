/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

#include "stdafx.h"

#include "PseudoDES.h"

PseudoDES *PseudoDES::CreatePRNG(void *seedData, size_t seedSize)
{
    PseudoDES *prng = nullptr;

    if (seedSize >= sizeof(uint32_t)) {
        uint32_t seed = *(uint32_t *)seedData;
        prng = new PseudoDES(seed);
    }
    else {
        throw "PseudoDES needs a 32-bit seed value.";
    }

    return prng;
}

PseudoDES::PseudoDES(uint32_t seed)
{
    pdes = new PSEUDO_DES(seed);
}

PseudoDES::~PseudoDES()
{
    delete pdes;
}

void PseudoDES::GetBits(void *buffer, size_t cBytes)
{
    if (cBytes % 4 != 0) {
        throw "Pseudo-DES generates bits 32 at a time.";
    }

    uint32_t *dest = (uint32_t *)buffer;
    size_t count   = cBytes / 4;
    for (size_t i = 0; i < count; i++) {
        *dest++ = pdes->ul();
    }
}

void PseudoDES::GetFloats(float *buffer, size_t cFloats)
{
    for (size_t i = 0; i < cFloats; i++) {
        *buffer++ = FloatFromBits(pdes->ul());
    }
}

uint32_t PseudoDES::GetNext32()
{
    return pdes->ul();
}

float PseudoDES::GetNextFloat()
{
    return FloatFromBits(pdes->ul());
}

char *PseudoDES::GetName() { return "PseudoDES"; }

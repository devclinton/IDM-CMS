#include "stdafx.h"

#include "LinearCongruential.h"

LinearCongruential *LinearCongruential::CreatePRNG(void *seedData, size_t seedSize)
{
    LinearCongruential *prng = nullptr;

    if (seedSize >= sizeof(uint32_t))
    {
        uint32_t seed = *(uint32_t *)seedData;
        prng = new LinearCongruential(seed);
    }

    return prng;
}

LinearCongruential::LinearCongruential(uint32_t seed)
{
    lcg = new RANDOM(seed);
}

LinearCongruential::~LinearCongruential()
{
    delete lcg;
}

void LinearCongruential::GetBits(void *buffer, size_t cBytes)
{
    if (cBytes % 4 != 0) {
        throw "LinearCongruential generates bits 32 at a time.";
    }

    uint32_t *dest = (uint32_t *)buffer;
    size_t count   = cBytes / 4;
    for (size_t i = 0; i < count; i++) {
        *dest++ = lcg->ul();
    }
}

void LinearCongruential::GetFloats(float *buffer, size_t cFloats)
{
    for (size_t i = 0; i < cFloats; i++) {
        *buffer++ = FloatFromBits(lcg->ul());
    }
}

uint32_t LinearCongruential::GetNext32()
{
    return lcg->ul();
}

float LinearCongruential::GetNextFloat()
{
    return FloatFromBits(lcg->ul());
}

char *LinearCongruential::GetName() { return "LinearCongruential"; }

#include "stdafx.h"

#include "MersenneTwister64.h"

MersenneTwister64 *MersenneTwister64::CreatePRNG(void *seedData, size_t seedSize)
{
    MersenneTwister64 *prng = nullptr;

    if (seedSize >= sizeof(uint64_t)) {
        uint64_t seed = *(uint64_t *)seedData;
        prng = new MersenneTwister64(seed);
    }
    else {
        throw "MersenneTwister64 needs a 64-bit seed value.";
    }

    return prng;
}

MersenneTwister64::MersenneTwister64(uint64_t seed)
{
    init_genrand64(seed);
}

MersenneTwister64::~MersenneTwister64()
{
}

void MersenneTwister64::GetBits(void *buffer, size_t cBytes)
{
    if (cBytes % 8 != 0) {
        throw "MersenneTwister64 generates bits 64 at a time.";
    }

    uint64_t *dest = (uint64_t *)buffer;
    size_t count = cBytes / 8;
    for (size_t i = 0; i < count; i++) {
        *dest++ = genrand64_int64();
    }
}

void MersenneTwister64::GetFloats(float *buffer, size_t cFloats)
{
    if (cFloats % 2 != 0) {
        throw "MersenneTwister64 generates floats 2 at a time.";
    }

    GetBits(buffer, cFloats * sizeof(float));
    float *pFloats = (float *)buffer;
    size_t count   = cFloats / 2;
    for (size_t i = 0; i < count; i++) {
        *pFloats++ = FloatFromBits(*(uint32_t*)pFloats);
        *pFloats++ = FloatFromBits(*(uint32_t*)pFloats);
    }
}

uint32_t MersenneTwister64::GetNext32()
{
    uint64_t next64 = genrand64_int64();

    return *(uint32_t *)&next64;
}

float MersenneTwister64::GetNextFloat()
{
    return FloatFromBits(GetNext32());
}

char *MersenneTwister64::GetName() { return "MersenneTwister64"; }

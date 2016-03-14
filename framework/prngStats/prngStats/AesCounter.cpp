#include "stdafx.h"

#include "AesCounter.h"

AesCounter *AesCounter::CreatePRNG(void *seedData, size_t seedSize)
{
    AesCounter *prng = nullptr;

    if (seedSize >= sizeof(uint64_t)) {
        prng = new AesCounter(*(uint64_t *)seedData);
    }
    else {
        throw "AesCounter needs a 64-bit seed value.";
    }

    return prng;
}

AesCounter::AesCounter(uint64_t nonce) :
    m_nonce(nonce),
    m_iteration(0)
{
    AES_Init_Ex(&m_keySchedule);
}

AesCounter::~AesCounter()
{
}

void AesCounter::GetBits(void *buffer, size_t cBytes)
{
    if (cBytes % 16 != 0) {
        throw "SimdTwister generates bits 128 at a time.";
    }

    memset(buffer, 0, sizeof(__m128i));
    AES_Get_Bits_Ex(buffer, cBytes, m_nonce, m_iteration++, &m_keySchedule);
}

void AesCounter::GetFloats(float *buffer, size_t cFloats)
{
    if (cFloats % 4 != 0) {
        throw "SimdTwister generates floats 4 at a time.";
    }

    GetBits(buffer, cFloats * sizeof(float));
    size_t count   = cFloats / 4;
    float *pFloats = (float *)buffer;
    for (size_t i = 0; i < count; i++) {
        *pFloats++ = FloatFromBits(*(uint32_t*)pFloats);
        *pFloats++ = FloatFromBits(*(uint32_t*)pFloats);
        *pFloats++ = FloatFromBits(*(uint32_t*)pFloats);
        *pFloats++ = FloatFromBits(*(uint32_t*)pFloats);
    }
}

uint32_t AesCounter::GetNext32()
{
    uint32_t bits[4];
    GetBits(bits, sizeof(bits));
    return bits[0];
}

float AesCounter::GetNextFloat()
{
    return FloatFromBits(GetNext32());
}

char *AesCounter::GetName() { return "AesCounter"; }

// AesCounterPrng.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include "AesCounterPrng.h"

#include <intrin.h>

CAesCounterPrng * CAesCounterPrng::CreatePRNG( uint64_t seedData )
{
    CAesCounterPrng *prng = nullptr;

    if (CpuSupportsAesInstructions())
    {
        prng = new CAesCounterPrng(seedData);
    }

    return prng;
}

bool CAesCounterPrng::CpuSupportsAesInstructions()
{
    int cpuInfo[4];
    __cpuid(cpuInfo, 1);

    return (cpuInfo[2] >> 25) & 0x00000001;
}

CAesCounterPrng::CAesCounterPrng(uint64_t nonce) :
    m_nonce(nonce),
    m_iteration(0)
{
    AES_Init_Ex(&m_keySchedule);
}

CAesCounterPrng::~CAesCounterPrng()
{
}

void CAesCounterPrng::GetBytes(uint8_t *buffer, size_t count)
{
    if (count % 16 != 0)
        throw "CAesCounterPrng generates bits 128 at a time.";

    memset(buffer, 0, sizeof(__m128i));
    AES_Get_Bits_Ex(buffer, count * sizeof(uint8_t), m_nonce, m_iteration++, &m_keySchedule);
}

void CAesCounterPrng::GetShorts(uint16_t *buffer, size_t count)
{
    if (count % 8 != 0)
        throw "CAesCounterPrng generates bits 128 at a time.";

    memset(buffer, 0, sizeof(__m128i));
    AES_Get_Bits_Ex(buffer, count * sizeof(uint16_t), m_nonce, m_iteration++, &m_keySchedule);
}

void CAesCounterPrng::GetInts(uint32_t *buffer, size_t count)
{
    if (count % 4 != 0)
        throw "CAesCounterPrng generates bits 128 at a time.";

    memset(buffer, 0, sizeof(__m128i));
    AES_Get_Bits_Ex(buffer, count * sizeof(uint32_t), m_nonce, m_iteration++, &m_keySchedule);
}

void CAesCounterPrng::GetLongs(uint64_t *buffer, size_t count)
{
    if (count % 2 != 0)
        throw "CAesCounterPrng generates bits 128 at a time.";

    memset(buffer, 0, sizeof(__m128i));
    AES_Get_Bits_Ex(buffer, count * sizeof(uint64_t), m_nonce, m_iteration++, &m_keySchedule);
}

void CAesCounterPrng::GetFloats(float *buffer, size_t count)
{
    if (count % 4 != 0) {
        throw "CAesCounterPrng generates floats 4 at a time.";
    }

    GetInts((uint32_t *)buffer, count);
    float *pFloats = buffer;
    size_t limit = (size_t)count / 4;
    for (size_t i = 0; i < limit; i++) {
        *pFloats++ = FloatFromBits(*(uint32_t*)pFloats);
        *pFloats++ = FloatFromBits(*(uint32_t*)pFloats);
        *pFloats++ = FloatFromBits(*(uint32_t*)pFloats);
        *pFloats++ = FloatFromBits(*(uint32_t*)pFloats);
    }
}

void CAesCounterPrng::GetDoubles(double *buffer, size_t count)
{
    if (count % 2 != 0) {
        throw "CAesCounterPrng generates doubles 2 at a time.";
    }

    GetLongs((uint64_t *)buffer, count);
    double *pFloats = buffer;
    size_t limit = (size_t)count / 2;
    for (size_t i = 0; i < limit; i++) {
        *pFloats++ = DoubleFromBits(*(uint64_t*)pFloats);
        *pFloats++ = DoubleFromBits(*(uint64_t*)pFloats);
    }
}

uint32_t CAesCounterPrng::GetNext32()
{
    uint32_t bits[4];
    GetLongs((uint64_t *)bits, 2);
    return bits[0];
}

uint64_t CAesCounterPrng::GetNext64()
{
    uint64_t bits[2];
    GetLongs(bits, 2);
    return bits[0];
}

float CAesCounterPrng::GetNextFloat()
{
    return FloatFromBits(GetNext32());
}

double CAesCounterPrng::GetNextDouble()
{
    return DoubleFromBits(GetNext64());
}

char *CAesCounterPrng::GetName() { return "CAesCounterPrng"; }

// 31 for keySchedule + 1 for nonce + 1 for iteration
size_t CAesCounterPrng::GetStateSize() { return 33; }

void CAesCounterPrng::GetState(uint64_t *buffer, size_t count)
{
    if (count < 33)
        throw "CAesCounterPrng state requires at least 33 ulongs.";

    uint64_t *pBuffer = buffer;
    uint64_t *pKey = (uint64_t *)&m_keySchedule.KEY;
    for (int i = 0; i < 30; i++)
    {
        *pBuffer++ = *pKey++;
    }

    *pBuffer++ = m_nonce;
    *pBuffer = m_iteration;
}

void CAesCounterPrng::SetState(uint64_t *buffer, size_t count)
{
    if (count < 33)
        throw "CAesCounterPrng state requires at least 33 ulongs.";

    uint64_t *pBuffer = buffer;
    uint64_t *pKey = (uint64_t *)&m_keySchedule.KEY;
    for (int i = 0; i < 30; i++)
    {
        *pKey++ = *pBuffer++;
    }

    m_nonce = *pBuffer++;
    m_iteration = (uint32_t)*pBuffer;
}

AESCOUNTERPRNG_API CAesCounterPrng * CreateAesCounterPrng(uint64_t seedData)
{
    return CAesCounterPrng::CreatePRNG(seedData);
}

AESCOUNTERPRNG_API void DeletePrng(IRandom *prng)
{
    delete prng;
}

AESCOUNTERPRNG_API void GetBytes(IRandom *prng, uint8_t *buffer, size_t count)     { prng->GetBytes(buffer, count); }
AESCOUNTERPRNG_API void GetShorts(IRandom *prng, uint16_t *buffer, size_t count)   { prng->GetShorts(buffer, count); }
AESCOUNTERPRNG_API void GetInts(IRandom *prng, uint32_t *buffer, size_t count)     { prng->GetInts(buffer, count); }
AESCOUNTERPRNG_API void GetLongs(IRandom *prng, uint64_t *buffer, size_t count)    { prng->GetLongs(buffer, count); }
AESCOUNTERPRNG_API void GetFloats(IRandom *prng, float *buffer, size_t count)      { prng->GetFloats(buffer, count); }
AESCOUNTERPRNG_API void GetDoubles(IRandom *prng, double *buffer, size_t count)    { prng->GetDoubles(buffer, count); }
AESCOUNTERPRNG_API uint32_t GetNext32(IRandom *prng)   { return prng->GetNext32(); }
AESCOUNTERPRNG_API uint64_t GetNext64(IRandom *prng)   { return prng->GetNext64(); }
AESCOUNTERPRNG_API float GetNextFloat(IRandom *prng)   { return prng->GetNextFloat(); }
AESCOUNTERPRNG_API double GetNextDouble(IRandom *prng) { return prng->GetNextDouble(); }
AESCOUNTERPRNG_API size_t GetStateSize(IRandom *prng) { return prng->GetStateSize(); }
AESCOUNTERPRNG_API void GetState(IRandom *prng, uint64_t *buffer, size_t count) { return prng->GetState(buffer, count); }
AESCOUNTERPRNG_API void SetState(IRandom *prng, uint64_t *buffer, size_t count) { return prng->SetState(buffer, count); }
AESCOUNTERPRNG_API bool CpuSupportsAesInstructions() { return CAesCounterPrng::CpuSupportsAesInstructions(); }
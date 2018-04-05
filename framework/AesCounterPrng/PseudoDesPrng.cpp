/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

#include "stdafx.h"
#include "PseudoDesPrng.h"

CPseudoDesPrng *CPseudoDesPrng::CreatePRNG(uint32_t seedData)
{
    CPseudoDesPrng *prng = new CPseudoDesPrng(seedData);

    return prng;
}

CPseudoDesPrng::CPseudoDesPrng(uint32_t seedData) :
    _iNum(0),
    _iSeq(seedData),
    _lastUl(0)
{
}

CPseudoDesPrng::~CPseudoDesPrng()
{
}

void CPseudoDesPrng::GetBytes(uint8_t *buffer, size_t count)
{
    if (count % 4 != 0)
        throw "CPseudoDesPrng generates bits 32 at a time.";

    GetInts((uint32_t *)buffer, count >> 2);
}

void CPseudoDesPrng::GetShorts(uint16_t *buffer, size_t count)
{
    if (count % 2 != 0)
        throw "CPseudoDesPrng generates bits 32 at a time.";

    GetInts((uint32_t *)buffer, count >> 1);
}

void CPseudoDesPrng::GetInts(uint32_t *buffer, size_t count)
{
    for (size_t i = 0; i < count; i++)
    {
        *buffer++ = GetNext32();
    }
}

void CPseudoDesPrng::GetLongs(uint64_t *buffer, size_t count)
{
    GetInts((uint32_t *)buffer, count << 1);
}

void CPseudoDesPrng::GetFloats(float *buffer, size_t count)
{
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

void CPseudoDesPrng::GetDoubles(double *buffer, size_t count)
{
    GetLongs((uint64_t *)buffer, count);
    double *pFloats = buffer;
    size_t limit = (size_t)count / 2;
    for (size_t i = 0; i < limit; i++) {
        *pFloats++ = DoubleFromBits(*(uint64_t*)pFloats);
        *pFloats++ = DoubleFromBits(*(uint64_t*)pFloats);
    }
}

uint32_t CPseudoDesPrng::C1[] = { 0xBAA96887, 0x1E17D32C, 0x03BCDC3C, 0x0F33D1B2 };
uint32_t CPseudoDesPrng::C2[] = { 0x4B0F3B58, 0xE874F0C3, 0x6955C5A6, 0x55A7CA46 };

#define HI(x) ((uint32_t) ((uint16_t *) &x)[1])
#define LO(x) ((uint32_t) ((uint16_t *) &x)[0])
#define XCHG(x) ((LO(x) << 16) | HI(x))

uint32_t CPseudoDesPrng::GetNext32()
{
    uint32_t kk[3];
    uint32_t iA, iB;

    iA = _iNum ^ C1[0];
    iB = LO(iA) * LO(iA) + ~(HI(iA) * HI(iA));
    kk[0] = _iSeq ^ ((XCHG(iB) ^ C2[0]) + LO(iA) * HI(iA));

    iA = kk[0] ^ C1[1];
    iB = LO(iA) * LO(iA) + ~(HI(iA) * HI(iA));
    kk[1] = _iNum ^ ((XCHG(iB) ^ C2[1]) + LO(iA) * HI(iA));

    _iNum++;
    if (_iNum == 0)
        _iSeq++;

    iA = kk[1] ^ C1[2];
    iB = LO(iA) * LO(iA) + ~(HI(iA) * HI(iA));
    kk[2] = kk[0] ^ ((XCHG(iB) ^ C2[2]) + LO(iA) * HI(iA));

    iA = kk[2] ^ C1[3];
    iB = LO(iA) * LO(iA) + ~(HI(iA) * HI(iA));

    _lastUl = kk[1] ^ ((XCHG(iB) ^ C2[3]) + LO(iA) * HI(iA));

    return _lastUl;
}

uint64_t CPseudoDesPrng::GetNext64()
{
    uint64_t next;
    GetLongs(&next, 1);

    return next;
}

float CPseudoDesPrng::GetNextFloat()
{
    return FloatFromBits(GetNext32());
}

double CPseudoDesPrng::GetNextDouble()
{
    return DoubleFromBits(GetNext64());
}

char *CPseudoDesPrng::GetName() { return "CPseudoDesPrng"; }

// _iNum + _iSeq + _lastUl
size_t CPseudoDesPrng::GetStateSize() { return 3; }

void CPseudoDesPrng::GetState(uint64_t *buffer, size_t count)
{
    if (count < 3)
        throw "CPseudoDesPrng state requires at least 3 ulongs.";

    buffer[0] = _iNum;
    buffer[1] = _iSeq;
    buffer[2] = _lastUl;
}

void CPseudoDesPrng::SetState(uint64_t *buffer, size_t count)
{
    if (count < 3)
        throw "CPseudoDesPrng state requires at least 3 ulongs.";

    _iNum   = (uint32_t) buffer[0];
    _iSeq   = (uint32_t) buffer[1];
    _lastUl = (uint32_t) buffer[2];
}

AESCOUNTERPRNG_API CPseudoDesPrng *CreatePseudoDesPrng(uint32_t seedData)
{
    return CPseudoDesPrng::CreatePRNG(seedData);
}


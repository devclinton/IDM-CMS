#pragma once

#include <stdint.h>

struct IRandom
{
    virtual char *GetName()                               = 0;
    virtual void GetBits(void *buffer, size_t cBytes)     = 0;
    virtual void GetFloats(float *buffer, size_t cFloats) = 0;
    virtual uint32_t GetNext32()                          = 0;
    virtual float GetNextFloat()                          = 0;

    virtual ~IRandom() {};
};

inline float FloatFromBits(uint32_t bits) {
    union { float f; uint32_t u; } t;

    t.f = 1.0f;
    t.u |= (bits & 0x007FFFFF) | 0x00000001;
    t.f -= 1.0f;

    return t.f;
}
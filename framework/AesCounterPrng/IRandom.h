#pragma once

#include <stdint.h>

struct IRandom
{
    virtual char *GetName()                                 = 0;
    virtual void GetBytes(uint8_t *buffer, size_t count)    = 0;
    virtual void GetShorts(uint16_t *buffer, size_t count)  = 0;
    virtual void GetInts(uint32_t *buffer, size_t count)    = 0;
    virtual void GetLongs(uint64_t *buffer, size_t count)   = 0;
    virtual void GetFloats(float *buffer, size_t count)     = 0;
    virtual void GetDoubles(double *buffer, size_t count)   = 0;
    virtual uint32_t GetNext32()                            = 0;
    virtual uint64_t GetNext64()                            = 0;
    virtual float GetNextFloat()                            = 0;
    virtual double GetNextDouble()                          = 0;

    virtual size_t GetStateSize()                           = 0;
    virtual void GetState(uint64_t *buffer, size_t count)   = 0;
    virtual void SetState(uint64_t *buffer, size_t count)   = 0;

    virtual ~IRandom() {};
};

inline float FloatFromBits(uint32_t bits) {
    union { float f; uint32_t u; } t;

    t.f = 1.0f;
    t.u |= (bits & 0x007FFFFF) | 0x00000001;
    t.f -= 1.0f;

    return t.f;
}

inline double DoubleFromBits(uint64_t bits) {
    union { double d; uint64_t u; } t;

    t.d = 1.0;
    t.u |= (bits & 0x000FFFFFFFFFFFFFL) | 0x0000000000000001L;
    t.d -= 1.0;

    return t.d;
}

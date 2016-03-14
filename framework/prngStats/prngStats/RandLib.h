#pragma once

#include "IRandom.h"

class RandLib : public IRandom
{
public:
    static RandLib *CreatePRNG(void *seedData, size_t seedSize);
    virtual ~RandLib();

    virtual char *GetName();
    virtual void GetBits(void *buffer, size_t cBytes);
    virtual uint32_t GetNext32();
    virtual void GetFloats(float *buffer, size_t cFloats);
    virtual float GetNextFloat();

protected:
    RandLib(uint32_t seed1, uint32_t seed2);
};
#pragma once

#include "IRandom.h"
#include "twister\sfmt\SFMT.h"

class SimdTwister : public IRandom
{
public:
    static SimdTwister *CreatePRNG(void *seedData, size_t seedSize);
    virtual ~SimdTwister();

    virtual char *GetName();
    virtual void GetBits(void *buffer, size_t cBytes);
    virtual uint32_t GetNext32();
    virtual void GetFloats(float *buffer, size_t cFloats);
    virtual float GetNextFloat();

protected:
    SimdTwister(uint32_t *seedData, size_t seedCount);

private:
};

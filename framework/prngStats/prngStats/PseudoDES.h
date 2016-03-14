#pragma once

#include "IRandom.h"
#include "EMOD\RANDOM.h"

class PseudoDES : public IRandom
{
public:
    static PseudoDES *CreatePRNG(void *seedData, size_t seedSize);
    virtual ~PseudoDES();

    virtual char *GetName();
    virtual void GetBits(void *buffer, size_t cBytes);
    virtual uint32_t GetNext32();
    virtual void GetFloats(float *buffer, size_t cFloats);
    virtual float GetNextFloat();

protected:
    PseudoDES(uint32_t seed);

private:
    PSEUDO_DES *pdes;
};

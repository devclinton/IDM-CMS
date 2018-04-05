/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

#pragma once

#include "IRandom.h"
#include "twister\sixtyFour\mt64.h"

class MersenneTwister64 : public IRandom
{
public:
    static MersenneTwister64 *CreatePRNG(void *seedData, size_t seedSize);
    virtual ~MersenneTwister64();

    virtual char *GetName();
    virtual void GetBits(void *buffer, size_t cBytes);
    virtual uint32_t GetNext32();
    virtual void GetFloats(float *buffer, size_t cFloats);
    virtual float GetNextFloat();

protected:
    MersenneTwister64(uint64_t seed);

private:
};

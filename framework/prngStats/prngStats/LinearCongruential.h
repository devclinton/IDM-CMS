/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

#pragma once

#include "IRandom.h"
#include "EMOD\RANDOM.h"

class LinearCongruential : public IRandom
{
public:
    static LinearCongruential *CreatePRNG(void *seedData, size_t seedSize);
    virtual ~LinearCongruential();

    virtual char *GetName();
    virtual void GetBits(void *buffer, size_t cBytes);
    virtual uint32_t GetNext32();
    virtual void GetFloats(float *buffer, size_t cFloats);
    virtual float GetNextFloat();

protected:
    LinearCongruential(uint32_t seed);

private:
    RANDOM *lcg;
};

/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

#ifdef AESCOUNTERPRNG_EXPORTS
#define AESCOUNTERPRNG_API __declspec(dllexport)
#else
#define AESCOUNTERPRNG_API __declspec(dllimport)
#endif

#include "IRandom.h"
class CPseudoDesPrng : public IRandom
{
public:
    static CPseudoDesPrng *CreatePRNG(uint32_t seedData);
    virtual ~CPseudoDesPrng();

    virtual char *GetName();
    virtual void GetBytes(uint8_t *buffer, size_t count);
    virtual void GetShorts(uint16_t *buffer, size_t count);
    virtual void GetInts(uint32_t *buffer, size_t count);
    virtual void GetLongs(uint64_t *buffer, size_t count);
    virtual void GetFloats(float *buffer, size_t count);
    virtual void GetDoubles(double *buffer, size_t count);
    virtual uint32_t GetNext32();
    virtual uint64_t GetNext64();
    virtual float GetNextFloat();
    virtual double GetNextDouble();

    virtual size_t GetStateSize();
    virtual void GetState(uint64_t *buffer, size_t count);
    virtual void SetState(uint64_t *buffer, size_t count);

protected:
    CPseudoDesPrng(uint32_t seedData);

private:
    static uint32_t C1[];
    static uint32_t C2[];
    uint32_t _iNum;
    uint32_t _iSeq;
    uint32_t _lastUl;

};

extern "C" {
    AESCOUNTERPRNG_API CPseudoDesPrng *CreatePseudoDesPrng(uint32_t seedData);
};

/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

// The following ifdef block is the standard way of creating macros which make exporting 
// from a DLL simpler. All files within this DLL are compiled with the AESCOUNTERPRNG_EXPORTS
// symbol defined on the command line. This symbol should not be defined on any project
// that uses this DLL. This way any other project whose source files include this file see 
// AESCOUNTERPRNG_API functions as being imported from a DLL, whereas this DLL sees symbols
// defined with this macro as being exported.
#if defined(_MSC_VER)
    #ifdef AESCOUNTERPRNG_EXPORTS
    #define AESCOUNTERPRNG_API __declspec(dllexport)
    #else
    #define AESCOUNTERPRNG_API __declspec(dllimport)
    #endif
#elif defined(__GNUC__)
    #ifdef AESCOUNTERPRNG_EXPORTS
        #define AESCOUNTERPRNG_API __attribute__((visibility("default")))
    #else
        #define AESCOUNTERPRNG_API
    #endif
#endif


/*
// This class is exported from the AesCounterPrng.dll
class AESCOUNTERPRNG_API CAesCounterPrng {
public:
    CAesCounterPrng(void);
    // TODO: add your methods here.
};
*/

#include "IRandom.h"
#include "aes.h"

class CAesCounterPrng : public IRandom
{
public:
    static CAesCounterPrng *CreatePRNG(uint64_t seedData);

    virtual ~CAesCounterPrng();

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

    static bool CpuSupportsAesInstructions();

protected:
    CAesCounterPrng(uint64_t nonce);

private:
    AES_KEY     m_keySchedule;
    uint64_t    m_nonce;
    uint32_t    m_iteration;
};

extern "C" {
    AESCOUNTERPRNG_API CAesCounterPrng *CreateAesCounterPrng(uint64_t seedData);
    AESCOUNTERPRNG_API void DeletePrng(IRandom *prng);
    AESCOUNTERPRNG_API void GetBytes(IRandom *prng, uint8_t *buffer, size_t count);
    AESCOUNTERPRNG_API void GetShorts(IRandom *prng, uint16_t *buffer, size_t count);
    AESCOUNTERPRNG_API void GetInts(IRandom *prng, uint32_t *buffer, size_t count);
    AESCOUNTERPRNG_API void GetLongs(IRandom *prng, uint64_t *buffer, size_t count);
    AESCOUNTERPRNG_API void GetFloats(IRandom *prng, float *buffer, size_t count);
    AESCOUNTERPRNG_API void GetDoubles(IRandom *prng, double *buffer, size_t count);
    AESCOUNTERPRNG_API uint32_t GetNext32(IRandom *prng);
    AESCOUNTERPRNG_API uint64_t GetNext64(IRandom *prng);
    AESCOUNTERPRNG_API float GetNextFloat(IRandom *prng);
    AESCOUNTERPRNG_API double GetNextDouble(IRandom *prng);
    AESCOUNTERPRNG_API size_t GetStateSize();
    AESCOUNTERPRNG_API void GetState(uint64_t *buffer, size_t count);
    AESCOUNTERPRNG_API void SetState(uint64_t *buffer, size_t count);
    AESCOUNTERPRNG_API bool CpuSupportsAesInstructions();
};

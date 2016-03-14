#pragma once

#include "IRandom.h"
#include "aes\aes.h"

class AesCounter : public IRandom
{
public:
    static AesCounter *CreatePRNG(void *seedData, size_t seedSize);
    virtual ~AesCounter();

    virtual char *GetName();
    virtual void GetBits(void *buffer, size_t cBytes);
    virtual uint32_t GetNext32();
    virtual void GetFloats(float *buffer, size_t cFloats);
    virtual float GetNextFloat();

protected:
    AesCounter(uint64_t nonce);

private:
    AES_KEY     m_keySchedule;
    uint64_t    m_nonce;
    uint32_t    m_iteration;
};

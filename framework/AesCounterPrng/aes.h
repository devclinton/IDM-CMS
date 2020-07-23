/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

#include <emmintrin.h>
#include <memory>



#if defined(_MSC_VER)
#define ALIGNED_(x) __declspec(align(x))
#else
#if defined(__GNUC__)
#define ALIGNED_(x) __attribute__ ((aligned(x)))
#endif
#endif

#define _ALIGNED_TYPE(t,x) typedef t ALIGNED_(x)
_ALIGNED_TYPE(__m128i, 16) aligned__m128i;
typedef uint8_t aligned__uint8_t ;


typedef struct KEY_SCHEDULE {
    aligned__m128i KEY[15];
    unsigned int nr;
} AES_KEY;

void AES_Init();
void AES_Init_Ex(AES_KEY *pExpanded);
void AES_Get_Bits(void *buffer, uint32_t bytes, uint32_t rank, uint64_t run);
void AES_Get_Bits_Ex(void *buffer, size_t bytes, uint64_t nonce, uint32_t count, AES_KEY *pExpanded);

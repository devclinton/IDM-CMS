/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

#include <emmintrin.h>

#define ALIGN16 __declspec (align (16))

typedef struct KEY_SCHEDULE {
    ALIGN16 __m128i KEY[15];
    unsigned int nr;
} AES_KEY;

void AES_Init();
void AES_Init_Ex(AES_KEY *pExpanded);
void AES_Get_Bits(void *buffer, uint32_t bytes, uint32_t rank, uint64_t run);
void AES_Get_Bits_Ex(void *buffer, size_t bytes, uint64_t nonce, uint32_t count, AES_KEY *pExpanded);

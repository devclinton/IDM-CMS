//#include "stdafx.h"
#include <stdint.h>
#include <stdio.h>
//#include <intrin.h>
#include <smmintrin.h>
#include <wmmintrin.h>
#include <cpuid.h>
#include <cstring>

#include "aes.h"
#ifndef LENGTH
#define LENGTH 64
#endif

#if defined(__GNUC__)
typedef uint8_t BYTE;
typedef uint32_t DWORD;
typedef int32_t LONG;
typedef int64_t LONGLONG;

typedef union _LARGE_INTEGER {
    struct {
        DWORD LowPart;
        LONG  HighPart;
    };
    struct {
        DWORD LowPart;
        LONG  HighPart;
    } u;
    LONGLONG QuadPart;
} LARGE_INTEGER, *PLARGE_INTEGER;
#endif

#define __OPTIMIZE__ 1
aligned__uint8_t AES128_TEST_KEY[] = {0x7E,0x24,0x06,0x78,0x17,0xFA,0xE0,0xD7,0x43,0xD6,0xCE,0x1F,0x32,0x53,0x91,0x63};
aligned__uint8_t AES192_TEST_KEY[] = {0x7C,0x5C,0xB2,0x40,0x1B,0x3D,0xC3,0x3C,0x19,0xE7,0x34,0x08,0x19,0xE0,0xF6,0x9C,0x67,0x8C,0x3D,0xB8,0xE6,0xF6,0xA9,0x1A};
aligned__uint8_t AES256_TEST_KEY[] = {0xF6,0xD6,0x6D,0x6B,0xD5,0x2D,0x59,0xBB,0x07,0x96,0x36,0x58,0x79,0xEF,0xF8,0x86,0xC6,0x6D,0xD5,0x1A,0x5B,0x6A,0x99,0x74,0x4B,0x50,0x59,0x0C,0x87,0xA2,0x38,0x84};

aligned__uint8_t AES_TEST_VECTOR[] = {0x00,0x01,0x02,0x03,0x04,0x05,0x06,0x07,0x08,0x09,0x0A,0x0B,0x0C,0x0D,0x0E,0x0F,0x10,0x11,0x12,0x13,0x14,0x15,0x16,0x17,0x18,0x19,0x1A,0x1B,0x1C,0x1D,0x1E,0x1F};

aligned__uint8_t CTR128_IV[] = {0xC0,0x54,0x3B,0x59,0xDA,0x48,0xD9,0x0B};
aligned__uint8_t CTR192_IV[] = {0x02,0x0C,0x6E,0xAD,0xC2,0xCB,0x50,0x0D};
aligned__uint8_t CTR256_IV[] = {0xC1,0x58,0x5E,0xF1,0x5A,0x43,0xD8,0x75};

aligned__uint8_t CTR128_NONCE[] = {0x00,0x6C,0xB6,0xDB};
aligned__uint8_t CTR192_NONCE[] = {0x00,0x96,0xB0,0x3B};
aligned__uint8_t CTR256_NONCE[] = {0x00,0xFA,0xAC,0x24};

aligned__uint8_t CTR128_EXPECTED[] = {0x51,0x04,0xA1,0x06,0x16,0x8A,0x72,0xD9,0x79,0x0D,0x41,0xEE,0x8E,0xDA,0xD3,0x88,0xEB,0x2E,0x1E,0xFC,0x46,0xDA,0x57,0xC8,0xFC,0xE6,0x30,0xDF,0x91,0x41,0xBE,0x28};
aligned__uint8_t CTR192_EXPECTED[] = {0x45,0x32,0x43,0xFC,0x60,0x9B,0x23,0x32,0x7E,0xDF,0xAA,0xFA,0x71,0x31,0xCD,0x9F,0x84,0x90,0x70,0x1C,0x5A,0xD4,0xA7,0x9C,0xFC,0x1F,0xE0,0xFF,0x42,0xF4,0xFB,0x00};
aligned__uint8_t CTR256_EXPECTED[] = {0xF0,0x5E,0x23,0x1B,0x38,0x94,0x61,0x2C,0x49,0xEE,0x00,0x0B,0x80,0x4E,0xB2,0xA9,0xB8,0x30,0x6B,0x50,0x8F,0x83,0x9D,0x6A,0x55,0x30,0x83,0x1D,0x93,0x44,0xAF,0x1C};

inline __m128i AES_128_ASSIST(__m128i temp1, __m128i temp2)
{
    __m128i   temp3;

    temp2 = _mm_shuffle_epi32(temp2, 0xFF);
    temp3 = _mm_slli_si128(temp1, 0x4);
    temp1 = _mm_xor_si128(temp1, temp3);
    temp3 = _mm_slli_si128(temp3, 0x4);
    temp1 = _mm_xor_si128(temp1, temp3);
    temp3 = _mm_slli_si128(temp3, 0x4);
    temp1 = _mm_xor_si128(temp1, temp3);
    temp1 = _mm_xor_si128(temp1, temp2);

    return temp1;
}

void AES_128_Key_Expansion(const unsigned char *userkey, AES_KEY *key)
{
    __m128i temp1, temp2;
    __m128i *Key_Schedule = (__m128i*)key;

    temp1            = _mm_loadu_si128((__m128i*)userkey);
    Key_Schedule[0]  = temp1;
    temp2            = _mm_aeskeygenassist_si128 (temp1 ,0x1);
    temp1            = AES_128_ASSIST(temp1, temp2);
    Key_Schedule[1]  = temp1;
    temp2            = _mm_aeskeygenassist_si128 (temp1,0x2);
    temp1            = AES_128_ASSIST(temp1, temp2);
    Key_Schedule[2]  = temp1;
    temp2            = _mm_aeskeygenassist_si128 (temp1,0x4);
    temp1            = AES_128_ASSIST(temp1, temp2);
    Key_Schedule[3]  = temp1;
    temp2            = _mm_aeskeygenassist_si128 (temp1,0x8);
    temp1            = AES_128_ASSIST(temp1, temp2);
    Key_Schedule[4]  = temp1;
    temp2            = _mm_aeskeygenassist_si128 (temp1,0x10);
    temp1            = AES_128_ASSIST(temp1, temp2);
    Key_Schedule[5]  = temp1;
    temp2            = _mm_aeskeygenassist_si128 (temp1,0x20);
    temp1            = AES_128_ASSIST(temp1, temp2);
    Key_Schedule[6]  = temp1;
    temp2            = _mm_aeskeygenassist_si128 (temp1,0x40);
    temp1            = AES_128_ASSIST(temp1, temp2);
    Key_Schedule[7]  = temp1;
    temp2            = _mm_aeskeygenassist_si128 (temp1,0x80);
    temp1            = AES_128_ASSIST(temp1, temp2);
    Key_Schedule[8]  = temp1;
    temp2            = _mm_aeskeygenassist_si128 (temp1,0x1b);
    temp1            = AES_128_ASSIST(temp1, temp2);
    Key_Schedule[9]  = temp1;
    temp2            = _mm_aeskeygenassist_si128 (temp1,0x36);
    temp1            = AES_128_ASSIST(temp1, temp2);
    Key_Schedule[10] = temp1;
}

void AES_CTR_encrypt(
    const unsigned char *in,
    unsigned char *out,
    const unsigned char *ivec, // [8]
    const unsigned char *nonce, // [4]
    unsigned long length,
    const __m128i *key,
    int number_of_rounds)
{
    __m128i ctr_block, tmp, ONE, BSWAP_EPI64;
    int i,j;

    if (length%16) {
        length = length/16 + 1;
    }
    else {
        length/=16;
    }

#ifdef _DEBUG
    uint8_t init[] = { 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC };
    ctr_block = *(__m128i *)init;
#endif

    ONE         = _mm_set_epi32(0,1,0,0);
    BSWAP_EPI64 = _mm_setr_epi8(7,6,5,4,3,2,1,0,15,14,13,12,11,10,9,8);
    ctr_block   = _mm_insert_epi64(ctr_block, *(long long*)ivec, 1);
    ctr_block   = _mm_insert_epi32(ctr_block, *(long*)nonce, 1);
    ctr_block   = _mm_srli_si128(ctr_block, 4);
    ctr_block   = _mm_shuffle_epi8(ctr_block, BSWAP_EPI64);
    ctr_block   = _mm_add_epi64(ctr_block, ONE);

    for (i = 0; i < (int)length; i++) {
        tmp       = _mm_shuffle_epi8(ctr_block, BSWAP_EPI64);
        ctr_block = _mm_add_epi64(ctr_block, ONE);
        tmp       = _mm_xor_si128(tmp, ((__m128i*)key)[0]);

        for (j = 1; j <number_of_rounds; j++) {
            tmp = _mm_aesenc_si128 (tmp, ((__m128i*)key)[j]);
        }

        tmp = _mm_aesenclast_si128 (tmp, ((__m128i*)key)[j]);
        tmp = _mm_xor_si128(tmp,_mm_loadu_si128(&((__m128i*)in)[i]));
        _mm_storeu_si128 (&((__m128i*)out)[i],tmp);
    }
}

void print_m128i_with_string(char *string, __m128i data)
{
    unsigned char *pointer = (unsigned char *)&data;
    int i;

    printf("%-40s[0x", string);

    for (i = 0; i < 16; i++) {
        printf("%02X", pointer[i]);
    }

    printf("]\n");
}

void print_m128i_with_string_short(char *string, __m128i data, int length)
{
    unsigned char *pointer = (unsigned char*)&data;
    int i;

    printf("%-40s[0x",string);

    for (i=0; i < length; i++) {
        printf("%02X",pointer[i]);
    }

    printf("]\n");
}

int AES_set_encrypt_key(const unsigned char *userKey, const int bits, AES_KEY *key)
{
    if (!userKey || !key) return -1;

    AES_128_Key_Expansion(userKey, key);
    key->nr = 10;
    return 0;
}

bool Check_CPU_support_AES()
{


#if defined(_MSC_VER)
    int cpuInfo[4];
    __cpuid(cpuInfo, 1);
    return (cpuInfo[2] >> 25) & 0x00000001;
#elif defined(__GNUC__)
    uint32_t eax, ebx, ecx, edx;
    eax = ebx = ecx = edx = 0;
    __get_cpuid(1, &eax, &ebx, &ecx, &edx);
    return (ecx & bit_AES) > 0;
#endif
}

int ctr()
{
    AES_KEY key;
    uint8_t *PLAINTEXT;
    uint8_t *CIPHERTEXT;
    uint8_t *DECRYPTEDTEXT;
    uint8_t *EXPECTED_CIPHERTEXT;
    uint8_t *CIPHER_KEY;
    uint8_t *NONCE;
    uint8_t *IV;
    int i,j;
    int key_length;

    if (!Check_CPU_support_AES()) {
        printf("CPU does not support AES instruction set. Bailing out.\n");
        return 1;
    }
    printf("CPU supports AES instruction set.\n\n");

#define STR "Performing AES128 CTR.\n"
    CIPHER_KEY          = AES128_TEST_KEY;
    EXPECTED_CIPHERTEXT = CTR128_EXPECTED;
    IV                  = CTR128_IV;
    NONCE               = CTR128_NONCE;
    key_length          = 128;

    PLAINTEXT     = (uint8_t*)malloc(LENGTH);
    CIPHERTEXT    = (uint8_t*)malloc(LENGTH);
    DECRYPTEDTEXT = (uint8_t*)malloc(LENGTH);

    for (i = 0; i < LENGTH/16/2; i++) {
        for (j = 0; j < 2; j++) {
            _mm_storeu_si128(&((__m128i*)PLAINTEXT)[i*2+j], ((__m128i*)AES_TEST_VECTOR)[j]);
        }
    }

    for (j = i*2; j < LENGTH/16; j++) {
        _mm_storeu_si128(&((__m128i*)PLAINTEXT)[j], ((__m128i*)AES_TEST_VECTOR)[j%4]);
    }

    if (LENGTH%16) {
        _mm_storeu_si128(&((__m128i*)PLAINTEXT)[j], ((__m128i*)AES_TEST_VECTOR)[j%4]);
    }

    AES_set_encrypt_key(CIPHER_KEY, key_length, &key);
    AES_CTR_encrypt(PLAINTEXT,  CIPHERTEXT,    IV, NONCE, LENGTH, key.KEY, key.nr);
    AES_CTR_encrypt(CIPHERTEXT, DECRYPTEDTEXT, IV, NONCE, LENGTH, key.KEY, key.nr);

    uint64_t start;
    uint64_t end;
    uint64_t freq;
    #if defined(_MSC_VER)
    QueryPerformanceFrequency((LARGE_INTEGER *)&freq);
    #endif
    for (int k = 0; k < 8; k++)
    {
        #if defined(_MSC_VER)
        QueryPerformanceCounter((LARGE_INTEGER *)&start);
        #endif
        for (int q = 0; q < ((1 << 20) / (LENGTH * 8)); q++) {
            AES_CTR_encrypt(CIPHERTEXT, DECRYPTEDTEXT, IV, NONCE, LENGTH, key.KEY, key.nr);
        }
        #if defined(_MSC_VER)
        QueryPerformanceCounter((LARGE_INTEGER *)&end);
        double bitsPerSecond = (1 << 20) * freq / double(end - start);
        printf("\t\t%f bits per second (%d * %I64u / %I64u)\n", bitsPerSecond, (1 << 20), freq, end - start);
        #endif
    }

    printf("%s\n",STR);
    printf("The Cipher Key:\n"); print_m128i_with_string("",((__m128i*)CIPHER_KEY)[0]);

    printf("The Key Schedule:\n");
    for (i = 0; i < (int)key.nr; i++) {
        print_m128i_with_string("",((__m128i*)key.KEY)[i]);
    }

    printf("The PLAINTEXT:\n");
    for (i = 0; i < LENGTH/16; i++) {
        print_m128i_with_string("",((__m128i*)PLAINTEXT)[i]);
    }

    if (LENGTH%16) {
        print_m128i_with_string_short("",((__m128i*)PLAINTEXT)[i],LENGTH%16);
    }

    printf("\n\nThe CIPHERTEXT:\n");
    for (i = 0; i < LENGTH/16; i++) {
        print_m128i_with_string("",((__m128i*)CIPHERTEXT)[i]);
    }

    if (LENGTH%16) {
        print_m128i_with_string_short("",((__m128i*)CIPHERTEXT)[i],LENGTH%16);
    }

    for (i = 0; i < ((32 < LENGTH) ? 32 : LENGTH); i++) {
        if (CIPHERTEXT[i] != EXPECTED_CIPHERTEXT[i%(16*2)]) {
            printf("\nThe ciphertext is not equal to the expected ciphertext.\n\n");
            return 1;
        }
    }
    printf("\nThe ciphertext equals the expected ciphertext.\n\n");

    for (i = 0; i < LENGTH; i++) {
        if (DECRYPTEDTEXT[i] != PLAINTEXT[i]) {
            printf("The decrypted text is not equal to the original plaintext.\n\n");
            return 1;
        }
    }
    printf("The decrypted text equals to the original plaintext.\n\n");

    {
        __m128i key;
        memset(&key, 0, sizeof(key));
        AES_KEY aes_key;
        AES_set_encrypt_key((const unsigned char *)&key, 128, &aes_key);
        uint32_t bit_count = (1 << 23);
        size_t count = (bit_count + 128) / (sizeof(uint32_t) * 8);
        size_t size  = sizeof(uint32_t);
        uint32_t *buffer = (uint32_t *)calloc(count, size);
        uint64_t ivec = 0;
        uint64_t start;
        uint64_t end;
        uint64_t freq;
        #if defined(_MSC_VER)
        QueryPerformanceFrequency((LARGE_INTEGER *)&freq);
        #endif
        for (uint32_t k = 0; k < 16; k++)
        {
            #if defined(_MSC_VER)
            QueryPerformanceCounter((LARGE_INTEGER *)&start);
            #endif
            AES_CTR_encrypt((const unsigned char *)buffer, (unsigned char *)(buffer + (16 / sizeof(uint32_t))), (const unsigned char *)&ivec, (const unsigned char *)&k, (bit_count >> 3), aes_key.KEY, aes_key.nr);
            #if defined(_MSC_VER)
            QueryPerformanceCounter((LARGE_INTEGER *)&end);
            double bitsPerSecond = bit_count * freq / double(end - start);
            printf("\t\t%f bits per second (%d * %I64u / %I64u)\n", bitsPerSecond, bit_count, freq, end - start);
            #endif
        }
    }

    {
        __m128i key; memset(&key, 0, sizeof(key));
        AES_KEY expanded;
        AES_set_encrypt_key((const unsigned char *)&key, 128, &expanded);
        __m128i in; memset(&in, 0, sizeof(in));
        __m128i out;
        uint64_t ivec = 0;
        uint32_t nonce = 0;
        for (int run = 0; run < 4; run++) {
            nonce = run;
            for (int rank = 0; rank < 4; rank++) {
                ivec = rank;
                AES_CTR_encrypt((const unsigned char *)&in, (unsigned char *)&out, (unsigned char *)&ivec, (const unsigned char *)&nonce, 128, expanded.KEY, expanded.nr);
                print_m128i_with_string("", out);
            }
        }
    }

    return 0;
}

static AES_KEY _expanded;

void AES_Init()
{
    __m128i key;
    memset(&key, 0, sizeof(key));
    AES_set_encrypt_key((const unsigned char *)&key, 128, &_expanded);
}

void AES_Init_Ex(AES_KEY *pExpanded)
{
    __m128i key;
    memset(&key, 0, sizeof(key));
    AES_set_encrypt_key((const unsigned char *)&key, 128, pExpanded);
}

void AES_Get_Bits(void *buffer, uint32_t bytes, uint32_t rank, uint64_t run)
{
    __m128i *in = (__m128i *)buffer;
    __m128i *out = in;
    __m128i *key = _expanded.KEY;
    unsigned int length;
    unsigned int nr = _expanded.nr;

    __m128i ctr_block, tmp, ONE, BSWAP_EPI64;
    unsigned int i,j;

    if (bytes%16) {
        length = bytes / 16 + 1;
    }
    else {
        length = bytes / 16;
    }

    memset(&ctr_block, 0xCC, sizeof(ctr_block));

    ONE         = _mm_set_epi32(0,1,0,0);
    BSWAP_EPI64 = _mm_setr_epi8(7,6,5,4,3,2,1,0,15,14,13,12,11,10,9,8);
    ctr_block   = _mm_insert_epi64(ctr_block, run, 1);
    ctr_block   = _mm_insert_epi32(ctr_block, rank, 1);
    ctr_block   = _mm_srli_si128(ctr_block, 4);
    ctr_block   = _mm_shuffle_epi8(ctr_block, BSWAP_EPI64);
    ctr_block   = _mm_add_epi64(ctr_block, ONE);

    {
        memset(in, 0, sizeof(__m128i));
        tmp       = _mm_shuffle_epi8(ctr_block, BSWAP_EPI64);
        ctr_block = _mm_add_epi64(ctr_block, ONE);
        tmp       = _mm_xor_si128(tmp, key[0]);

        for (j = 1; j < nr; j++) {
            tmp = _mm_aesenc_si128 (tmp, key[j]);
        }

        tmp = _mm_aesenclast_si128 (tmp, key[j]);
        tmp = _mm_xor_si128(tmp,_mm_loadu_si128(in));
        _mm_storeu_si128(out,tmp);
    }

    for (i = 1; i < length; i++) {
        tmp       = _mm_shuffle_epi8(ctr_block, BSWAP_EPI64);
        ctr_block = _mm_add_epi64(ctr_block, ONE);
        tmp       = _mm_xor_si128(tmp, key[0]);

        for (j = 1; j < nr; j++) {
            tmp = _mm_aesenc_si128 (tmp, key[j]);
        }

        tmp = _mm_aesenclast_si128 (tmp, key[j]);
        tmp = _mm_xor_si128(tmp,_mm_loadu_si128(in+i-1));
        _mm_storeu_si128(out+i,tmp);
    }
}

void AES_Get_Bits_Ex(void *buffer, size_t bytes, uint64_t nonce, uint32_t count, AES_KEY *pKey)
{
    __m128i *in = (__m128i *)buffer;
    __m128i *out = in;
    __m128i *key = pKey->KEY;
    size_t length;
    unsigned int nr = pKey->nr;

    __m128i ctr_block, tmp, ONE, BSWAP_EPI64;
    unsigned int i,j;

    if (bytes%16) {
        length = bytes / 16 + 1;
    }
    else {
        length = bytes / 16;
    }

    memset(&ctr_block, 0xCC, sizeof(ctr_block));

    ONE         = _mm_set_epi32(0,1,0,0);
    BSWAP_EPI64 = _mm_setr_epi8(7,6,5,4,3,2,1,0,15,14,13,12,11,10,9,8);
    ctr_block   = _mm_insert_epi64(ctr_block, nonce, 1);
    ctr_block   = _mm_insert_epi32(ctr_block, count, 1);
    ctr_block   = _mm_srli_si128(ctr_block, 4);
    ctr_block   = _mm_shuffle_epi8(ctr_block, BSWAP_EPI64);
    ctr_block   = _mm_add_epi64(ctr_block, ONE);

    {
        memset(in, 0, sizeof(__m128i));
        tmp       = _mm_shuffle_epi8(ctr_block, BSWAP_EPI64);
        ctr_block = _mm_add_epi64(ctr_block, ONE);
        tmp       = _mm_xor_si128(tmp, key[0]);

        for (j = 1; j < nr; j++) {
            tmp = _mm_aesenc_si128 (tmp, key[j]);
        }

        tmp = _mm_aesenclast_si128(tmp, key[j]);
        tmp = _mm_xor_si128(tmp,_mm_loadu_si128(in));
        _mm_storeu_si128(out,tmp);
    }

    for (i = 1; i < length; i++) {
        tmp       = _mm_shuffle_epi8(ctr_block, BSWAP_EPI64);
        ctr_block = _mm_add_epi64(ctr_block, ONE);
        tmp       = _mm_xor_si128(tmp, key[0]);

        for (j = 1; j < nr; j++) {
            tmp = _mm_aesenc_si128(tmp, key[j]);
        }

        tmp = _mm_aesenclast_si128(tmp, key[j]);
        tmp = _mm_xor_si128(tmp,_mm_loadu_si128(in+i-1));
        _mm_storeu_si128(out+i,tmp);
    }
}


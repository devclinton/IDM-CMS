/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

/******************************Module*Header*******************************\
* Module Name: random.hxx                                                  *
*                                                                          *
* A random number class.  It depends on a 32 bit ULONG type and 16 bit     *
* USHORT type.                                                             *
*                                                                          *
* Created: 25-Mar-1994 12:34:57                                            *
* Author: Charles Whitmer [chuckwh]                                        *
*                                                                          *
* Copyright (c) 1994 Charles Whitmer                                       *
\**************************************************************************/
#pragma once

// helper macros for verifying 

#include <stdint.h>
/*
#define int16_t     __int16
#define uint16_t    unsigned __int16
#define int32_t     __int32
#define uint32_t    unsigned __int32
#define int64_t     __int64
#define uint64_t    unsigned __int64
*/

#define __ULONG     uint32_t
#define __UINT      uint32_t
#define __BOOL      bool
#define __USHORT    uint16_t
#define __ULONGLONG uint64_t

class RANDOMBASE
{
protected:
    __ULONG   iSeq;
    __BOOL    bSeq;
    __ULONG   last_ul;
private:
    __BOOL    bWrite;
    __BOOL    bGauss;
    double  eGauss_;
    static int cReg;

    // precision of gamma-distributed random number
    static double cdf_random_num_precision;
    static double tan_pi_4;
    static double pi;

/**************\
* Constructors *
\**************/

public:

// RANDOMBASE(iSequence) - Starts a given sequence.  Does not update the registry.

    RANDOMBASE(__ULONG iSequence) : last_ul(0)
    {
        iSeq   = iSequence;
        bSeq   = true;
        bWrite = false;
        bGauss = false;
    }

// RANDOMBASE() - Reads the seed from the registry and saves it on exit.

    RANDOMBASE();

    virtual ~RANDOMBASE();

/************************\
* User callable routines *
\************************/

public:
// iSequence() - Allows the caller to query the present sequence.

    __ULONG iSequence()
    {
        return iSeq;
    }

// ul() - Returns a random 32 bit number.    
    
    virtual __ULONG ul() = 0;
    __ULONG LastUL()   { return last_ul; }

// i(N) - Returns a random USHORT less than N.

    __USHORT i(__USHORT N)
    {
        __ULONG ulA = ul();
        __ULONG ll = (ulA & 0xFFFFL) * N;
        ll >>= 16;
        ll += (ulA >> 16) * N;
        return (__USHORT) (ll >> 16);
    }

// e() - Returns a random float between 0 and 1.

#define FLOAT_EXP   8
#define DOUBLE_EXP 11

//#define CHECK_RNG_SEQUENCE // debug option to make it easy to visualize the random number sequence

    float e()
    {
        union {float e; __ULONG ul;} e_ul;
        
        e_ul.e = 1.0f;
        e_ul.ul += (ul() >> (FLOAT_EXP+1)) | 1;
        float _e =  e_ul.e - 1.0f;
#ifdef CHECK_RNG_SEQUENCE
        log_info_printf("e() = %f\n", _e);
#endif
        return _e;
    }

    double ee();
    
// vShuffleFloats(pe,N) - Shuffles a list of N floats.

    void  vShuffleFloats(float *pe,__USHORT N);

// eGauss - Returns a normal deviate.

    double eGauss();

//  Added by Philip Eckhoff, Poisson takes in a rate, and returns the number of events in unit time
//  Or equivalently, takes in rate*time and returns number of events in that time
//  Poisson uses a Gaussian approximation for large lambda, while Poisson_true is the fully accurate Poisson
// expdist takes in a rate and returns the sample from an exponential distribution
    unsigned long long int Poisson(double=1.0);
    unsigned long int Poisson_true(double=1.0);
    double expdist(double=1.0);
    uint64_t binomial_approx(uint64_t=1, double=1.0);
    
// M Behrend
// gamma-distributed random number
// shape constant k=2

    double get_pi();
    double rand_gamma(double mean);
    double gamma_cdf(double x, double mean);
    double get_cdf_random_num_precision();
};

class RANDOM : public RANDOMBASE
{
public:
    RANDOM(__ULONG iSequence) : RANDOMBASE(iSequence)
    {}
        
    RANDOM() : RANDOMBASE()
    {
        if (!bSeq)
        {
            iSeq = 0x31415926;
            bSeq = true;
        }
    }

   ~RANDOM()
    {}
    
    __ULONG ul();
};

// Numerical Recipes in C, 2nd ed. Press, William H. et. al, 1992.
class PSEUDO_DES : public RANDOMBASE
{
    __ULONG   iNum;

public:
    PSEUDO_DES(__ULONG iSequence) : RANDOMBASE(iSequence)
    {
        iNum = 0;
    }
        
    PSEUDO_DES() : RANDOMBASE()
    {
        if (!bSeq)
        {
            iSeq = 0;
            bSeq = true;
        }
        iNum = 0;
    }

    __ULONG get_iNum() { return iNum; }

   ~PSEUDO_DES()
    {
        iSeq++;
    }
    
    __ULONG ul();
};

union __ULONGLONG_
{
    struct 
    {
        __ULONG Low;
        __ULONG High;
    };
    __ULONGLONG Quad;
};

void IntializeRNG( int RANDOM_TYPE, int Run_Number, int rank );

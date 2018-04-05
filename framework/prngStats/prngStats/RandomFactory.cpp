/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

#include "stdafx.h"

#include "RandomFactory.h"
#include "LinearCongruential.h"
#include "PseudoDES.h"
#include "MersenneTwister64.h"
#include "SimdTwister.h"
#include "AesCounter.h"
#include "RandLib.h"

IRandom *RandomFactory::CreatePRNG(PRNG_TYPE prngType, void *seedData, size_t cSeedBytes)
{
    IRandom *prng = nullptr;

    switch (prngType)
    {
    case PRNG_EMODLCG:
        prng = LinearCongruential::CreatePRNG(seedData, cSeedBytes);
        break;

    case PRNG_EMODPDES:
        prng = PseudoDES::CreatePRNG(seedData, cSeedBytes);
        break;

    case PRNG_MT64:
        prng = MersenneTwister64::CreatePRNG(seedData, cSeedBytes);
        break;

    case PRNG_SFMT:
        prng = SimdTwister::CreatePRNG(seedData, cSeedBytes);
        break;

    case PRNG_AESCTR:
        prng = AesCounter::CreatePRNG(seedData, cSeedBytes);
        break;

    case PRNG_RANDLIB:
        prng = RandLib::CreatePRNG(seedData, cSeedBytes);
        break;

    default:
        throw "Unknown PRNG specified in call to RandomFactory::CreatePRNG().";
    }

    return prng;
}

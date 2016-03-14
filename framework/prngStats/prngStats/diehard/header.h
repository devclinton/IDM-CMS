#ifndef _HEADER_H_
#define _HEADER_H_

#include <stdio.h>
#include <stdlib.h>
#include <math.h>

#include "..\IRandom.h"

#define DIM       4096
#define UNIMAX    4294967296.   /*pow(2,32)*/

typedef unsigned int    uniform;    

typedef unsigned long   counter;

typedef double          real;

uniform uni(char *filename);

double Phi(double z);

double Chisq(int df, double chsq);

double Poisson(double lambda, int k);

real KStest(real *x, int dim);

void bday    (char *);
void operm5  (char *);
void binrnk  (char *, char *);
void bitst   (char *);
void monky   (char *, char *);
void cnt1s   (char *, char *);
void park    (char *);
void mindist (char *);
void sphere  (char *);
void squeez  (char *);
void osum    (char *);
void runtest (char *);
void craptest(char *);

enum BinRankTest {
    SIX_BY_EIGHT           = 0,
    THIRTYONE_BY_THIRTYONE = 1,
    THIRTYTWO_BY_THIRTYTWO = 2,
    BINRANKTESTENUM_COUNT  = 3
};

extern char *BinRankTestEnumNames[BINRANKTESTENUM_COUNT];

enum MonkeyTest {
    OPSO = 0,
    OQSO = 1,
    DNA  = 2,
    MONKEYTESTENUM_COUNT = 3
};

extern char *MonkeyTestEnumNames[MONKEYTESTENUM_COUNT];

enum CountOnesTest {
    STREAM   = 0,
    SPECIFIC = 1,
    COUNTONESTTESTENUM_COUNT = 2
};

extern char *CountOnesTestEnumNames[COUNTONESTTESTENUM_COUNT];

void BDayEx(IRandom *prng);
void Operm5Ex(IRandom *prng);
void BinRankEx(IRandom *prng, BinRankTest test);
void BitTestEx(IRandom *prng);
void MonkeyTestEx(IRandom *prng, MonkeyTest test);
void Count1sEx(IRandom *prng, CountOnesTest test);
void ParkEx(IRandom *prng);
void MinDistEx(IRandom *prng);
void SphereEx(IRandom *prng);
void SqueezeEx(IRandom *prng);
void OSumEx(IRandom *prng);
void RunTestEx(IRandom *prng);
void CrapTestEx(IRandom *prng);
void DiehardEx(IRandom *prng);

#endif /*_HEADER_H_*/




//#include "stdafx.h"

#include "ranlib.h"
#include <stdio.h>
#include <stdlib.h>

namespace RngLib {

/*
**********************************************************************
     void advnst(long k)
               ADV-a-N-ce ST-ate
     Advances the state  of  the current  generator  by 2^K values  and
     resets the initial seed to that value.
     This is  a  transcription from   Pascal to  Fortran    of  routine
     Advance_State from the paper
     L'Ecuyer, P. and  Cote, S. "Implementing  a  Random Number Package
     with  Splitting   Facilities."  ACM  Transactions  on Mathematical
     Software, 17:98-111 (1991)
                              Arguments
     k -> The generator is advanced by2^K values
**********************************************************************
*/
/*
void Common::AdvanceState(int k)
{
    int g;
    int ib1,ib2;
    int qrgnin;

    // Abort unless random number generator initialized
    GetSetGeneratorInitialized(false, &qrgnin);

    if (!qrgnin)
    {
        fputs("AdvanceState called before random generator initialized - ABORT\n", stderr);
        exit(-1);
    }

    RNG::GetSetArguments(false, &g);

    ib1 = Xa1;
    ib2 = Xa2;
    for (int i = 0; i < k; i++)
    {
        ib1 = Common::mltmod(ib1, ib1, Xm1);
        ib2 = Common::mltmod(ib2, ib2, Xm2);
    }

    SetSeed(Common::mltmod(ib1, Xcg1[g-1], Xm1), Common::mltmod(ib2, Xcg2[g-1], Xm2));

    // NOW, IB1 = A1**K AND IB2 = A2**K
}
*/

/*
**********************************************************************
     void getsd(long *iseed1,long *iseed2)
               GET SeeD
     Returns the value of two integer seeds of the current generator
     This  is   a  transcription from  Pascal   to  Fortran  of routine
     Get_State from the paper
     L'Ecuyer, P. and  Cote,  S. "Implementing a Random Number  Package
     with   Splitting Facilities."  ACM  Transactions   on Mathematical
     Software, 17:98-111 (1991)
                              Arguments
     iseed1 <- First integer seed of generator G
     iseed2 <- Second integer seed of generator G
**********************************************************************
*/
/*
void Common::GetSeed(int *iseed1, int *iseed2)
{
    int g;
    int qrgnin;

    // Abort unless random number generator initialized
    GetSetGeneratorInitialized(false, &qrgnin);

    if (!qrgnin)
    {
        fprintf(stderr,"%s\n", "GetSeed called before random number generator  initialized -- abort!");
        exit(-1);
    }

    RNG::GetSetArguments(false, &g);
    *iseed1 = Xcg1[g-1];
    *iseed2 = Xcg2[g-1];
}
*/

/*
**********************************************************************
     long ignlgi(void)
               GeNerate LarGe Integer
     Returns a random integer following a uniform distribution over
     (1, 2147483562) using the current generator.
     This is a transcription from Pascal to Fortran of routine
     Random from the paper
     L'Ecuyer, P. and Cote, S. "Implementing a Random Number Package
     with Splitting Facilities." ACM Transactions on Mathematical
     Software, 17:98-111 (1991)
**********************************************************************
*/
int Common::GenerateLargeInteger(void)
{
    int curntg;
    int k, s1, s2, z;
    int qqssd;
    int qrgnin;

    // IF THE RANDOM NUMBER PACKAGE HAS NOT BEEN INITIALIZED YET, DO SO.
    // IT CAN BE INITIALIZED IN ONE OF TWO WAYS : 1) THE FIRST CALL TO
    // THIS ROUTINE  2) A CALL TO SETALL.
    GetSetGeneratorInitialized(false, &qrgnin);
    if (!qrgnin) InitializeGeneratorCommon();

    RNG::GetSetSeedState(false, &qqssd);
    if (!qqssd) SetInitialSeed(1234567890, 123456789);

    // Get Current Generator
    RNG::GetSetArguments(false, &curntg);
    s1 = Xcg1[curntg-1];
    s2 = Xcg2[curntg-1];
    k = s1 / 53668L;
    s1 = Xa1 * (s1 - k * 53668) - k * 12211;

    if (s1 < 0)
    {
        s1 += Xm1;
    }

    k = s2 / 52774L;
    s2 = Xa2 * (s2 - k * 52774) - k * 3791;

    if (s2 < 0)
    {
        s2 += Xm2;
    }

    Xcg1[curntg-1] = s1;
    Xcg2[curntg-1] = s2;
    z = s1-s2;

    if (z < 1)
    {
        z += (Xm1 - 1);
    }

    if (Xqanti[curntg-1])
    {
        z = Xm1 - z;
    }

    return z;
}

/*
**********************************************************************
     void initgn(long isdtyp)
          INIT-ialize current G-e-N-erator
     Reinitializes the state of the current generator
     This is a transcription from Pascal to Fortran of routine
     Init_Generator from the paper
     L'Ecuyer, P. and Cote, S. "Implementing a Random Number Package
     with Splitting Facilities." ACM Transactions on Mathematical
     Software, 17:98-111 (1991)
                              Arguments
     isdtyp -> The state to which the generator is to be set
          isdtyp = -1  => sets the seeds to their initial value
          isdtyp =  0  => sets the seeds to the first value of
                          the current block
          isdtyp =  1  => sets the seeds to the first value of
                          the next block
**********************************************************************
*/
void Common::InitializeGenerator(int isdtyp)
{
    int g;
    int qrgnin;

    // Abort unless random number generator initialized
    GetSetGeneratorInitialized(false, &qrgnin);
    if (!qrgnin)
    {
        fprintf(stderr,"%s\n", "InitializeGenerator called before random number generator  initialized -- abort!");
        exit(-1);
    }

    RNG::GetSetArguments(false, &g);

    switch (isdtyp)
    {
    case -1:
        Xlg1[g-1] = Xig1[g-1];
        Xlg2[g-1] = Xig2[g-1];
        break;

    case 0:
        // No special handling.
        break;

    case 1:
        Xlg1[g-1] = Common::mltmod(Xa1w, Xlg1[g-1], Xm1);
        Xlg2[g-1] = Common::mltmod(Xa2w, Xlg2[g-1], Xm2);
        break;

    default:
        fprintf(stderr,"%s\n","isdtyp not in range in INITGN");
        exit(-1);
        break;
    }

    Xcg1[g-1] = Xlg1[g-1];
    Xcg2[g-1] = Xlg2[g-1];
}

/*
**********************************************************************
     void inrgcm(void)
          INitialize Random number Generator CoMmon
                              Function
     Initializes common area  for random number  generator.  This saves
     the  nuisance  of  a  BLOCK DATA  routine  and the  difficulty  of
     assuring that the routine is loaded with the other routines.
**********************************************************************
*/
void Common::InitializeGeneratorCommon(void)
{
    /*
         V=20;                            W=30;
         A1W = MOD(A1**(2**W),M1)         A2W = MOD(A2**(2**W),M2)
         A1VW = MOD(A1**(2**(V+W)),M1)    A2VW = MOD(A2**(2**(V+W)),M2)
       If V or W is changed A1W, A2W, A1VW, and A2VW need to be recomputed.
        An efficient way to precompute a**(2*j) MOD m is to start with
        a and square it j times modulo m using the function MLTMOD.
    */
    Xm1   = 2147483563L;
    Xm2   = 2147483399L;
    Xa1   = 40014L;
    Xa2   = 40692L;
    Xa1w  = 1033780774L;
    Xa2w  = 1494757890L;
    Xa1vw = 2082007225L;
    Xa2vw = 784306273L;

    Xcg1 = gcnew array<Int32>(numg);
    Xcg2 = gcnew array<Int32>(numg);
    Xig1 = gcnew array<Int32>(numg);
    Xig2 = gcnew array<Int32>(numg);
    Xlg1 = gcnew array<Int32>(numg);
    Xlg2 = gcnew array<Int32>(numg);
    Xqanti = gcnew array<bool>(numg);

    for (int i = 0; i < numg; i++)
        Xqanti[i] = false;

    /*
         Tell the world that common has been initialized
    */
    int T1 = 1;
    GetSetGeneratorInitialized(true, &T1);
}

/*
**********************************************************************
     void setall(long iseed1,long iseed2)
               SET ALL random number generators
     Sets the initial seed of generator 1 to ISEED1 and ISEED2. The
     initial seeds of the other generators are set accordingly, and
     all generators states are set to these seeds.
     This is a transcription from Pascal to Fortran of routine
     Set_Initial_Seed from the paper
     L'Ecuyer, P. and Cote, S. "Implementing a Random Number Package
     with Splitting Facilities." ACM Transactions on Mathematical
     Software, 17:98-111 (1991)
                              Arguments
     iseed1 -> First of two integer seeds
     iseed2 -> Second of two integer seeds
**********************************************************************
*/
void Common::SetInitialSeed(int iseed1, int iseed2)
{
    int T1 = 1;
    int ocgn;
    int qrgnin;

    if (iseed1 == 0) iseed1 = 1234567890;
    if (iseed2 == 0) iseed2 = 123456789;

    /*
         TELL IGNLGI, THE ACTUAL NUMBER GENERATOR, THAT THIS ROUTINE
          HAS BEEN CALLED.
    */
    RNG::GetSetSeedState(true, &T1);
    RNG::GetSetArguments(false, &ocgn);

    /*
         Initialize Common Block if Necessary
    */
    GetSetGeneratorInitialized(false, &qrgnin);

    if (!qrgnin) InitializeGeneratorCommon();

//     *Xig1 = iseed1;
//     *Xig2 = iseed2;
    Xig1[0] = iseed1;
    Xig2[0] = iseed2;
    InitializeGenerator(-1L);

    for (int g = 2; g <= numg; g++) {
        Xig1[g-1] = Common::mltmod(Xa1vw, Xig1[g-2], Xm1);
        Xig2[g-1] = Common::mltmod(Xa2vw, Xig2[g-2], Xm2);
        RNG::GetSetArguments(true, &g);
        InitializeGenerator(-1L);
    }

    RNG::GetSetArguments(true, &ocgn);
}

/*
**********************************************************************
     void setant(long qvalue)
               SET ANTithetic
     Sets whether the current generator produces antithetic values.  If
     X   is  the value  normally returned  from  a uniform [0,1] random
     number generator then 1  - X is the antithetic  value. If X is the
     value  normally  returned  from a   uniform  [0,N]  random  number
     generator then N - 1 - X is the antithetic value.
     All generators are initialized to NOT generate antithetic values.
     This is a transcription from Pascal to Fortran of routine
     Set_Antithetic from the paper
     L'Ecuyer, P. and Cote, S. "Implementing a Random Number Package
     with Splitting Facilities." ACM Transactions on Mathematical
     Software, 17:98-111 (1991)
                              Arguments
     qvalue -> nonzero if generator G is to generating antithetic
                    values, otherwise zero
**********************************************************************
*/
void Common::SetAntithetic(bool qvalue)
{
    int g;
    int qrgnin;

    /*
         Abort unless random number generator initialized
    */
    GetSetGeneratorInitialized(false, &qrgnin);

    if (!qrgnin)
    {
        fprintf(stderr,"%s\n", "SetAntithetic called before random number generator  initialized -- abort!");
        exit(-1);
    }

    RNG::GetSetArguments(false, &g);
    Xqanti[g-1] = qvalue;
}

/*
**********************************************************************
     void setsd(long iseed1,long iseed2)
               SET S-ee-D of current generator
     Resets the initial  seed of  the current  generator to  ISEED1 and
     ISEED2. The seeds of the other generators remain unchanged.
     This is a transcription from Pascal to Fortran of routine
     Set_Seed from the paper
     L'Ecuyer, P. and Cote, S. "Implementing a Random Number Package
     with Splitting Facilities." ACM Transactions on Mathematical
     Software, 17:98-111 (1991)
                              Arguments
     iseed1 -> First integer seed
     iseed2 -> Second integer seed
**********************************************************************
*/
void Common::SetSeed(int iseed1, int iseed2)
{
    int g;
    int qrgnin;
    /*
         Abort unless random number generator initialized
    */
    GetSetGeneratorInitialized(false, &qrgnin);

    if (!qrgnin)
    {
        fprintf(stderr,"%s\n", "SetSeed called before random number generator  initialized -- abort!");
        exit(-1);
    }

    RNG::GetSetArguments(false, &g);
    Xig1[g-1] = iseed1;
    Xig2[g-1] = iseed2;
    InitializeGenerator(-1L);
}

/*
**********************************************************************
     long mltmod(long a,long s,long m)
                    Returns (A*S) MOD M
     This is a transcription from Pascal to Fortran of routine
     MULtMod_Decompos from the paper
     L'Ecuyer, P. and Cote, S. "Implementing a Random Number Package
     with Splitting Facilities." ACM Transactions on Mathematical
     Software, 17:98-111 (1991)
                              Arguments
     a, s, m  -->
**********************************************************************
*/
int Common::mltmod(int a, int s, int m)
{
#define h 32768L
    int a0, a1, k, p, q, qh, rh;

    /*
         H = 2**((b-2)/2) where b = 32 because we are using a 32 bit
          machine. On a different machine recompute H
    */
    if (a <= 0 || a >= m || s <= 0 || s >= m)
    {
        fputs(" a, m, s out of order in mltmod - ABORT!",stderr);
        fprintf(stderr," a = %12ld s = %12ld m = %12ld\n",a,s,m);
        fputs(" mltmod requires: 0 < a < m; 0 < s < m",stderr);
        exit(1);
    }

    if (a < h)
    {
        a0 = a;
        p  = 0;
    }
    else
    {
        a1 = a / h;
        a0 = a - h * a1;
        qh = m / h;
        rh = m - h * qh;

        if (a1 >= h)
        {
            a1 -= h;
            k = s / qh;
            p = h * (s - k * qh) - k * rh;

            while (p < 0)
            {
                p += m;
            }
        }
        else
            p = 0;

        /*
        P = (A2*S*H)MOD M
        */
        if (a1 != 0)
        {
            q = m / a1;
            k = s / q;
            p -= (k * (m - a1 * q));
            if (p > 0) p -= m;

            p += (a1 * (s - k * q));

            while (p < 0)
            {
                p += m;
            }
        }

        k = p / qh;
        /*
        P = ((A2*H + A1)*S)MOD M
        */
        p = h * (p - k * qh) - k * rh;

        while (p < 0)
        {
            p += m;
        }
    }

    if (a0 != 0)
    {
        /*
        P = ((A2*H + A1)*H*S)MOD M
        */
        q = m / a0;
        k = s / q;
        p -= (k * (m - a0 * q));
        if (p > 0) p -= m;

        p += (a0 * (s - k * q));

        while (p < 0)
        {
            p += m;
        }
    }

    return p;

#undef h
}

}

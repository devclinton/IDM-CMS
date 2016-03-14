/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
U T I L I T I E S
* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

#include "stdafx.h"

#include <stdio.h>
#include <stdlib.h>
#include <math.h>
#include <string.h>
#include <time.h>
#include "../include/externs.h"
#include "../include/utilities.h"
#include "../include/generators.h"
#include "../include/stat_fncs.h"

int
displayGeneratorOptions()
{
    int		option = 0;

    printf("           G E N E R A T O R    S E L E C T I O N \n");
    printf("           ______________________________________\n\n");
    printf("    [ 0] Input File                 [ 1] Linear Congruential\n");
    printf("    [ 2] Quadratic Congruential I   [ 3] Quadratic Congruential II\n");
    printf("    [ 4] Cubic Congruential         [ 5] XOR\n");
    printf("    [ 6] Modular Exponentiation     [ 7] Blum-Blum-Shub\n");
    printf("    [ 8] Micali-Schnorr             [ 9] G Using SHA-1\n");
    printf("    [10] EMOD LCG                   [11] RANDLIB\n");
    printf("    [12] Pseudo-DES                 [13] Mersenne Twister\n");
    printf("    [14] Mersenne Twister (64-bit)  [15] TinyMT\n");
    printf("    [16] SIMD Mersenne Twister      [17] AES_CTR\n\n");
    printf("   Enter Choice: ");
#ifdef WIN32
    scanf_s("%d", &option);
#else
    scanf("%d", &option);
#endif
    printf("\n\n");

    return option;
}

int DeterminePRNG(char *prngName, char **streamFile)
{
    int option = 0;

    if (prngName != nullptr) {
        for (int i = 1; i < NUMOFGENERATORS; i++) {
            if (_stricmp(prngName, generatorDir[i]) == 0) {
                option = i;
                *streamFile = generatorDir[i];
                break;
            }
        }
    }
    else {
        option = generatorOptions(streamFile);
    }

    return option;
}

int
generatorOptions(char** streamFile)
{
    char	file[200];
    int		option = NUMOFGENERATORS+1;
    FILE	*fp;
    
    while ( (option < 0) || (option > NUMOFGENERATORS) ) {
        option = displayGeneratorOptions();
        switch( option ) {
            case 0:
                printf("\t\tUser Prescribed Input File: ");
#ifdef WIN32
                scanf_s("%s", file);
#else
                scanf("%s", file);
#endif
                *streamFile = (char*)calloc(200, sizeof(char));
#ifdef WIN32
                sprintf_s(*streamFile, 200, "%s", file);
#else
                sprintf(*streamFile, "%s", file);
#endif
                printf("\n");
#ifdef WIN32
                if (fopen_s(&fp, *streamFile, "r") != 0) {
#else
                if ( (fp = fopen(*streamFile, "r")) == NULL ) {
#endif
                    printf("File Error:  file %s could not be opened.\n",  *streamFile);
                    exit(-1);
                }
                else
                    fclose(fp);
                break;

            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
            case 7:
            case 8:
            case 9:
            case 10:
            case 11:
            case 12:
            case 13:
            case 14:
            case 15:
            case 16:
            case 17:
                *streamFile = generatorDir[option];
                break;

            /* INTRODUCE NEW PRNG NAMES HERE */
            /*
            case 10:  *streamFile = "myNewPRNG";
                break;
            */
            default:
                printf("Error:  Out of range - Try again!\n");
                break;
        }
    }

    return option;
}


void
chooseTests(int vector)
{
    if (vector == 0)
    {
        int i;

        printf("                S T A T I S T I C A L   T E S T S\n");
        printf("                _________________________________\n\n");
        printf("    [01] Frequency                       [02] Block Frequency\n");
        printf("    [03] Cumulative Sums                 [04] Runs\n");
        printf("    [05] Longest Run of Ones             [06] Rank\n");
        printf("    [07] Discrete Fourier Transform      [08] Nonperiodic Template Matchings\n");
        printf("    [09] Overlapping Template Matchings  [10] Universal Statistical\n");
        printf("    [11] Approximate Entropy             [12] Random Excursions\n");
        printf("    [13] Random Excursions Variant       [14] Serial\n");
        printf("    [15] Linear Complexity\n\n");
        printf("         INSTRUCTIONS\n");
        printf("            Enter 0 if you DO NOT want to apply all of the\n");
        printf("            statistical tests to each sequence and 1 if you DO.\n\n");
        printf("   Enter Choice: ");
#ifdef WIN32
        scanf_s("%d", &testVector[0]);
#else
        scanf("%d", &testVector[0]);
#endif
        printf("\n");
        if ( testVector[0] == 1 )
            for( i=1; i<=NUMOFTESTS; i++ )
                testVector[i] = 1;
        else {
            printf("         INSTRUCTIONS\n");
            printf("            Enter a 0 or 1 to indicate whether or not the numbered statistical\n");
            printf("            test should be applied to each sequence.\n\n");
            printf("      123456789111111\n");
            printf("               012345\n");
            printf("      ");
            for ( i=1; i<=NUMOFTESTS; i++ )
#ifdef WIN32
                scanf_s("%1d", &testVector[i]);
#else
                scanf("%1d", &testVector[i]);
#endif
            printf("\n\n");
        }
    }
    else
    {
        for ( int i = 1; i <= NUMOFTESTS; i++ )
        {
            testVector[i] = (vector >> i) & 1;
        }
    }
}


void
fixParameters()
{
    int counter, testid;

    //  Check to see if any parameterized tests are selected
    if ( (testVector[TEST_BLOCK_FREQUENCY] != 1) && (testVector[TEST_NONPERIODIC] != 1) && 
         (testVector[TEST_OVERLAPPING] != 1) && (testVector[TEST_APEN] != 1) &&
         (testVector[TEST_SERIAL] != 1) && (testVector[TEST_LINEARCOMPLEXITY] != 1) )
            return;

    do {
        counter = 1;
        printf("        P a r a m e t e r   A d j u s t m e n t s\n");
        printf("        -----------------------------------------\n");
        if ( testVector[TEST_BLOCK_FREQUENCY] == 1 )
            printf("    [%d] Block Frequency Test - block length(M):         %d\n", counter++, tp.blockFrequencyBlockLength);
        if ( testVector[TEST_NONPERIODIC] == 1 )
            printf("    [%d] NonOverlapping Template Test - block length(m): %d\n", counter++, tp.nonOverlappingTemplateBlockLength);
        if ( testVector[TEST_OVERLAPPING] == 1 )
            printf("    [%d] Overlapping Template Test - block length(m):    %d\n", counter++, tp.overlappingTemplateBlockLength);
        if ( testVector[TEST_APEN] == 1 )
            printf("    [%d] Approximate Entropy Test - block length(m):     %d\n", counter++, tp.approximateEntropyBlockLength);
        if ( testVector[TEST_SERIAL] == 1 )
            printf("    [%d] Serial Test - block length(m):                  %d\n", counter++, tp.serialBlockLength);
        if ( testVector[TEST_LINEARCOMPLEXITY] == 1 )
            printf("    [%d] Linear Complexity Test - block length(M):       %d\n", counter++, tp.linearComplexitySequenceLength);
        printf("\n");
        printf("   Select Test (0 to continue): ");
#ifdef WIN32
        scanf_s("%1d", &testid);
#else
        scanf("%1d", &testid);
#endif
        printf("\n");
        
        counter = 0;
        if ( testVector[TEST_BLOCK_FREQUENCY] == 1 ) {
            counter++;
            if ( counter == testid ) {
                printf("   Enter Block Frequency Test block length: ");
#ifdef WIN32
                scanf_s("%d", &tp.blockFrequencyBlockLength);
#else
                scanf("%d", &tp.blockFrequencyBlockLength);
#endif
                printf("\n");
                continue;
            }
        }
        if ( testVector[TEST_NONPERIODIC] == 1 ) {
            counter++;
            if ( counter == testid ) {
                printf("   Enter NonOverlapping Template Test block Length: ");
#ifdef WIN32
                scanf_s("%d", &tp.nonOverlappingTemplateBlockLength);
#else
                scanf("%d", &tp.nonOverlappingTemplateBlockLength);
#endif
                printf("\n");
                continue;
            }
        }
        if ( testVector[TEST_OVERLAPPING] == 1 ) {
            counter++;
            if ( counter == testid ) {
                printf("   Enter Overlapping Template Test block Length: ");
#ifdef WIN32
                scanf_s("%d", &tp.overlappingTemplateBlockLength);
#else
                scanf("%d", &tp.overlappingTemplateBlockLength);
#endif
                printf("\n");
                continue;
            }
        }
        if ( testVector[TEST_APEN] == 1 ) {
            counter++;
            if ( counter == testid ) {
                printf("   Enter Approximate Entropy Test block Length: ");
#ifdef WIN32
                scanf_s("%d", &tp.approximateEntropyBlockLength);
#else
                scanf("%d", &tp.approximateEntropyBlockLength);
#endif
                printf("\n");
                continue;
            }
        }
        if ( testVector[TEST_SERIAL] == 1 ) {
            counter++;
            if ( counter == testid ) {
                printf("   Enter Serial Test block Length: ");
#ifdef WIN32
                scanf_s("%d", &tp.serialBlockLength);
#else
                scanf("%d", &tp.serialBlockLength);
#endif
                printf("\n");
                continue;
            }
        }
        if ( testVector[TEST_LINEARCOMPLEXITY] == 1 ) {
            counter++;
            if ( counter == testid ) {
                printf("   Enter Linear Complexity Test block Length: ");
#ifdef WIN32
                scanf_s("%d", &tp.linearComplexitySequenceLength);
#else
                scanf("%d", &tp.linearComplexitySequenceLength);
#endif
                printf("\n");
                continue;
            }
        }
    } while ( testid != 0 );
}


void
fileBasedBitStreams(char *streamFile)
{
    FILE	*fp;
    int		mode;
    
    printf("   Input File Format:\n");
    printf("    [0] ASCII - A sequence of ASCII 0's and 1's\n");
    printf("    [1] Binary - Each byte in data file contains 8 bits of data\n\n");
    printf("   Select input mode:  ");
#ifdef WIN32
    scanf_s("%1d", &mode);
#else
    scanf("%1d", &mode);
#endif
    printf("\n");
    if ( mode == 0 ) {
#ifdef WIN32
        if (fopen_s(&fp, streamFile, "r") != 0) {
#else
        if ( (fp = fopen(streamFile, "r")) == NULL ) {
#endif
            printf("ERROR IN FUNCTION fileBasedBitStreams:  file %s could not be opened.\n",  streamFile);
            exit(-1);
        }
        readBinaryDigitsInASCIIFormat(fp, streamFile);
        fclose(fp);
    }
    else if ( mode == 1 ) {
#ifdef WIN32
        if (fopen_s(&fp, streamFile, "rb") != 0) {
#else
        if ( (fp = fopen(streamFile, "rb")) == NULL ) {
#endif
            printf("ERROR IN FUNCTION fileBasedBitStreams:  file %s could not be opened.\n", streamFile);
            exit(-1);
        }
        readHexDigitsInBinaryFormat(fp);
        fclose(fp);
    }
}


void
readBinaryDigitsInASCIIFormat(FILE *fp, char *streamFile)
{
    int		i, j, num_0s, num_1s, bitsRead, bit;
    
    if ( (epsilon = (BitSequence *) calloc(tp.n, sizeof(BitSequence))) == NULL ) {
        printf("BITSTREAM DEFINITION:  Insufficient memory available.\n");
        printf("Statistical Testing Aborted!\n");
        return;
    }
    printf("     Statistical Testing In Progress.........\n\n");   
    for ( i=0; i<tp.numOfBitStreams; i++ ) {
        num_0s = 0;
        num_1s = 0;
        bitsRead = 0;
        for ( j=0; j<tp.n; j++ ) {
#ifdef WIN32
            if (fscanf_s(fp, "%1d", &bit) <= 0) {
#else
            if ( fscanf(fp, "%1d", &bit) == EOF ) {
#endif
                printf("ERROR:  Insufficient data in file %s.  %d bits were read.\n", streamFile, bitsRead);
                fclose(fp);
                free(epsilon);
                return;
            }
            else {
                bitsRead++;
                if ( bit == 0 ) 
                    num_0s++;
                else 
                    num_1s++;
                epsilon[j] = bit;
            }
        }
        fprintf(freqfp, "\t\tBITSREAD = %d 0s = %d 1s = %d\n", bitsRead, num_0s, num_1s);
        nist_test_suite();
    }
    free(epsilon);
}


void
readHexDigitsInBinaryFormat(FILE *fp)
{
    int		i, done, num_0s, num_1s, bitsRead;
    BYTE	buffer[4];
    
    if ( (epsilon = (BitSequence *) calloc(tp.n,sizeof(BitSequence))) == NULL ) {
        printf("BITSTREAM DEFINITION:  Insufficient memory available.\n");
        return;
    }

    printf("     Statistical Testing In Progress.........\n\n");   
    for ( i=0; i<tp.numOfBitStreams; i++ ) {
        num_0s = 0;
        num_1s = 0;
        bitsRead = 0;
        done = 0;
        do {
            if ( fread(buffer, sizeof(unsigned char), 4, fp) != 4 ) {
                printf("READ ERROR:  Insufficient data in file.\n");
                free(epsilon);
                return;
            }
            done = convertToBits(buffer, 32, tp.n, &num_0s, &num_1s, &bitsRead);
        } while ( !done );
        fprintf(freqfp, "\t\tBITSREAD = %d 0s = %d 1s = %d\n", bitsRead, num_0s, num_1s);
        
        nist_test_suite();
        
    }
    free(epsilon);
}


int
convertToBits(BYTE *x, int xBitLength, int bitsNeeded, int *num_0s, int *num_1s, int *bitsRead)
{
    int		i, j, count, bit;
    BYTE	mask;
    int		zeros, ones;

    count = 0;
    zeros = ones = 0;
    for ( i=0; i<(xBitLength+7)/8; i++ ) {
        mask = 0x80;
        for ( j=0; j<8; j++ ) {
            if ( *(x+i) & mask ) {
                bit = 1;
                (*num_1s)++;
                ones++;
            }
            else {
                bit = 0;
                (*num_0s)++;
                zeros++;
            }
            mask >>= 1;
            epsilon[*bitsRead] = bit;
            (*bitsRead)++;
            if ( *bitsRead == bitsNeeded )
                return 1;
            if ( ++count == xBitLength )
                return 0;
        }
    }
    
    return 0;
}

#include <direct.h>

void
openOutputStreams(int option, int numStreams)
{
    int		i, numOfBitStreams, numOfOpenFiles = 0;
    char	freqfn[200], summaryfn[200], statsDir[200], resultsDir[200];

    char currentPath[FILENAME_MAX];
    _getcwd(currentPath, sizeof(currentPath));

#ifdef WIN32
    sprintf_s(freqfn, 200, "experiments/%s/freq.txt", generatorDir[option]);
#else
    sprintf(freqfn, "experiments/%s/freq.txt", generatorDir[option]);
#endif

#ifdef WIN32
    if (fopen_s(&freqfp, freqfn, "w") != 0) {
#else
    if ( (freqfp = fopen(freqfn, "w")) == NULL ) {
#endif
        printf("\t\tMAIN:  Could not open freq file: <%s>", freqfn);
        exit(-1);
    }
#ifdef WIN32
    sprintf_s(summaryfn, 200, "experiments/%s/finalAnalysisReport.txt", generatorDir[option]);
#else
    sprintf(summaryfn, "experiments/%s/finalAnalysisReport.txt", generatorDir[option]);
#endif

#ifdef WIN32
    if (fopen_s(&summary, summaryfn, "w") != 0) {
#else
    if ( (summary = fopen(summaryfn, "w")) == NULL ) {
#endif
        printf("\t\tMAIN:  Could not open stats file: <%s>", summaryfn);
        exit(-1);
    }
    
    for( i=1; i<=NUMOFTESTS; i++ ) {
        if ( testVector[i] == 1 ) {
#ifdef WIN32
            sprintf_s(statsDir, 200, "experiments/%s/%s/stats.txt", generatorDir[option], testNames[i]);
            sprintf_s(resultsDir, 200, "experiments/%s/%s/results.txt", generatorDir[option], testNames[i]);
#else
            sprintf(statsDir, "experiments/%s/%s/stats.txt", generatorDir[option], testNames[i]);
            sprintf(resultsDir, "experiments/%s/%s/results.txt", generatorDir[option], testNames[i]);
#endif

#ifdef WIN32
            if (fopen_s(&stats[i], statsDir, "w") != 0) {
#else
            if ( (stats[i] = fopen(statsDir, "w")) == NULL ) {	/* STATISTICS LOG */
#endif
                printf("ERROR: LOG FILES COULD NOT BE OPENED.\n");
                printf("       MAX # OF OPENED FILES HAS BEEN REACHED = %d\n", numOfOpenFiles);
                printf("-OR-   THE OUTPUT DIRECTORY DOES NOT EXIST.\n");
                exit(-1);
            }
            else
                numOfOpenFiles++;

#ifdef WIN32
            if (fopen_s(&results[i], resultsDir, "w") != 0) {
#else   
            if ( (results[i] = fopen(resultsDir, "w")) == NULL ) {	/* P_VALUES LOG   */
#endif
                 printf("ERROR: LOG FILES COULD NOT BE OPENED.\n");
                 printf("       MAX # OF OPENED FILES HAS BEEN REACHED = %d\n", numOfOpenFiles);
                 printf("-OR-   THE OUTPUT DIRECTORY DOES NOT EXIST.\n");
                 exit(-1);
            }
            else
                numOfOpenFiles++;
        }
    }

    if (numStreams <= 0)
    {
        printf("   How many bitstreams? ");
#ifdef WIN32
        scanf_s("%d", &numOfBitStreams);
#else
        scanf("%d", &numOfBitStreams);
#endif
        tp.numOfBitStreams = numOfBitStreams;
        printf("\n");
    }
    else {
        tp.numOfBitStreams = numStreams;
    }
}


void
invokeTestSuite(int option, char *streamFile)
{
    fprintf(freqfp, "________________________________________________________________________________\n\n");
    fprintf(freqfp, "\t\tFILE = %s\t\tALPHA = %6.4f\n", streamFile, ALPHA);
    fprintf(freqfp, "________________________________________________________________________________\n\n");
    if ( option != 0 )
        printf("     Statistical Testing In Progress.........\n\n");
    switch( option ) {
        case 0:
            fileBasedBitStreams(streamFile);
            break;
        case 1:
            lcg();
            break;
        case 2:
            quadRes1();
            break;
        case 3:
            quadRes2();
            break;
        case 4:
            cubicRes();
            break;
        case 5:
            exclusiveOR();
            break;
        case 6:
            modExp();
            break;
        case 7:
            bbs();
            break;
        case 8:
            micali_schnorr();
            break;
        case 9:
            SHA1();
            break;

        case 10:
            EmodLCG();
            break;

        case 11:
            RandLib();
            break;

        case 12:
            EmodPDES();
            break;

        case 13:
            MersenneTwister();
            break;

        case 14:
            MT64();
            break;

        case 15:
            TinyMT();
            break;

        case 16:
            SFMT();
            break;

        case 17:
            AES_CTR();
            break;

        /* INTRODUCE NEW PSEUDO RANDOM NUMBER GENERATORS HERE */
            
        default:
            printf("Error in invokeTestSuite!\n");
            break;
    }
    printf("     Statistical Testing Complete!!!!!!!!!!!!\n\n");
}


void
nist_test_suite()
{
    if ( (testVector[0] == 1) || (testVector[TEST_FREQUENCY] == 1) ) 
        Frequency(tp.n);
    
    if ( (testVector[0] == 1) || (testVector[TEST_BLOCK_FREQUENCY] == 1) ) 
        BlockFrequency(tp.blockFrequencyBlockLength, tp.n);
    
    if ( (testVector[0] == 1) || (testVector[TEST_CUSUM] == 1) )
        CumulativeSums(tp.n);
    
    if ( (testVector[0] == 1) || (testVector[TEST_RUNS] == 1) )
        Runs(tp.n); 
    
    if ( (testVector[0] == 1) || (testVector[TEST_LONGEST_RUN] == 1) )
        LongestRunOfOnes(tp.n);
    
    if ( (testVector[0] == 1) || (testVector[TEST_RANK] == 1) )
        Rank(tp.n);
    
    if ( (testVector[0] == 1) || (testVector[TEST_FFT] == 1) )
        DiscreteFourierTransform(tp.n);
    
    if ( (testVector[0] == 1) || (testVector[TEST_NONPERIODIC] == 1) )
        NonOverlappingTemplateMatchings(tp.nonOverlappingTemplateBlockLength, tp.n);
    
    if ( (testVector[0] == 1) || (testVector[TEST_OVERLAPPING] == 1) )
        OverlappingTemplateMatchings(tp.overlappingTemplateBlockLength, tp.n);
    
    if ( (testVector[0] == 1) || (testVector[TEST_UNIVERSAL] == 1) )
        Universal(tp.n);
    
    if ( (testVector[0] == 1) || (testVector[TEST_APEN] == 1) )
        ApproximateEntropy(tp.approximateEntropyBlockLength, tp.n);
    
    if ( (testVector[0] == 1) || (testVector[TEST_RND_EXCURSION] == 1) )
        RandomExcursions(tp.n);
    
    if ( (testVector[0] == 1) || (testVector[TEST_RND_EXCURSION_VAR] == 1) )
        RandomExcursionsVariant(tp.n);
    
    if ( (testVector[0] == 1) || (testVector[TEST_SERIAL] == 1) )
        Serial(tp.serialBlockLength,tp.n);
    
    if ( (testVector[0] == 1) || (testVector[TEST_LINEARCOMPLEXITY] == 1) )
        LinearComplexity(tp.linearComplexitySequenceLength, tp.n);
}

/*
 * Copied from OpenBSD project (src/lib/libm/src/s_erf.c)
 * Specialized for 32-bit little endian architectures.
 */

/* Real math libraries provide erf(), CUDA also provides an implementation. */
#if defined(WIN32) && !defined(NAMD_CUDA)

/*
 * ====================================================
 * Copyright (C) 1993 by Sun Microsystems, Inc. All rights reserved.
 *
 * Developed at SunPro, a Sun Microsystems, Inc. business.
 * Permission to use, copy, modify, and distribute this
 * software is freely granted, provided that this notice 
 * is preserved.
 * ====================================================
 */

/* double erf(double x)
 * double erfc(double x)
 *                           x
 *                    2      |\
 *     erf(x)  =  ---------  | exp(-t*t)dt
 *                 sqrt(pi) \| 
 *                           0
 *
 *     erfc(x) =  1-erf(x)
 *  Note that 
 *              erf(-x) = -erf(x)
 *              erfc(-x) = 2 - erfc(x)
 *
 * Method:
 *      1. For |x| in [0, 0.84375]
 *          erf(x)  = x + x*R(x^2)
 *          erfc(x) = 1 - erf(x)           if x in [-.84375,0.25]
 *                  = 0.5 + ((0.5-x)-x*R)  if x in [0.25,0.84375]
 *         where R = P/Q where P is an odd poly of degree 8 and
 *         Q is an odd poly of degree 10.
 *                                               -57.90
 *                      | R - (erf(x)-x)/x | <= 2
 *      
 *
 *         Remark. The formula is derived by noting
 *          erf(x) = (2/sqrt(pi))*(x - x^3/3 + x^5/10 - x^7/42 + ....)
 *         and that
 *          2/sqrt(pi) = 1.128379167095512573896158903121545171688
 *         is close to one. The interval is chosen because the fix
 *         point of erf(x) is near 0.6174 (i.e., erf(x)=x when x is
 *         near 0.6174), and by some experiment, 0.84375 is chosen to
 *         guarantee the error is less than one ulp for erf.
 *
 *      2. For |x| in [0.84375,1.25], let s = |x| - 1, and
 *         c = 0.84506291151 rounded to single (24 bits)
 *              erf(x)  = sign(x) * (c  + P1(s)/Q1(s))
 *              erfc(x) = (1-c)  - P1(s)/Q1(s) if x > 0
 *                        1+(c+P1(s)/Q1(s))    if x < 0
 *              |P1/Q1 - (erf(|x|)-c)| <= 2**-59.06
 *         Remark: here we use the taylor series expansion at x=1.
 *              erf(1+s) = erf(1) + s*Poly(s)
 *                       = 0.845.. + P1(s)/Q1(s)
 *         That is, we use rational approximation to approximate
 *                      erf(1+s) - (c = (single)0.84506291151)
 *         Note that |P1/Q1|< 0.078 for x in [0.84375,1.25]
 *         where 
 *              P1(s) = degree 6 poly in s
 *              Q1(s) = degree 6 poly in s
 *
 *      3. For x in [1.25,1/0.35(~2.857143)], 
 *              erfc(x) = (1/x)*exp(-x*x-0.5625+R1/S1)
 *              erf(x)  = 1 - erfc(x)
 *         where 
 *              R1(z) = degree 7 poly in z, (z=1/x^2)
 *              S1(z) = degree 8 poly in z
 *
 *      4. For x in [1/0.35,28]
 *              erfc(x) = (1/x)*exp(-x*x-0.5625+R2/S2) if x > 0
 *                      = 2.0 - (1/x)*exp(-x*x-0.5625+R2/S2) if -6<x<0
 *                      = 2.0 - tiny            (if x <= -6)
 *              erf(x)  = sign(x)*(1.0 - erfc(x)) if x < 6, else
 *              erf(x)  = sign(x)*(1.0 - tiny)
 *         where
 *              R2(z) = degree 6 poly in z, (z=1/x^2)
 *              S2(z) = degree 7 poly in z
 *
 *      Note1:
 *         To compute exp(-x*x-0.5625+R/S), let s be a single
 *         precision number and s := x; then
 *              -x*x = -s*s + (s-x)*(s+x)
 *              exp(-x*x-0.5626+R/S) = 
 *                      exp(-s*s-0.5625)*exp((s-x)*(s+x)+R/S);
 *      Note2:
 *         Here 4 and 5 make use of the asymptotic series
 *                        exp(-x*x)
 *              erfc(x) ~ ---------- * ( 1 + Poly(1/x^2) )
 *                        x*sqrt(pi)
 *         We use rational approximation to approximate
 *              g(s)=f(1/x^2) = log(erfc(x)*x) - x*x + 0.5625
 *         Here is the error bound for R1/S1 and R2/S2
 *              |R1/S1 - f(x)|  < 2**(-62.57)
 *              |R2/S2 - f(x)|  < 2**(-61.52)
 *
 *      5. For inf > x >= 28
 *              erf(x)  = sign(x) *(1 - tiny)  (raise inexact)
 *              erfc(x) = tiny*tiny (raise underflow) if x > 0
 *                      = 2 - tiny if x<0
 *
 *      7. Special case:
 *              erf(0)  = 0, erf(inf)  = 1, erf(-inf) = -1,
 *              erfc(0) = 1, erfc(inf) = 0, erfc(-inf) = 2, 
 *              erfc/erf(NaN) is NaN
 */

#include <math.h>

extern "C" {

/*  assume 32 bit int  */

typedef int int32_t;
typedef unsigned int u_int32_t;

/*  assume little endian  */
typedef union 
{
  double value;
  struct 
  {
    u_int32_t lsw;
    u_int32_t msw;
  } parts;
} ieee_double_shape_type;


/* Get the more significant 32 bit int from a double.  */

#define GET_HIGH_WORD(i,d)                                      \
do {                                                            \
  ieee_double_shape_type gh_u;                                  \
  gh_u.value = (d);                                             \
  (i) = gh_u.parts.msw;                                         \
} while (0)


/* Set the less significant 32 bits of a double from an int.  */

#define SET_LOW_WORD(d,v)                                       \
do {                                                            \
  ieee_double_shape_type sl_u;                                  \
  sl_u.value = (d);                                             \
  sl_u.parts.lsw = (v);                                         \
  (d) = sl_u.value;                                             \
} while (0)


/* Eliminate reference to internal OpenBSD call  */

#define __ieee754_exp(X) exp(X)


static const double
tiny        = 1e-300,
half=  5.00000000000000000000e-01, /* 0x3FE00000, 0x00000000 */
one =  1.00000000000000000000e+00, /* 0x3FF00000, 0x00000000 */
two =  2.00000000000000000000e+00, /* 0x40000000, 0x00000000 */
        /* c = (float)0.84506291151 */
erx =  8.45062911510467529297e-01, /* 0x3FEB0AC1, 0x60000000 */
/*
 * Coefficients for approximation to  erf on [0,0.84375]
 */
efx =  1.28379167095512586316e-01, /* 0x3FC06EBA, 0x8214DB69 */
efx8=  1.02703333676410069053e+00, /* 0x3FF06EBA, 0x8214DB69 */
pp0  =  1.28379167095512558561e-01, /* 0x3FC06EBA, 0x8214DB68 */
pp1  = -3.25042107247001499370e-01, /* 0xBFD4CD7D, 0x691CB913 */
pp2  = -2.84817495755985104766e-02, /* 0xBF9D2A51, 0xDBD7194F */
pp3  = -5.77027029648944159157e-03, /* 0xBF77A291, 0x236668E4 */
pp4  = -2.37630166566501626084e-05, /* 0xBEF8EAD6, 0x120016AC */
qq1  =  3.97917223959155352819e-01, /* 0x3FD97779, 0xCDDADC09 */
qq2  =  6.50222499887672944485e-02, /* 0x3FB0A54C, 0x5536CEBA */
qq3  =  5.08130628187576562776e-03, /* 0x3F74D022, 0xC4D36B0F */
qq4  =  1.32494738004321644526e-04, /* 0x3F215DC9, 0x221C1A10 */
qq5  = -3.96022827877536812320e-06, /* 0xBED09C43, 0x42A26120 */
/*
 * Coefficients for approximation to  erf  in [0.84375,1.25] 
 */
pa0  = -2.36211856075265944077e-03, /* 0xBF6359B8, 0xBEF77538 */
pa1  =  4.14856118683748331666e-01, /* 0x3FDA8D00, 0xAD92B34D */
pa2  = -3.72207876035701323847e-01, /* 0xBFD7D240, 0xFBB8C3F1 */
pa3  =  3.18346619901161753674e-01, /* 0x3FD45FCA, 0x805120E4 */
pa4  = -1.10894694282396677476e-01, /* 0xBFBC6398, 0x3D3E28EC */
pa5  =  3.54783043256182359371e-02, /* 0x3FA22A36, 0x599795EB */
pa6  = -2.16637559486879084300e-03, /* 0xBF61BF38, 0x0A96073F */
qa1  =  1.06420880400844228286e-01, /* 0x3FBB3E66, 0x18EEE323 */
qa2  =  5.40397917702171048937e-01, /* 0x3FE14AF0, 0x92EB6F33 */
qa3  =  7.18286544141962662868e-02, /* 0x3FB2635C, 0xD99FE9A7 */
qa4  =  1.26171219808761642112e-01, /* 0x3FC02660, 0xE763351F */
qa5  =  1.36370839120290507362e-02, /* 0x3F8BEDC2, 0x6B51DD1C */
qa6  =  1.19844998467991074170e-02, /* 0x3F888B54, 0x5735151D */
/*
 * Coefficients for approximation to  erfc in [1.25,1/0.35]
 */
ra0  = -9.86494403484714822705e-03, /* 0xBF843412, 0x600D6435 */
ra1  = -6.93858572707181764372e-01, /* 0xBFE63416, 0xE4BA7360 */
ra2  = -1.05586262253232909814e+01, /* 0xC0251E04, 0x41B0E726 */
ra3  = -6.23753324503260060396e+01, /* 0xC04F300A, 0xE4CBA38D */
ra4  = -1.62396669462573470355e+02, /* 0xC0644CB1, 0x84282266 */
ra5  = -1.84605092906711035994e+02, /* 0xC067135C, 0xEBCCABB2 */
ra6  = -8.12874355063065934246e+01, /* 0xC0545265, 0x57E4D2F2 */
ra7  = -9.81432934416914548592e+00, /* 0xC023A0EF, 0xC69AC25C */
sa1  =  1.96512716674392571292e+01, /* 0x4033A6B9, 0xBD707687 */
sa2  =  1.37657754143519042600e+02, /* 0x4061350C, 0x526AE721 */
sa3  =  4.34565877475229228821e+02, /* 0x407B290D, 0xD58A1A71 */
sa4  =  6.45387271733267880336e+02, /* 0x40842B19, 0x21EC2868 */
sa5  =  4.29008140027567833386e+02, /* 0x407AD021, 0x57700314 */
sa6  =  1.08635005541779435134e+02, /* 0x405B28A3, 0xEE48AE2C */
sa7  =  6.57024977031928170135e+00, /* 0x401A47EF, 0x8E484A93 */
sa8  = -6.04244152148580987438e-02, /* 0xBFAEEFF2, 0xEE749A62 */
/*
 * Coefficients for approximation to  erfc in [1/.35,28]
 */
rb0  = -9.86494292470009928597e-03, /* 0xBF843412, 0x39E86F4A */
rb1  = -7.99283237680523006574e-01, /* 0xBFE993BA, 0x70C285DE */
rb2  = -1.77579549177547519889e+01, /* 0xC031C209, 0x555F995A */
rb3  = -1.60636384855821916062e+02, /* 0xC064145D, 0x43C5ED98 */
rb4  = -6.37566443368389627722e+02, /* 0xC083EC88, 0x1375F228 */
rb5  = -1.02509513161107724954e+03, /* 0xC0900461, 0x6A2E5992 */
rb6  = -4.83519191608651397019e+02, /* 0xC07E384E, 0x9BDC383F */
sb1  =  3.03380607434824582924e+01, /* 0x403E568B, 0x261D5190 */
sb2  =  3.25792512996573918826e+02, /* 0x40745CAE, 0x221B9F0A */
sb3  =  1.53672958608443695994e+03, /* 0x409802EB, 0x189D5118 */
sb4  =  3.19985821950859553908e+03, /* 0x40A8FFB7, 0x688C246A */
sb5  =  2.55305040643316442583e+03, /* 0x40A3F219, 0xCEDF3BE6 */
sb6  =  4.74528541206955367215e+02, /* 0x407DA874, 0xE79FE763 */
sb7  = -2.24409524465858183362e+01; /* 0xC03670E2, 0x42712D62 */

double erf(double x) 
{
        int32_t hx,ix,i;
        double R,S,P,Q,s,y,z,r;
        GET_HIGH_WORD(hx,x);
        ix = hx&0x7fffffff;
        if(ix>=0x7ff00000) {            /* erf(nan)=nan */
            i = ((u_int32_t)hx>>31)<<1;
            return (double)(1-i)+one/x; /* erf(+-inf)=+-1 */
        }

        if(ix < 0x3feb0000) {           /* |x|<0.84375 */
            if(ix < 0x3e300000) {       /* |x|<2**-28 */
                if (ix < 0x00800000) 
                    return 0.125*(8.0*x+efx8*x);  /*avoid underflow */
                return x + efx*x;
            }
            z = x*x;
            r = pp0+z*(pp1+z*(pp2+z*(pp3+z*pp4)));
            s = one+z*(qq1+z*(qq2+z*(qq3+z*(qq4+z*qq5))));
            y = r/s;
            return x + x*y;
        }
        if(ix < 0x3ff40000) {           /* 0.84375 <= |x| < 1.25 */
            s = fabs(x)-one;
            P = pa0+s*(pa1+s*(pa2+s*(pa3+s*(pa4+s*(pa5+s*pa6)))));
            Q = one+s*(qa1+s*(qa2+s*(qa3+s*(qa4+s*(qa5+s*qa6)))));
            if(hx>=0) return erx + P/Q; else return -erx - P/Q;
        }
        if (ix >= 0x40180000) {         /* inf>|x|>=6 */
            if(hx>=0) return one-tiny; else return tiny-one;
        }
        x = fabs(x);
        s = one/(x*x);
        if(ix< 0x4006DB6E) {    /* |x| < 1/0.35 */
            R=ra0+s*(ra1+s*(ra2+s*(ra3+s*(ra4+s*(
                                ra5+s*(ra6+s*ra7))))));
            S=one+s*(sa1+s*(sa2+s*(sa3+s*(sa4+s*(
                                sa5+s*(sa6+s*(sa7+s*sa8)))))));
        } else {        /* |x| >= 1/0.35 */
            R=rb0+s*(rb1+s*(rb2+s*(rb3+s*(rb4+s*(
                                rb5+s*rb6)))));
            S=one+s*(sb1+s*(sb2+s*(sb3+s*(sb4+s*(
                                sb5+s*(sb6+s*sb7))))));
        }
        z  = x;  
        SET_LOW_WORD(z,0);
        r  =  __ieee754_exp(-z*z-0.5625)*__ieee754_exp((z-x)*(z+x)+R/S);
        if(hx>=0) return one-r/x; else return  r/x-one;
}

double erfc(double x) 
{
        int32_t hx,ix;
        double R,S,P,Q,s,y,z,r;
        GET_HIGH_WORD(hx,x);
        ix = hx&0x7fffffff;
        if(ix>=0x7ff00000) {                    /* erfc(nan)=nan */
                                                /* erfc(+-inf)=0,2 */
            return (double)(((u_int32_t)hx>>31)<<1)+one/x;
        }

        if(ix < 0x3feb0000) {           /* |x|<0.84375 */
            if(ix < 0x3c700000)         /* |x|<2**-56 */
                return one-x;
            z = x*x;
            r = pp0+z*(pp1+z*(pp2+z*(pp3+z*pp4)));
            s = one+z*(qq1+z*(qq2+z*(qq3+z*(qq4+z*qq5))));
            y = r/s;
            if(hx < 0x3fd00000) {       /* x<1/4 */
                return one-(x+x*y);
            } else {
                r = x*y;
                r += (x-half);
                return half - r ;
            }
        }
        if(ix < 0x3ff40000) {           /* 0.84375 <= |x| < 1.25 */
            s = fabs(x)-one;
            P = pa0+s*(pa1+s*(pa2+s*(pa3+s*(pa4+s*(pa5+s*pa6)))));
            Q = one+s*(qa1+s*(qa2+s*(qa3+s*(qa4+s*(qa5+s*qa6)))));
            if(hx>=0) {
                z  = one-erx; return z - P/Q; 
            } else {
                z = erx+P/Q; return one+z;
            }
        }
        if (ix < 0x403c0000) {          /* |x|<28 */
            x = fabs(x);
            s = one/(x*x);
            if(ix< 0x4006DB6D) {        /* |x| < 1/.35 ~ 2.857143*/
                R=ra0+s*(ra1+s*(ra2+s*(ra3+s*(ra4+s*(
                                ra5+s*(ra6+s*ra7))))));
                S=one+s*(sa1+s*(sa2+s*(sa3+s*(sa4+s*(
                                sa5+s*(sa6+s*(sa7+s*sa8)))))));
            } else {                    /* |x| >= 1/.35 ~ 2.857143 */
                if(hx<0&&ix>=0x40180000) return two-tiny;/* x < -6 */
                R=rb0+s*(rb1+s*(rb2+s*(rb3+s*(rb4+s*(
                                rb5+s*rb6)))));
                S=one+s*(sb1+s*(sb2+s*(sb3+s*(sb4+s*(
                                sb5+s*(sb6+s*sb7))))));
            }
            z  = x;
            SET_LOW_WORD(z,0);
            r  =  __ieee754_exp(-z*z-0.5625)*
                        __ieee754_exp((z-x)*(z+x)+R/S);
            if(hx>0) return r/x; else return two-r/x;
        } else {
            if(hx>0) return tiny*tiny; else return two-tiny;
        }
}

}

#else  /* WIN32 */

int dummy_erf(int i) { return i; }  /* avoid empty translation unit */

#endif  /* WIN32 */


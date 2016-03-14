/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
              U T I L I T Y  F U N C T I O N  P R O T O T Y P E S 
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

#include "config.h"

int     displayGeneratorOptions();
int     DeterminePRNG(char *prngName, char **streamFile);
int     generatorOptions(char** streamFile);
void    chooseTests(int vector);
void    fixParameters();
void    fileBasedBitStreams(char *streamFile);
void    readBinaryDigitsInASCIIFormat(FILE *fp, char *streamFile);
void    readHexDigitsInBinaryFormat(FILE *fp);
int     convertToBits(BYTE *x, int xBitLength, int bitsNeeded, int *num_0s, int *num_1s, int *bitsRead);
void    openOutputStreams(int option, int numStreams);
void    invokeTestSuite(int option, char *streamFile);
void    nist_test_suite();

extern "C"
{
    double  erf(double);
    double  erfc(double);
}

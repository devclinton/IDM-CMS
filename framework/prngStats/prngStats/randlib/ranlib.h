/* Prototypes for all user accessible RANLIB routines */

#define numg 32L

class RNG {
private:
    static float fsign(float num, float sign);
    static void  ftnstop(char*);

public:
    static float GenerateBeta(float aa,float bb);
    static int   GenerateBinomial(int n, float pp);
    static float GenerateChiSquare(float df);
    static float GenerateExponential(float av);
    static float GenerateF(float dfn, float dfd);
    static float GenerateGamma(float scale, float shape);
    static void  GenerateMultinomial(int n, float p[], int ncat, int ix[]);
    static void  GenerateMultinomial2(int n, float p[],  int ncat, int ix[]);
    static void  GenerateMultivariateNormal(float *parm, float *x, float *work);
    static int   GenerateNegativeBinomial(int n, float p);
    static float GenerateNoncentralChiSquare(float df, float xnonc);
    static float GenerateNoncentralF(float dfn, float dfd, float xnonc);
    static float GenerateNormal(float av, float sd);
    static void  GeneratePermutation(int iarray[], int larray);
    static int   GeneratePoisson(float mu);
    static float GenerateUniform(float low, float high);
    static float GenerateUniformFloat01(void);
    static int   GenerateUniformInteger(int low, int high);

    static void GetSetArguments(bool fSet, int *g);
    static void GetSetSeedState(bool fSet, int *qset);

    static void PhraseToSeeds(char* phrase, int *seed1, int *seed2);

    static void  SetGenerateMultivariateNormal(float *meanv, float *covm, int p, float *parm);
    static float StandardExponential(void);
    static float StandardGamma(float a);
    static float StandardNormal(void);
};

class Common {
private:
    static int Xm1, Xm2;
    static int Xa1, Xa2;
    static int Xcg1[numg], Xcg2[numg];
    static int Xa1w, Xa2w;
    static int Xig1[numg], Xig2[numg];
    static int Xlg1[numg], Xlg2[numg];
    static int Xa1vw,Xa2vw;
    static bool Xqanti[numg];

    static int mltmod(int a, int s, int m);

public:
    static void AdvanceState(int k);

    static int GenerateLargeInteger(void);

    static void GetSeed(int *iseed1, int *iseed2);
    static void GetSetGeneratorInitialized(bool fSet, int *qvalue);

    static void InitializeGenerator(int isdtyp);
    static void InitializeGeneratorCommon(void);

    static void SetAntithetic(bool qvalue);
    static void SetInitialSeed(int iseed1, int iseed2);
    static void SetSeed(int iseed1, int iseed2);
};

class Linpack {
public:
    static void FactorSymmetricPositiveMatrix(float *a, int lda, int n, int *info);
};

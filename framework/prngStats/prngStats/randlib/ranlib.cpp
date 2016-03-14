#include "stdafx.h"

#include "ranlib.h"
#include <stdio.h>
#include <math.h>
#include <stdlib.h>
#define ABS(x) ((x) >= 0 ? (x) : -(x))
#define min(a,b) ((a) <= (b) ? (a) : (b))
#define max(a,b) ((a) >= (b) ? (a) : (b))

#define square(x)   (x * x)
#define cube(x)     (x * x * x)

#define _expmax 89.0
#define _infnty 1.0E38f

/*
**********************************************************************
     float genbet(float aa,float bb)
               GeNerate BETa random deviate
                              Function
     Returns a single random deviate from the beta distribution with
     parameters A and B.  The density of the beta is
               x^(a-1) * (1-x)^(b-1) / B(a,b) for 0 < x < 1
                              Arguments
     aa --> First parameter of the beta distribution
       
     bb --> Second parameter of the beta distribution
       
                              Method
     R. C. H. Cheng
     Generating Beta Variatew with Nonintegral Shape Parameters
     Communications of the ACM, 21:317-322  (1978)
     (Algorithms BB and BC)
**********************************************************************
*/
float RNG::GenerateBeta(float aa,float bb)
{
#ifdef BETA_CACHING
    static float olda = -1.0;
    static float oldb = -1.0;
    bool qsame;
#endif
    float genbet, a, alpha, b, beta, delta, gamma, k1, k2, r, s, t, u1, u2, v, w, y, z;

#ifdef BETA_CACHING
    qsame = ((olda == aa) && (oldb == bb));
    if (!qsame)
    {
#endif
        if (aa <= 0.0 || bb <= 0.0)
        {
            fputs(" AA or BB <= 0 in GENBET - Abort!",stderr);
            fprintf(stderr," AA: %16.6E BB %16.6E\n",aa,bb);
            exit(-1);
        }
#ifdef BETA_CACHING
        olda = aa;
        oldb = bb;
    }
#endif

    if (!(min(aa,bb) > 1.0)) goto S100;
/*
     Algorithm BB
     Initialize
*/
#ifdef BETA_CACHING
    if (!qsame)
#endif
    {
        a = min(aa,bb);
        b = max(aa,bb);
        alpha = a + b;
        beta = (float)(sqrt((alpha - 2.0)/(2.0 * a * b - alpha)));
        gamma = a + 1.0f / beta;
    }

S40:
    u1 = RNG::GenerateUniformFloat01();
/*
     Step 1
*/
    u2 = RNG::GenerateUniformFloat01();
    v = (float)(beta * log(u1 / (1.0 - u1)));

    if (v > _expmax)
    {
        w = _infnty;
    }
    else
        w = a * exp(v);

    z = pow(u1, 2.0f) * u2;
    r = gamma * v - 1.3862944f;
    s = a + r - w;

    /*
         Step 2
    */
    if ((s + 2.609438f) < (5.0f * z))
    {
        /*
        Step 3
        */
        t = log(z);

        if (s <= t)
        {
            /*
            Step 4
            */
            if ((r + alpha * log(alpha / (b + w))) < t) goto S40;
        }
    }

    /*
         Step 5
    */
    if (aa == a)
        genbet = w / (b + w);
    else
        genbet = b / (b + w);

    goto S230;

S100:
    /*
         Algorithm BC
         Initialize
    */
#ifdef BETA_CACHING
    if (!qsame)
#endif
    {
        a     = max(aa,bb);
        b     = min(aa,bb);
        alpha = a + b;
        beta  = 1.0f / b;
        delta = 1.0f + a - b;
        k1    = delta * (1.38889E-2f + 4.16667E-2f * b) / (a * beta - 0.777778f);
        k2    = 0.25f + (0.5f + 0.25f / delta) * b;
    }

S120:
    /*
         Step 1
    */
    u1 = RNG::GenerateUniformFloat01();
    u2 = RNG::GenerateUniformFloat01();

    if (u1 < 0.5)
    {
        /*
        Step 2
        */
        y = u1 * u2;
        z = u1 * y;

        if (0.25 * u2 + z - y >= k1) goto S120;
        goto S170;
    }
/*
     Step 3
*/
    z = pow(u1, 2.0f) * u2;

    if (!(z <= 0.25)) goto S160;

    v = (float)(beta * log(u1 / (1.0 - u1)));

    if (v > _expmax)
        w = _infnty;
    else
        w = a * exp(v);

    goto S200;

S160:
    if (z >= k2) goto S120;

S170:
/*
     Step 4
     Step 5
*/
    v = (float)(beta * log(u1 / (1.0 - u1)));

    if (v > _expmax)
        w = _infnty;
    else
        w = a * exp(v);

    if ((alpha * (log(alpha / (b + w)) + v) - 1.3862944) < log(z)) goto S120;

S200:
/*
     Step 6
*/
    if (a == aa)
        genbet = w / (b + w);
    else
        genbet = b / (b + w);

S230:
    return genbet;
}

/*
**********************************************************************
     float genchi(float df)
                Generate random value of CHIsquare variable
                              Function
     Generates random deviate from the distribution of a chisquare
     with DF degrees of freedom random variable.
                              Arguments
     df --> Degrees of freedom of the chisquare
            (Must be positive)
       
                              Method
     Uses relation between chisquare and gamma.
**********************************************************************
*/
float RNG::GenerateChiSquare(float df)
{
    float genchi;

    if (df <= 0.0)
    {
        fputs("DF <= 0 in GENCHI - ABORT",stderr);
        fprintf(stderr,"Value of DF: %16.6E\n",df);
        exit(11);
    }

    genchi = 2.0f * GenerateGamma(1.0f, df / 2.0f);
    return genchi;
}

/*
**********************************************************************
     float genexp(float av)
                    GENerate EXPonential random deviate
                              Function
     Generates a single random deviate from an exponential
     distribution with mean AV.
                              Arguments
     av --> The mean of the exponential distribution from which
            a random deviate is to be generated.
                              Method
     Renames SEXPO from TOMS as slightly modified by BWB to use RANF
     instead of SUNIF.
     For details see:
               Ahrens, J.H. and Dieter, U.
               Computer Methods for Sampling From the
               Exponential and Normal Distributions.
               Comm. ACM, 15,10 (Oct. 1972), 873 - 882.
**********************************************************************
*/
float RNG::GenerateExponential(float av)
{
    float genexp;

    genexp = StandardExponential() * av;
    return genexp;
}

/*
**********************************************************************
     float genf(float dfn,float dfd)
                GENerate random deviate from the F distribution
                              Function
     Generates a random deviate from the F (variance ratio)
     distribution with DFN degrees of freedom in the numerator
     and DFD degrees of freedom in the denominator.
                              Arguments
     dfn --> Numerator degrees of freedom
             (Must be positive)
     dfd --> Denominator degrees of freedom
             (Must be positive)
                              Method
     Directly generates ratio of chisquare variates
**********************************************************************
*/
float RNG::GenerateF(float dfn,float dfd)
{
    float genf, xden, xnum;

    if ((dfn <= 0.0) || (dfd <= 0.0))
    {
        fputs("Degrees of freedom nonpositive in GENF - abort!",stderr);
        fprintf(stderr, "DFN value: %16.6EDFD value: %16.6E\n", dfn, dfd);
        exit(-1);
    }

    xnum = RNG::GenerateChiSquare(dfn) / dfn;

    /*
          GENF = ( GENCHI( DFN ) / DFN ) / ( GENCHI( DFD ) / DFD )
    */
    xden = RNG::GenerateChiSquare(dfd) / dfd;

    if (xden <= (9.999999999998E-39 * xnum))
    {
        fputs(" GENF - generated numbers would cause overflow", stderr);
        fprintf(stderr, " Numerator %16.6E Denominator %16.6E\n", xnum, xden);
        fputs(" GENF returning 1.0E38", stderr);
        genf = _infnty;
    }
    else
        genf = xnum / xden;

    return genf;
}

/*
**********************************************************************
     float gengam(float a,float r)
           GENerates random deviates from GAMma distribution
                              Function
     Generates random deviates from the gamma distribution whose
     density is
          (A**R)/Gamma(R) * X**(R-1) * Exp(-A*X)
                              Arguments
     a --> Location parameter of Gamma distribution
     r --> Shape parameter of Gamma distribution
                              Method
     Renames SGAMMA from TOMS as slightly modified by BWB to use RANF
     instead of SUNIF.
     For details see:
               (Case R >= 1.0)
               Ahrens, J.H. and Dieter, U.
               Generating Gamma Variates by a
               Modified Rejection Technique.
               Comm. ACM, 25,1 (Jan. 1982), 47 - 54.
     Algorithm GD
               (Case 0.0 <= R <= 1.0)
               Ahrens, J.H. and Dieter, U.
               Computer Methods for Sampling from Gamma,
               Beta, Poisson and Binomial Distributions.
               Computing, 12 (1974), 223-246/
     Adapted algorithm GS.
**********************************************************************
*/
float RNG::GenerateGamma(float scale, float shape)
{
    float gengam;

    gengam  = RNG::StandardGamma(shape);
    gengam /= scale;

    return gengam;
}

/*
**********************************************************************
     void genmn(float *parm,float *x,float *work)
              GENerate Multivariate Normal random deviate
                              Arguments
     parm --> Parameters needed to generate multivariate normal
               deviates (MEANV and Cholesky decomposition of
               COVM). Set by a previous call to SETGMN.
               1 : 1                - size of deviate, P
               2 : P + 1            - mean vector
               P+2 : P*(P+3)/2 + 1  - upper half of cholesky
                                       decomposition of cov matrix
     x    <-- Vector deviate generated.
     work <--> Scratch array
                              Method
     1) Generate P independent standard normal deviates - Ei ~ N(0,1)
     2) Using Cholesky decomposition find A s.t. trans(A)*A = COVM
     3) trans(A)E + MEANV ~ N(MEANV,COVM)
**********************************************************************
*/
void RNG::GenerateMultivariateNormal(float *parm, float *x, float *work)
{
    int icount, p;
    float ae;

    p = (int)(*parm);
/*
     Generate P independent normal deviates - WORK ~ N(0,1)
*/
    for (int i = 1; i <= p; i++)
        work[i-1] = RNG::StandardNormal();

    for (int i = 1, D3 = 1, D4 = (p - i + D3) / D3; D4 > 0; D4--, i += D3)
    {
/*
     PARM (P+2 : P*(P+3)/2 + 1) contains A, the Cholesky
      decomposition of the desired covariance matrix.
          trans(A)(1,1) = PARM(P+2)
          trans(A)(2,1) = PARM(P+3)
          trans(A)(2,2) = PARM(P+2+P)
          trans(A)(3,1) = PARM(P+4)
          trans(A)(3,2) = PARM(P+3+P)
          trans(A)(3,3) = PARM(P+2-1+2P)  ...
     trans(A)*WORK + MEANV ~ N(MEANV,COVM)
*/
        icount = 0;
        ae     = 0.0;
        for (int j = 1, D1 = 1, D2 = (i - j + D1) / D1; D2 > 0; D2--, j += D1)
        {
            icount += j - 1;
            ae     += parm[i + (j - 1) * p - icount + p] * work[j-1];
        }

        x[i-1] = ae + parm[i];
    }
}

/*
**********************************************************************
 
     void genmul(int n,float *p,int ncat,int *ix)
     GENerate an observation from the MULtinomial distribution
                              Arguments
     N --> Number of events that will be classified into one of
           the categories 1..NCAT
     P --> Vector of probabilities.  P(i) is the probability that
           an event will be classified into category i.  Thus, P(i)
           must be [0,1]. Only the first NCAT-1 P(i) must be defined
           since P(NCAT) is 1.0 minus the sum of the first
           NCAT-1 P(i).
     NCAT --> Number of categories.  Length of P and IX.
     IX <-- Observation from multinomial distribution.  All IX(i)
            will be nonnegative and their sum will be N.
                              Method
     Algorithm from page 559 of
 
     Devroye, Luc
 
     Non-Uniform Random Variate Generation.  Springer-Verlag,
     New York, 1986.
 
**********************************************************************
*/
void RNG::GenerateMultinomial(int n, float *p, int ncat, int *ix)
{
    //float prob, ptot, sum;
    float prob, sum;
    int ntot;

    /*
    if (n < 0) ftnstop("N < 0 in GENMUL");
    if (ncat <= 1) ftnstop("NCAT <= 1 in GENMUL");
    
    ptot = 0.0f;
    
    for (int i = 0; i < ncat ; i++) {
        if (p[i] < 0.0f) ftnstop("Some P(i) < 0 in GENMUL");
        if (p[i] > 1.0f) ftnstop("Some P(i) > 1 in GENMUL");
        ptot += p[i];
    }

    if (ptot > 1.0f) ftnstop("Sum of P(i) > 1 in GENMUL");
    */

    //   Initialize variables
    ntot = n;
    sum  = 1.0F;
    for (int i = 0; i < ncat; i++)
        ix[i] = 0;

    /*
         Generate the observation
    */
    for (int icat = 0; icat < ncat - 1; icat++)
    {
        prob = p[icat] / sum;
        ix[icat] = GenerateBinomial(ntot, prob);
        ntot -= ix[icat];

        if (ntot <= 0) return;

        sum -= p[icat];
    }

    ix[ncat-1] = ntot;
}

void RNG::GenerateMultinomial2(int n, float p[], int ncat, int ix[])
{
    int   ntot = n;
    for (int i = 0; i < ncat; i++)
    {
        ix[i] = 0;
    }

    float prob = 0.0f;
    float sum  = 1.0f;
    for (int icat = 0; icat < (ncat - 1); icat++)
    {
        prob     = p[icat] / sum;
        ix[icat] = GenerateBinomial(ntot, prob);
        ntot    -= ix[icat];

        if (ntot <= 0)
        {
            return;
        }

        sum -= p[icat];
    }

    ix[ncat-1] = ntot;
}

/*
**********************************************************************
     float gennch(float df,float xnonc)
           Generate random value of Noncentral CHIsquare variable
                              Function
     Generates random deviate  from the  distribution  of a  noncentral
     chisquare with DF degrees  of freedom and noncentrality  parameter
     xnonc.
                              Arguments
     df --> Degrees of freedom of the chisquare
            (Must be > 1.0)
     xnonc --> Noncentrality parameter of the chisquare
               (Must be >= 0.0)
                              Method
     Uses fact that  noncentral chisquare  is  the  sum of a  chisquare
     deviate with DF-1  degrees of freedom plus the  square of a normal
     deviate with mean XNONC and standard deviation 1.
**********************************************************************
*/
float RNG::GenerateNoncentralChiSquare(float df, float xnonc)
{
    float gennch;

    if ((df <= 1.0) || (xnonc < 0.0))
    {
        fputs("DF <= 1 or XNONC < 0 in GENNCH - ABORT",stderr);
        fprintf(stderr,"Value of DF: %16.6E Value of XNONC%16.6E\n",df,xnonc);
        exit(1);
    }

    gennch = RNG::GenerateChiSquare(df - 1.0f) + pow(GenerateNormal((float)sqrt(xnonc), 1.0f), 2.0f);
    return gennch;
}

/*
**********************************************************************
     float gennf(float dfn,float dfd,float xnonc)
           GENerate random deviate from the Noncentral F distribution
                              Function
     Generates a random deviate from the  noncentral F (variance ratio)
     distribution with DFN degrees of freedom in the numerator, and DFD
     degrees of freedom in the denominator, and noncentrality parameter
     XNONC.
                              Arguments
     dfn --> Numerator degrees of freedom
             (Must be >= 1.0)
     dfd --> Denominator degrees of freedom
             (Must be positive)
     xnonc --> Noncentrality parameter
               (Must be nonnegative)
                              Method
     Directly generates ratio of noncentral numerator chisquare variate
     to central denominator chisquare variate.
**********************************************************************
*/
float RNG::GenerateNoncentralF(float dfn, float dfd, float xnonc)
{
    float gennf,xden,xnum;
    bool qcond;

    qcond = ((dfn <= 1.0) || (dfd <= 0.0) || (xnonc < 0.0));
    if (qcond)
    {
        fputs("In GENNF - Either (1) Numerator DF <= 1.0 or",stderr);
        fputs("(2) Denominator DF < 0.0 or ",stderr);
        fputs("(3) Noncentrality parameter < 0.0",stderr);
        fprintf(stderr,
            "DFN value: %16.6EDFD value: %16.6EXNONC value: \n%16.6E\n",dfn,dfd,
            xnonc);
        exit(-1);
    }

    xnum = GenerateNoncentralChiSquare(dfn, xnonc) / dfn;

    /*
          GENNF = ( GENNCH( DFN, XNONC ) / DFN ) / ( GENCHI( DFD ) / DFD )
    */
    xden = RNG::GenerateChiSquare(dfd)/dfd;
    if (xden <= (9.999999999998E-39 * xnum))
    {
        fputs(" GENNF - generated numbers would cause overflow",stderr);
        fprintf(stderr," Numerator %16.6E Denominator %16.6E\n",xnum,xden);
        fputs(" GENNF returning 1.0E38",stderr);
        gennf = _infnty;
    }
    else
        gennf = xnum/xden;

    return gennf;
}

/*
**********************************************************************
     float gennor(float av,float sd)
         GENerate random deviate from a NORmal distribution
                              Function
     Generates a single random deviate from a normal distribution
     with mean, AV, and standard deviation, SD.
                              Arguments
     av --> Mean of the normal distribution.
     sd --> Standard deviation of the normal distribution.
                              Method
     Renames SNORM from TOMS as slightly modified by BWB to use RANF
     instead of SUNIF.
     For details see:
               Ahrens, J.H. and Dieter, U.
               Extensions of Forsythe's Method for Random
               Sampling from the Normal Distribution.
               Math. Comput., 27,124 (Oct. 1973), 927 - 937.
**********************************************************************
*/
float RNG::GenerateNormal(float av, float sd)
{
    float gennor;

    gennor = sd * RNG::StandardNormal() + av;

    return gennor;
}

/*
**********************************************************************
    void genprm(long *iarray,int larray)
               GENerate random PeRMutation of iarray
                              Arguments
     iarray <--> On output IARRAY is a random permutation of its
                 value on input
     larray <--> Length of IARRAY
**********************************************************************
*/
void RNG::GeneratePermutation(int *vector, int length)
{
    for (int i = 1, D1 = 1, D2 = (length - i + D1) / D1; D2 > 0; D2--, i += D1)
    {
        int iwhich = RNG::GenerateUniformInteger(i, length);
        int itmp = vector[iwhich-1];
        vector[iwhich-1] = vector[i-1];
        vector[i-1]      = itmp;
    }
}

/*
**********************************************************************
     float genunf(float low,float high)
               GeNerate Uniform Real between LOW and HIGH
                              Function
     Generates a real uniformly distributed between LOW and HIGH.
                              Arguments
     low --> Low bound (exclusive) on real value to be generated
     high --> High bound (exclusive) on real value to be generated
**********************************************************************
*/
float RNG::GenerateUniform(float low,float high)
{
    float genunf;

    if (low > high)
    {
        fprintf(stderr,"LOW > HIGH in GENUNF: LOW %16.6E HIGH: %16.6E\n",low,high);
        fputs("Abort",stderr);
        exit(1);
    }

    genunf = low + (high - low) * RNG::GenerateUniformFloat01();
    return genunf;
}

/*
**********************************************************************
     void gscgn(long getset,long *g)
                         Get/Set GeNerator
     Gets or returns in G the number of the current generator
                              Arguments
     getset --> 0 Get
                1 Set
     g <-- Number of the current random number generator (1..32)
**********************************************************************
*/
void RNG::GetSetArguments(bool fSet, int *g)
{
    int curntg = 1;

    if (!fSet)
        *g = curntg;
    else
    {
        if (*g < 0 || *g > numg) {
            fputs(" Generator number out of range in GetSetArguments",stderr);
            exit(-1);
        }

        curntg = *g;
    }
}

/*
**********************************************************************
     void gsrgs(long getset,long *qvalue)
               Get/Set Random Generators Set
     Gets or sets whether random generators set (initialized).
     Initially (data statement) state is not set
     If getset is 1 state is set to qvalue
     If getset is 0 state returned in qvalue
**********************************************************************
*/
void Common::GetSetGeneratorInitialized(bool fSet, int *qvalue)
{
    static int qinit = 0;

    if (!fSet)
        *qvalue = qinit;
    else
        qinit = *qvalue;
}

/*
**********************************************************************
     void gssst(long getset,long *qset)
          Get or Set whether Seed is Set
     Initialize to Seed not Set
     If getset is 1 sets state to Seed Set
     If getset is 0 returns T in qset if Seed Set
     Else returns F in qset
**********************************************************************
*/
void RNG::GetSetSeedState(bool fSet, int *qset)
{
    static int qstate = 0;

    if (fSet)
        qstate = 1;
    else
        *qset = qstate;
}

/*
**********************************************************************
     long ignbin(long n,float pp)
                    GENerate BINomial random deviate
                              Function
     Generates a single random deviate from a binomial
     distribution whose number of trials is N and whose
     probability of an event in each trial is P.
                              Arguments
     n  --> The number of trials in the binomial distribution
            from which a random deviate is to be generated.
     p  --> The probability of an event in each trial of the
            binomial distribution from which a random deviate
            is to be generated.
     ignbin <-- A random deviate yielding the number of events
                from N independent trials, each of which has
                a probability of event P.
                              Method
     This is algorithm BTPE from:
         Kachitvichyanukul, V. and Schmeiser, B. W.
         Binomial Random Variate Generation.
         Communications of the ACM, 31, 2
         (February, 1988) 216.
**********************************************************************
     SUBROUTINE BTPEC(N,PP,ISEED,JX)
     BINOMIAL RANDOM VARIATE GENERATOR
     MEAN .LT. 30 -- INVERSE CDF
       MEAN .GE. 30 -- ALGORITHM BTPE:  ACCEPTANCE-REJECTION VIA
       FOUR REGION COMPOSITION.  THE FOUR REGIONS ARE A TRIANGLE
       (SYMMETRIC IN THE CENTER), A PAIR OF PARALLELOGRAMS (ABOVE
       THE TRIANGLE), AND EXPONENTIAL LEFT AND RIGHT TAILS.
     BTPE REFERS TO BINOMIAL-TRIANGLE-PARALLELOGRAM-EXPONENTIAL.
     BTPEC REFERS TO BTPE AND "COMBINED."  THUS BTPE IS THE
       RESEARCH AND BTPEC IS THE IMPLEMENTATION OF A COMPLETE
       USABLE ALGORITHM.
     REFERENCE:  VORATAS KACHITVICHYANUKUL AND BRUCE SCHMEISER,
       "BINOMIAL RANDOM VARIATE GENERATION,"
       COMMUNICATIONS OF THE ACM, FORTHCOMING
     WRITTEN:  SEPTEMBER 1980.
       LAST REVISED:  MAY 1985, JULY 1987
     REQUIRED SUBPROGRAM:  RAND() -- A UNIFORM (0,1) RANDOM NUMBER
                           GENERATOR
     ARGUMENTS
       N : NUMBER OF BERNOULLI TRIALS            (INPUT)
       PP : PROBABILITY OF SUCCESS IN EACH TRIAL (INPUT)
       ISEED:  RANDOM NUMBER SEED                (INPUT AND OUTPUT)
       JX:  RANDOMLY GENERATED OBSERVATION       (OUTPUT)
     VARIABLES
       PSAVE: VALUE OF PP FROM THE LAST CALL TO BTPEC
       NSAVE: VALUE OF N FROM THE LAST CALL TO BTPEC
       XNP:  VALUE OF THE MEAN FROM THE LAST CALL TO BTPEC
       P: PROBABILITY USED IN THE GENERATION PHASE OF BTPEC
       FFM: TEMPORARY VARIABLE EQUAL TO XNP + P
       M:  INTEGER VALUE OF THE CURRENT MODE
       FM:  FLOATING POINT VALUE OF THE CURRENT MODE
       XNPQ: TEMPORARY VARIABLE USED IN SETUP AND SQUEEZING STEPS
       P1:  AREA OF THE TRIANGLE
       C:  HEIGHT OF THE PARALLELOGRAMS
       XM:  CENTER OF THE TRIANGLE
       XL:  LEFT END OF THE TRIANGLE
       XR:  RIGHT END OF THE TRIANGLE
       AL:  TEMPORARY VARIABLE
       XLL:  RATE FOR THE LEFT EXPONENTIAL TAIL
       XLR:  RATE FOR THE RIGHT EXPONENTIAL TAIL
       P2:  AREA OF THE PARALLELOGRAMS
       P3:  AREA OF THE LEFT EXPONENTIAL TAIL
       P4:  AREA OF THE RIGHT EXPONENTIAL TAIL
       U:  A U(0,P4) RANDOM VARIATE USED FIRST TO SELECT ONE OF THE
           FOUR REGIONS AND THEN CONDITIONALLY TO GENERATE A VALUE
           FROM THE REGION
       V:  A U(0,1) RANDOM NUMBER USED TO GENERATE THE RANDOM VALUE
           (REGION 1) OR TRANSFORMED INTO THE VARIATE TO ACCEPT OR
           REJECT THE CANDIDATE VALUE
       IX:  INTEGER CANDIDATE VALUE
       X:  PRELIMINARY CONTINUOUS CANDIDATE VALUE IN REGION 2 LOGIC
           AND A FLOATING POINT IX IN THE ACCEPT/REJECT LOGIC
       K:  ABSOLUTE VALUE OF (IX-M)
       F:  THE HEIGHT OF THE SCALED DENSITY FUNCTION USED IN THE
           ACCEPT/REJECT DECISION WHEN BOTH M AND IX ARE SMALL
           ALSO USED IN THE INVERSE TRANSFORMATION
       R: THE RATIO P/Q
       G: CONSTANT USED IN CALCULATION OF PROBABILITY
       MP:  MODE PLUS ONE, THE LOWER INDEX FOR EXPLICIT CALCULATION
            OF F WHEN IX IS GREATER THAN M
       IX1:  CANDIDATE VALUE PLUS ONE, THE LOWER INDEX FOR EXPLICIT
             CALCULATION OF F WHEN IX IS LESS THAN M
       I:  INDEX FOR EXPLICIT CALCULATION OF F FOR BTPE
       AMAXP: MAXIMUM ERROR OF THE LOGARITHM OF NORMAL BOUND
       YNORM: LOGARITHM OF NORMAL BOUND
       ALV:  NATURAL LOGARITHM OF THE ACCEPT/REJECT VARIATE V
       X1,F1,Z,W,Z2,X2,F2, AND W2 ARE TEMPORARY VARIABLES TO BE
       USED IN THE FINAL ACCEPT/REJECT TEST
       QN: PROBABILITY OF NO SUCCESS IN N TRIALS
     REMARK
       IX AND JX COULD LOGICALLY BE THE SAME VARIABLE, WHICH WOULD
       SAVE A MEMORY POSITION AND A LINE OF CODE.  HOWEVER, SOME
       COMPILERS (E.G.,CDC MNF) OPTIMIZE BETTER WHEN THE ARGUMENTS
       ARE NOT INVOLVED.
     ISEED NEEDS TO BE DOUBLE PRECISION IF THE IMSL ROUTINE
     GGUBFS IS USED TO GENERATE UNIFORM RANDOM NUMBER, OTHERWISE
     TYPE OF ISEED SHOULD BE DICTATED BY THE UNIFORM GENERATOR
**********************************************************************
*****DETERMINE APPROPRIATE ALGORITHM AND WHETHER SETUP IS NECESSARY
*/
int RNG::GenerateBinomial(int n, float pp)
{
#ifdef BIN_CACHE
    static float psave = -1.0;
    static int nsave = -1;
    static float xnp;
#else
    float xnp = n * min(pp, 1.0f - pp);
#endif
    int ix, ix1, k, m, mp, T1;
    float al, alv, amaxp, c, f, f1, f2, ffm, fm, g, p, p1, p2, p3, p4, q, qn, r;
    float u, v, w, w2, x, x1, x2, xl, xll, xlr, xm, xnpq, xr, ynorm, z, z2;

#ifdef BIN_CACHE
    if (pp == psave)
    {
        if (n != nsave) goto S20;
        if (xnp < 30.0) goto S150;
        goto S30;
    }
#endif

    /*
    *****SETUP, PERFORM ONLY WHEN PARAMETERS CHANGE
    */
#ifdef BIN_CACHE
    psave = pp;
    p     = min(psave,1.0f-psave);
#else
    p     = min(pp, 1.0f - pp);
#endif
    q     = 1.0f-p;
#ifdef BIN_CACHE
S20:
    xnp   = n*p;
    nsave = n;
#endif

    if (xnp < 30.0) goto S140;

    ffm  = xnp + p;
    m    = (int)ffm;
    fm   = (float)m;
    xnpq = xnp * q;
    p1   = (float)((2.195 * sqrt(xnpq) - 4.6 * q) + 0.5);
    xm   = fm + 0.5f;
    xl   = xm - p1;
    xr   = xm + p1;
    c    = 0.134f + 20.5f / (15.3f + fm);
    al   = (ffm - xl) / (ffm - xl * p);
    xll  = al * (1.0f + 0.5f * al);
    al   = (xr - ffm) / (xr * q);
    xlr  = al * (1.0f + 0.5f * al);
    p2   = p1 * (1.0f + c + c);
    p3   = p2 + c / xll;
    p4   = p3 + c / xlr;

S30:
    /*
    *****GENERATE VARIATE
    */
    u = RNG::GenerateUniformFloat01()*p4;
    v = RNG::GenerateUniformFloat01();

    /*
         TRIANGULAR REGION
    */
    if (u <= p1)
    {
        ix = (int)(xm - p1 * v + u);
        goto S170;
    }

    /*
         PARALLELOGRAM REGION
    */
    if (u <= p2)
    {
        x = xl + (u - p1) / c;
        v = v * c + 1.0f - ABS(xm - x) / p1;
        if (v > 1.0 || v <= 0.0) goto S30;
        ix = (int)x;
    }
    else
    {
        /*
        LEFT TAIL
        */
        if (u <= p3)
        {
            ix = (int)(xl + log(v) / xll);
            if (ix < 0) goto S30;
            v *= ((u - p2) * xll);
        }
        else
            /*
            RIGHT TAIL
            */
        {
            ix = (int)(xr - log(v) / xlr);
            if (ix > n) goto S30;
            v *= ((u - p3) * xlr);
        }
    }

    /*
    *****DETERMINE APPROPRIATE WAY TO PERFORM ACCEPT/REJECT TEST
    */
    k = ABS(ix - m);
    if (k > 20 && k < (xnpq / 2 - 1)) goto S130;

    /*
         EXPLICIT EVALUATION
    */
    f = 1.0;
    r = p / q;
    g = (n + 1) * r;
    T1 = m - ix;
    if (T1 < 0) goto S80;
    else if (T1 == 0) goto S120;
    else goto S100;

S80:
    mp = m + 1;
    for (int i = mp; i <= ix; i++) f *= (g / i - r);
    goto S120;

S100:
    ix1 = ix + 1;
    for (int i = ix1; i <= m; i++) f /= (g / i - r);

S120:
    if (v <= f) goto S170;
    goto S30;

S130:
    /*
         SQUEEZING USING UPPER AND LOWER BOUNDS ON ALOG(F(X))
    */
    amaxp = k / xnpq * ((k * (k / 3.0f + 0.625f) + 0.1666666666666f) / xnpq + 0.5f);
    ynorm = -(square(k) / (2.0f * xnpq));
    alv   = log(v);

    if (alv < ynorm - amaxp) goto S170;
    if (alv > ynorm + amaxp) goto S30;

    /*
         STIRLING'S FORMULA TO MACHINE ACCURACY FOR
         THE FINAL ACCEPTANCE/REJECTION TEST
    */
    x1 = ix + 1.0f;
    f1 = fm + 1.0f;
    z  = n + 1.0f - fm;
    w  = n - ix + 1.0f;
    z2 = square(z);
    x2 = square(x1);
    f2 = square(f1);
    w2 = square(w);

    if (alv <= xm * log(f1 / x1) + (n - m + 0.5) * log(z / w) + (ix - m) * log(w * p / (x1 * q)) + (13860.0 -
      (462.0 - (132.0 - (99.0 - 140.0 / f2) / f2) / f2) / f2) / f1 / 166320.0 + (13860.0 - (462.0 -
      (132.0 - (99.0 - 140.0 / z2) / z2) / z2) / z2) / z / 166320.0 + (13860.0 - (462.0 - (132.0 -
      (99.0 - 140.0 / x2) / x2) / x2) / x2) / x1 / 166320.0 + (13860.0 - (462.0 - (132.0 - (99.0 -
      140.0 / w2) / w2) / w2) / w2) / w / 166320.0) goto S170;
    goto S30;

S140:
    /*
         INVERSE CDF LOGIC FOR MEAN LESS THAN 30
    */
    qn = pow(q, (float)n);
    r  = p / q;
    g  = r * (n + 1);

S150:
    ix = 0;
    f  = qn;
    u  = RNG::GenerateUniformFloat01();

S160:
    if (u >= f)
    {
        if (ix > 110) goto S150;

        u  -= f;
        ix += 1;
        f  *= (g / ix - r);
        goto S160;
    }

S170:
#ifdef BIN_CACHE
    if (psave > 0.5f)
#else
    if (pp > 0.5f)
#endif
        ix = n - ix;

    return ix;
}

/*
**********************************************************************
 
     long ignnbn(long n,float p)
                GENerate Negative BiNomial random deviate
                              Function
     Generates a single random deviate from a negative binomial
     distribution.
                              Arguments
     N  --> The number of trials in the negative binomial distribution
            from which a random deviate is to be generated.
     P  --> The probability of an event.
                              Method
     Algorithm from page 480 of
 
     Devroye, Luc
 
     Non-Uniform Random Variate Generation.  Springer-Verlag,
     New York, 1986.
**********************************************************************
*/

int RNG::GenerateNegativeBinomial(int n, float p)
{
    int ignnbn;
    float y, a, r;

    /*
         Check Arguments
    */
    if (n < 0) ftnstop("N < 0 in IGNNBN");
    if (p <= 0.0F) ftnstop("P <= 0 in IGNNBN");
    if (p >= 1.0F) ftnstop("P >= 1 in IGNNBN");

    /*
         Generate Y, a random gamma (n,(1-p)/p) variable
    */
    r = (float)n;
    a = p / (1.0f - p);
    y = GenerateGamma(a,r);

    /*
         Generate a random Poisson(y) variable
    */
    ignnbn = RNG::GeneratePoisson(y);
    return ignnbn;
}

/*
**********************************************************************
     long ignpoi(float mu)
                    GENerate POIsson random deviate
                              Function
     Generates a single random deviate from a Poisson
     distribution with mean AV.
                              Arguments
     av --> The mean of the Poisson distribution from which
            a random deviate is to be generated.
     genexp <-- The random deviate.
                              Method
     Renames KPOIS from TOMS as slightly modified by BWB to use RANF
     instead of SUNIF.
     For details see:
               Ahrens, J.H. and Dieter, U.
               Computer Generation of Poisson Deviates
               From Modified Normal Distributions.
               ACM Trans. Math. Software, 8, 2
               (June 1982),163-179
**********************************************************************
**********************************************************************
                                                                      
                                                                      
     P O I S S O N  DISTRIBUTION                                      
                                                                      
                                                                      
**********************************************************************
**********************************************************************
                                                                      
     FOR DETAILS SEE:                                                 
                                                                      
               AHRENS, J.H. AND DIETER, U.                            
               COMPUTER GENERATION OF POISSON DEVIATES                
               FROM MODIFIED NORMAL DISTRIBUTIONS.                    
               ACM TRANS. MATH. SOFTWARE, 8,2 (JUNE 1982), 163 - 179. 
                                                                      
     (SLIGHTLY MODIFIED VERSION OF THE PROGRAM IN THE ABOVE ARTICLE)  
                                                                      
**********************************************************************
      INTEGER FUNCTION IGNPOI(IR,MU)
     INPUT:  IR=CURRENT STATE OF BASIC RANDOM NUMBER GENERATOR
             MU=MEAN MU OF THE POISSON DISTRIBUTION
     OUTPUT: IGNPOI=SAMPLE FROM THE POISSON-(MU)-DISTRIBUTION
     MUPREV=PREVIOUS MU, MUOLD=MU AT LAST EXECUTION OF STEP P OR B.
     TABLES: COEFFICIENTS A0-A7 FOR STEP F. FACTORIALS FACT
     COEFFICIENTS A(K) - FOR PX = FK*V*V*SUM(A(K)*V**K)-DEL
     SEPARATION OF CASES A AND B
*/
int RNG::GeneratePoisson(float mu)
{
#define A0  -0.5f
#define A1   0.3333333f
#define A2  -0.2500068f
#define A3   0.2000118f
#define A4  -0.1661269f
#define A5   0.1421878f
#define A6  -0.1384794f
#define A7   0.125006f

#ifdef POISSON_CACHE
    static float muold    =  0.0f;
    static float muprev   =  0.0f;
#endif
    static float fact[10] = { 1.0f, 1.0f, 2.0f, 6.0f, 24.0f, 120.0f, 720.0f, 5040.0f, 40320.0f, 362880.0f };

    int ignpoi, j, k, kflag, l, m;
    float b1, b2, c, c0, c1, c2, c3, d, del, difmuk, e, fk, fx, fy, g;
    float omega, p, p0, px, py, q, s,  t, u, v, x, xx, pp[35];

#ifdef POISSON_CACHE
    if (mu != muprev)
#endif
    {
        if (mu < 10.0) goto S120;

        /*
        C A S E  A. (RECALCULATION OF S,D,L IF MU HAS CHANGED)
        */
#ifdef POISSON_CACHE
        muprev = mu;
#endif
        s      = sqrt(mu);
        d      = 6.0f * square(mu * mu);

        /*
        THE POISSON PROBABILITIES PK EXCEED THE DISCRETE NORMAL
        PROBABILITIES FK WHENEVER K >= M(MU). L=IFIX(MU-1.1484)
        IS AN UPPER BOUND TO M(MU) FOR ALL MU >= 10 .
        */
        l = (int)(mu - 1.1484);
    }

    /*
         STEP N. NORMAL SAMPLE - SNORM(IR) FOR STANDARD NORMAL DEVIATE
    */
    g = mu + s * RNG::StandardNormal();
    if (g >= 0.0)
    {
        ignpoi = (int)(g);

        /*
        STEP I. IMMEDIATE ACCEPTANCE IF IGNPOI IS LARGE ENOUGH
        */
        if (ignpoi >= l) return ignpoi;

        /*
        STEP S. SQUEEZE ACCEPTANCE - SUNIF(IR) FOR (0,1)-SAMPLE U
        */
        fk = (float)ignpoi;
        difmuk = mu - fk;
        u = RNG::GenerateUniformFloat01();
        if ((d * u) >= cube(difmuk)) return ignpoi;
    }

    /*
         STEP P. PREPARATIONS FOR STEPS Q AND H.
                 (RECALCULATIONS OF PARAMETERS IF NECESSARY)
                 .3989423=(2*PI)**(-.5)  .416667E-1=1./24.  .1428571=1./7.
                 THE QUANTITIES B1, B2, C3, C2, C1, C0 ARE FOR THE HERMITE
                 APPROXIMATIONS TO THE DISCRETE NORMAL PROBABILITIES FK.
                 C=.1069/MU GUARANTEES MAJORIZATION BY THE 'HAT'-FUNCTION.
    */
#ifdef POISSON_CACHE
    if (mu != muold)
    {
        muold = mu;
#else
    {
#endif
        omega = 0.3989423f / s;
        b1    = 4.166667E-2f / mu;
        b2    = 0.3f * square(b1);
        c3    = 0.1428571f * b1 * b2;
        c2    = b2 - 15.0f * c3;
        c1    = b1 - 6.0f * b2 + 45.0f * c3;
        c0    = 1.0f - b1 + 3.0f * b2 - 15.0f * c3;
        c     = 0.1069f / mu;
    }

    if (g >= 0.0)
    {
        /*
        'SUBROUTINE' F IS CALLED (KFLAG=0 FOR CORRECT RETURN)
        */
        kflag = 0;
        goto S70;

S40:
        /*
        STEP Q. QUOTIENT ACCEPTANCE (RARE CASE)
        */
        if ((fy - u * fy) <= (py * exp(px - fx))) return ignpoi;
    }

S50:
    do
    {
        /*
        STEP E. EXPONENTIAL SAMPLE - SEXPO(IR) FOR STANDARD EXPONENTIAL
        DEVIATE E AND SAMPLE T FROM THE LAPLACE 'HAT'
        (IF T <= -.6744 THEN PK < FK FOR ALL MU >= 10.)
        */
        e  = StandardExponential();
        u  = RNG::GenerateUniformFloat01();
        u += (u - 1.0f);
        t  = 1.8f+ fsign(e, u);
    }
    while (t <= -0.6744);

    ignpoi = (int)(mu + s * t);
    fk     = (float)ignpoi;
    difmuk = mu - fk;

    /*
                 'SUBROUTINE' F IS CALLED (KFLAG=1 FOR CORRECT RETURN)
    */
    kflag = 1;

S70:
    /*
         STEP F. 'SUBROUTINE' F. CALCULATION OF PX,PY,FX,FY.
                 CASE IGNPOI .LT. 10 USES FACTORIALS FROM TABLE FACT
    */
    if (ignpoi < 10)
    {
        px = -mu;
        py = pow(mu, (float)ignpoi) / fact[ignpoi];
        goto S110;
    }

/*
             CASE IGNPOI .GE. 10 USES POLYNOMIAL APPROXIMATION
             A0-A7 FOR ACCURACY WHEN ADVISABLE
             .8333333E-1=1./12.  .3989423=(2*PI)**(-.5)
*/
    del  = 8.333333E-2f / fk;
    del -= (4.8f * cube(del));
    v    = difmuk / fk;

    if (fabs(v) > 0.25)
    {
        px = (float)(fk * log(1.0 + v) - difmuk - del);
    }
    else
        px = fk * square(v) * (((((((A7 * v + A6) * v + A5) * v + A4) * v + A3) * v + A2) * v + A1) * v + A0) - del;

    py = 0.3989423f / sqrt(fk);

S110:
    x = (0.5f - difmuk) / s;
    xx = square(x);
    fx = -0.5f * xx;
    fy = omega * (((c3 * xx + c2) * xx + c1) * xx + c0);

    if (kflag <= 0) goto S40;

    /*
         STEP H. HAT ACCEPTANCE (E IS REPEATED ON REJECTION)
    */
    if ((c * fabs(u)) > (py * exp(px + e) - fy * exp(fx + e))) goto S50;
    return ignpoi;

S120:
    /*
         C A S E  B. (START NEW TABLE AND CALCULATE P0 IF NECESSARY)
    */
#ifdef POISSON_CACHE
    muprev = 0.0;
    if (mu != muold)
    {
        muold = mu;
#else
    {
#endif
        m = max(1, (int)mu);
        l = 0;
        p = exp(-mu);
        q = p0 = p;
    }

S130:
    /*
         STEP U. UNIFORM SAMPLE FOR INVERSION METHOD
    */
    u = RNG::GenerateUniformFloat01();
    ignpoi = 0;
    if (u <= p0) return ignpoi;

    /*
         STEP T. TABLE COMPARISON UNTIL THE END PP(L) OF THE
                 PP-TABLE OF CUMULATIVE POISSON PROBABILITIES
                 (0.458=PP(9) FOR MU=10)
    */
    if (l != 0)
    {
        j = 1;
        if (u > 0.458) j = min(l, m);
        for (k = j; k <= l; k++) {
            if (u <= pp[k-1]) goto S180;
        }
        if (l == 35) goto S130;
    }

    /*
         STEP C. CREATION OF NEW POISSON PROBABILITIES P
                 AND THEIR CUMULATIVES Q=PP(K)
    */
    l += 1;
    for (k = l; k <= 35; k++) {
        p = p * mu / (float)k;
        q += p;
        pp[k-1] = q;
        if (u <= q) goto S170;
    }

    l = 35;
    goto S130;

S170:
    l = k;

S180:
    ignpoi = k;
    return ignpoi;

#undef A0
#undef A1
#undef A2
#undef A3
#undef A4
#undef A5
#undef A6
#undef A7
}

/*
**********************************************************************
     long ignuin(long low,long high)
               GeNerate Uniform INteger
                              Function
     Generates an integer uniformly distributed between LOW and HIGH.
                              Arguments
     low --> Low bound (inclusive) on integer value to be generated
     high --> High bound (inclusive) on integer value to be generated
                              Note
     If (HIGH-LOW) > 2,147,483,561 prints error message on * unit and
     stops the program.
**********************************************************************
     IGNLGI generates integers between 1 and 2147483562
     MAXNUM is 1 less than maximum generable value
*/
int RNG::GenerateUniformInteger(int low, int high)
{
#define maxnum 2147483561L
    int ignuin, ign, maxnow, range, ranp1;

    if (low > high)
    {
        fputs(" low > high in ignuin - ABORT",stderr);
        exit(1);
    }

    range = high-low;
    if (range > maxnum)
    {
        fputs(" high - low too large in ignuin - ABORT",stderr);
        exit(1);
    }

    if (low == high)
    {
        ignuin = low;
        return ignuin;
    }

    /*
         Number to be generated should be in range 0..RANGE
         Set MAXNOW so that the number of integers in 0..MAXNOW is an
         integral multiple of the number in 0..RANGE
    */
    ranp1 = range + 1;
    maxnow = maxnum / ranp1 * ranp1;

    do
    {
        ign = Common::GenerateLargeInteger() - 1;
    }
    while (ign > maxnow);

    ignuin = low + ign % ranp1;
    return ignuin;

#undef maxnum
}

int lennob(char *str)
    /* 
    Returns the length of str ignoring trailing blanks but not 
    other white space.
    */
{
    int i_nb = -1;

    for (int i = 0; str[i]; i++)
        if (str[i] != ' ')
            i_nb = i;

    return (i_nb + 1);
}

/*
**********************************************************************
     void phrtsd(char* phrase,long *seed1,long *seed2)
               PHRase To SeeDs

                              Function

     Uses a phrase (character string) to generate two seeds for the RGN
     random number generator.
                              Arguments
     phrase --> Phrase to be used for random number generation
      
     seed1 <-- First seed for generator
                        
     seed2 <-- Second seed for generator
                        
                              Note

     Trailing blanks are eliminated before the seeds are generated.
     Generated seed values will fall in the range 1..2^30
     (1..1,073,741,824)
**********************************************************************
*/
void RNG::PhraseToSeeds(char* phrase, int *seed1, int *seed2)
{
    static char table[] = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()_+[];:'\\\"<>?,./";
    static int twop30 = 1073741824;
    static int shift[5] = { 1, 64, 4096, 262144, 16777216 };

    int ix;
    int ichr, lphr, values[5];

    *seed1 = 1234567890L;
    *seed2 = 123456789L;
    lphr   = lennob(phrase); 

    if (lphr < 1) return;

    for (int i = 0; i <= (lphr - 1); i++)
    {
        for (ix = 0; table[ix]; ix++)
            if (phrase[i] == table[ix]) break;

        if (!table[ix]) ix = 0;

        ichr = ix % 64;
        if (ichr == 0) ichr = 63;

        for (int j = 1; j <= 5; j++) {
            values[j - 1] = ichr - j;
            if (values[j-1] < 1) values[j-1] += 63;
        }

        for (int j = 1; j <= 5; j++) {
            *seed1 = (*seed1 + shift[j - 1] * values[j - 1])    % twop30;
            *seed2 = (*seed2 + shift[j - 1] * values[6 - j -1]) % twop30;
        }
    }
}

/*
**********************************************************************
     float ranf(void)
                RANDom number generator as a Function
     Returns a random floating point number from a uniform distribution
     over 0 - 1 (endpoints of this interval are not returned) using the
     current generator
     This is a transcription from Pascal to Fortran of routine
     Uniform_01 from the paper
     L'Ecuyer, P. and Cote, S. "Implementing a Random Number Package
     with Splitting Facilities." ACM Transactions on Mathematical
     Software, 17:98-111 (1991)
**********************************************************************
*/
float RNG::GenerateUniformFloat01(void)
{
    /*
         4.656613057E-10 is 1/M1  M1 is set in a data statement in IGNLGI
          and is currently 2147483563. If M1 changes, change this also.
    */
    float ranf = Common::GenerateLargeInteger()*4.656613057E-10f;
    return ranf;
}

/*
**********************************************************************
     void setgmn(float *meanv,float *covm,long p,float *parm)
            SET Generate Multivariate Normal random deviate
                              Function
      Places P, MEANV, and the Cholesky factorization of COVM
      in GENMN.
                              Arguments
     meanv --> Mean vector of multivariate normal distribution.
     covm   <--> (Input) Covariance   matrix    of  the  multivariate
                 normal distribution
                 (Output) Destroyed on output
     p     --> Dimension of the normal, or length of MEANV.
     parm <-- Array of parameters needed to generate multivariate norma
                deviates (P, MEANV and Cholesky decomposition of
                COVM).
                1 : 1                - P
                2 : P + 1            - MEANV
                P+2 : P*(P+3)/2 + 1  - Cholesky decomposition of COVM
               Needed dimension is (p*(p+3)/2 + 1)
**********************************************************************
*/
void RNG::SetGenerateMultivariateNormal(float *meanv, float *covm, int p, float *parm)
{
    int T1;
    int icount, info;

    T1 = p * (p + 3) / 2 + 1;
/*
     TEST THE INPUT
*/
    if (p <= 0)
    {
        fputs("P nonpositive in SETGMN",stderr);
        fprintf(stderr,"Value of P: %12ld\n",p);
        exit(1);
    }

    *parm = (float)p;
/*
     PUT P AND MEANV INTO PARM
*/
    for (int i = 2, D2 = 1, D3 = (p + 1 - i + D2) / D2; D3 > 0; D3--, i += D2)
        parm[i-1] = meanv[i-2];
/*
      Cholesky decomposition to find A s.t. trans(A)*(A) = COVM
*/
    Linpack::FactorSymmetricPositiveMatrix(covm, p, p, &info);
    if (info != 0)
    {
        fputs(" COVM not positive definite in SETGMN",stderr);
        exit(1);
    }

    icount = p + 1;
/*
     PUT UPPER HALF OF A, WHICH IS NOW THE CHOLESKY FACTOR, INTO PARM
          COVM(1,1) = PARM(P+2)
          COVM(1,2) = PARM(P+3)
                    :
          COVM(1,P) = PARM(2P+1)
          COVM(2,2) = PARM(2P+2)  ...
*/
    for (int i = 1, D4 = 1, D5 = (p - i + D4) / D4; D5 > 0; D5--, i += D4) {
        for (int j = i - 1; j < p; j++) {
            icount += 1;
            parm[icount-1] = covm[i-1+j*p];
        }
    }
}

/*
**********************************************************************
                                                                      
                                                                      
     (STANDARD-)  E X P O N E N T I A L   DISTRIBUTION                
                                                                      
                                                                      
**********************************************************************
**********************************************************************
                                                                      
     FOR DETAILS SEE:                                                 
                                                                      
               AHRENS, J.H. AND DIETER, U.                            
               COMPUTER METHODS FOR SAMPLING FROM THE                 
               EXPONENTIAL AND NORMAL DISTRIBUTIONS.                  
               COMM. ACM, 15,10 (OCT. 1972), 873 - 882.               
                                                                      
     ALL STATEMENT NUMBERS CORRESPOND TO THE STEPS OF ALGORITHM       
     'SA' IN THE ABOVE PAPER (SLIGHTLY MODIFIED IMPLEMENTATION)       
                                                                      
     Modified by Barry W. Brown, Feb 3, 1988 to use RANF instead of   
     SUNIF.  The argument IR thus goes away.                          
                                                                      
**********************************************************************
     Q(N) = SUM(ALOG(2.0)**K/K!)    K=1,..,N ,      THE HIGHEST N
     (HERE 8) IS DETERMINED BY Q(N)=1.0 WITHIN STANDARD PRECISION
*/
float RNG::StandardExponential(void)
{
    static float q[8] = { 0.6931472f,0.9333737f,0.9888778f,0.9984959f,0.9998293f,0.9999833f,0.9999986f,1.0f };
    int i;
    float sexpo, a, u, ustar, umin;
    float *q1 = q;

    a = 0.0f;
    u = RNG::GenerateUniformFloat01();
    goto S30;

S20:
    a += *q1;

S30:
    u += u;
    if (u <= 1.0) goto S20;

    u -= 1.0;
    if (u <= *q1)
    {
        sexpo = a + u;
        return sexpo;
    }

    i = 1;
    ustar = RNG::GenerateUniformFloat01();
    umin = ustar;

    do
    {
        ustar = RNG::GenerateUniformFloat01();
        if (ustar < umin) umin = ustar;
        i += 1;
    }
    while (u > q[i-1]);

    sexpo = a + umin * *q1;
    return sexpo;
}

/*
**********************************************************************
                                                                      
                                                                      
     (STANDARD-)  G A M M A  DISTRIBUTION                             
                                                                      
                                                                      
**********************************************************************
**********************************************************************
                                                                      
               PARAMETER  A >= 1.0  !                                 
                                                                      
**********************************************************************
                                                                      
     FOR DETAILS SEE:                                                 
                                                                      
               AHRENS, J.H. AND DIETER, U.                            
               GENERATING GAMMA VARIATES BY A                         
               MODIFIED REJECTION TECHNIQUE.                          
               COMM. ACM, 25,1 (JAN. 1982), 47 - 54.                  
                                                                      
     STEP NUMBERS CORRESPOND TO ALGORITHM 'GD' IN THE ABOVE PAPER     
                                 (STRAIGHTFORWARD IMPLEMENTATION)     
                                                                      
     Modified by Barry W. Brown, Feb 3, 1988 to use RANF instead of   
     SUNIF.  The argument IR thus goes away.                          
                                                                      
**********************************************************************
                                                                      
               PARAMETER  0.0 < A < 1.0  !                            
                                                                      
**********************************************************************
                                                                      
     FOR DETAILS SEE:                                                 
                                                                      
               AHRENS, J.H. AND DIETER, U.                            
               COMPUTER METHODS FOR SAMPLING FROM GAMMA,              
               BETA, POISSON AND BINOMIAL DISTRIBUTIONS.              
               COMPUTING, 12 (1974), 223 - 246.                       
                                                                      
     (ADAPTED IMPLEMENTATION OF ALGORITHM 'GS' IN THE ABOVE PAPER)    
                                                                      
**********************************************************************
     INPUT: A =PARAMETER (MEAN) OF THE STANDARD GAMMA DISTRIBUTION
     OUTPUT: SGAMMA = SAMPLE FROM THE GAMMA-(A)-DISTRIBUTION
     COEFFICIENTS Q(K) - FOR Q0 = SUM(Q(K)*A**(-K))
     COEFFICIENTS A(K) - FOR Q = Q0+(T*T/2)*SUM(A(K)*V**K)
     COEFFICIENTS E(K) - FOR EXP(Q)-1 = SUM(E(K)*Q**K)
     PREVIOUS A PRE-SET TO ZERO - AA IS A', AAA IS A"
     SQRT32 IS THE SQUAREROOT OF 32 = 5.656854249492380
*/
float RNG::StandardGamma(float a)
{
static float q1 = 4.166669E-2f;
static float q2 = 2.083148E-2f;
static float q3 = 8.01191E-3f;
static float q4 = 1.44121E-3f;
static float q5 = -7.388E-5f;
static float q6 = 2.4511E-4f;
static float q7 = 2.424E-4f;
static float a1 = 0.3333333f;
static float a2 = -0.250003f;
static float a3 = 0.2000062f;
static float a4 = -0.1662921f;
static float a5 = 0.1423657f;
static float a6 = -0.1367177f;
static float a7 = 0.1233795f;
static float e1 = 1.0f;
static float e2 = 0.4999897f;
static float e3 = 0.166829f;
static float e4 = 4.07753E-2f;
static float e5 = 1.0293E-2f;
static float aa = 0.0f;
static float aaa = 0.0f;
static float sqrt32 = 5.656854f;
static float sgamma,s2,s,d,t,x,u,r,q0,b,si,c,v,q,e,w,p;
    if(a == aa) goto S10;
    if(a < 1.0) goto S120;
/*
     STEP  1:  RECALCULATIONS OF S2,S,D IF A HAS CHANGED
*/
    aa = a;
    s2 = (float)(a-0.5);
    s = sqrt(s2);
    d = (float)(sqrt32-12.0*s);
S10:
/*
     STEP  2:  T=STANDARD NORMAL DEVIATE,
               X=(S,1/2)-NORMAL DEVIATE.
               IMMEDIATE ACCEPTANCE (I)
*/
    
    t = RNG::StandardNormal();
    x = (float)(s+0.5*t);
    sgamma = x*x;
    if(t >= 0.0) return sgamma;
/*
     STEP  3:  U= 0,1 -UNIFORM SAMPLE. SQUEEZE ACCEPTANCE (S)
*/
    u = RNG::GenerateUniformFloat01();
    if(d*u <= t*t*t) return sgamma;
/*
     STEP  4:  RECALCULATIONS OF Q0,B,SI,C IF NECESSARY
*/
    if(a == aaa) goto S40;
    aaa = a;
    r = (float)(1.0/ a);
    q0 = ((((((q7*r+q6)*r+q5)*r+q4)*r+q3)*r+q2)*r+q1)*r;
/*
               APPROXIMATION DEPENDING ON SIZE OF PARAMETER A
               THE CONSTANTS IN THE EXPRESSIONS FOR B, SI AND
               C WERE ESTABLISHED BY NUMERICAL EXPERIMENTS
*/
    if(a <= 3.686) goto S30;
    if(a <= 13.022) goto S20;
/*
               CASE 3:  A .GT. 13.022
*/
    b = (float)(1.77);
    si = 0.75;
    c = (float)(0.1515/s);
    goto S40;
S20:
/*
               CASE 2:  3.686 .LT. A .LE. 13.022
*/
    b = (float)(1.654+7.6E-3*s2);
    si = (float)(1.68/s+0.275);
    c = (float)(6.2E-2/s+2.4E-2);
    goto S40;
S30:
/*
               CASE 1:  A .LE. 3.686
*/
    b = (float)(0.463+s+0.178*s2);
    si = (float)(1.235);
    c = (float)(0.195/s-7.9E-2+1.6E-1*s);
S40:
/*
     STEP  5:  NO QUOTIENT TEST IF X NOT POSITIVE
*/
    if(x <= 0.0) goto S70;
/*
     STEP  6:  CALCULATION OF V AND QUOTIENT Q
*/
    v = t/(s+s);
    if(fabs(v) <= 0.25) goto S50;
    q = (float)(q0-s*t+0.25*t*t+(s2+s2)*log(1.0+v));
    goto S60;
S50:
    q = (float)(q0+0.5*t*t*((((((a7*v+a6)*v+a5)*v+a4)*v+a3)*v+a2)*v+a1)*v);
S60:
/*
     STEP  7:  QUOTIENT ACCEPTANCE (Q)
*/
    if(log(1.0-u) <= q) return sgamma;
S70:
/*
     STEP  8:  E=STANDARD EXPONENTIAL DEVIATE
               U= 0,1 -UNIFORM DEVIATE
               T=(B,SI)-DOUBLE EXPONENTIAL (LAPLACE) SAMPLE
*/
    e = StandardExponential();
    u = RNG::GenerateUniformFloat01();
    u = (float)(u + (u-1.0));
    t = b+fsign(si*e,u);
/*
     STEP  9:  REJECTION IF T .LT. TAU(1) = -.71874483771719
*/
    if(t < -0.7187449) goto S70;
/*
     STEP 10:  CALCULATION OF V AND QUOTIENT Q
*/
    v = t/(s+s);
    if(fabs(v) <= 0.25) goto S80;
    q = (float)(q0-s*t+0.25*t*t+(s2+s2)*log(1.0+v));
    goto S90;
S80:
    q = (float)(q0+0.5*t*t*((((((a7*v+a6)*v+a5)*v+a4)*v+a3)*v+a2)*v+a1)*v);
S90:
/*
     STEP 11:  HAT ACCEPTANCE (H) (IF Q NOT POSITIVE GO TO STEP 8)
*/
    if(q <= 0.0) goto S70;
    if(q <= 0.5) goto S100;
    w = (float)(exp(q)-1.0);
    goto S110;
S100:
    w = ((((e5*q+e4)*q+e3)*q+e2)*q+e1)*q;
S110:
/*
               IF T IS REJECTED, SAMPLE AGAIN AT STEP 8
*/
    if(c*fabs(u) > w*exp(e-0.5*t*t)) goto S70;
    x = (float)(s+0.5*t);
    sgamma = x*x;
    return sgamma;
S120:
/*
     ALTERNATE METHOD FOR PARAMETERS A BELOW 1  (.3678794=EXP(-1.))
*/
    aa = 0.0;
    b = (float)(1.0+0.3678794*a);
S130:
    p = b*RNG::GenerateUniformFloat01();
    if(p >= 1.0) goto S140;
    sgamma = exp(log(p)/ a);
    if(StandardExponential() < sgamma) goto S130;
    return sgamma;
S140:
    sgamma = -log((b-p)/ a);
    if(StandardExponential() < (1.0-a)*log(sgamma)) goto S130;
    return sgamma;
}

/*
**********************************************************************
                                                                      
                                                                      
     (STANDARD-)  N O R M A L  DISTRIBUTION                           
                                                                      
                                                                      
**********************************************************************
**********************************************************************
                                                                      
     FOR DETAILS SEE:                                                 
                                                                      
               AHRENS, J.H. AND DIETER, U.                            
               EXTENSIONS OF FORSYTHE'S METHOD FOR RANDOM             
               SAMPLING FROM THE NORMAL DISTRIBUTION.                 
               MATH. COMPUT., 27,124 (OCT. 1973), 927 - 937.          
                                                                      
     ALL STATEMENT NUMBERS CORRESPOND TO THE STEPS OF ALGORITHM 'FL'  
     (M=5) IN THE ABOVE PAPER     (SLIGHTLY MODIFIED IMPLEMENTATION)  
                                                                      
     Modified by Barry W. Brown, Feb 3, 1988 to use RANF instead of   
     SUNIF.  The argument IR thus goes away.                          
                                                                      
**********************************************************************
     THE DEFINITIONS OF THE CONSTANTS A(K), D(K), T(K) AND
     H(K) ARE ACCORDING TO THE ABOVEMENTIONED ARTICLE
*/
float RNG::StandardNormal(void)
{
    static float a[32] = {
        0.0f,3.917609E-2f,7.841241E-2f,0.11777f,0.1573107f,0.1970991f,0.2372021f,0.2776904f,
        0.3186394f,0.36013f,0.4022501f,0.4450965f,0.4887764f,0.5334097f,0.5791322f,
        0.626099f,0.6744898f,0.7245144f,0.7764218f,0.8305109f,0.8871466f,0.9467818f,
        1.00999f,1.077516f,1.150349f,1.229859f,1.318011f,1.417797f,1.534121f,1.67594f,
        1.862732f,2.153875f
    };

    static float d[31] = {
        0.0f,0.0f,0.0f,0.0f,0.0f,0.2636843f,0.2425085f,0.2255674f,0.2116342f,0.1999243f,
        0.1899108f,0.1812252f,0.1736014f,0.1668419f,0.1607967f,0.1553497f,0.1504094f,
        0.1459026f,0.14177f,0.1379632f,0.1344418f,0.1311722f,0.128126f,0.1252791f,
        0.1226109f,0.1201036f,0.1177417f,0.1155119f,0.1134023f,0.1114027f,0.1095039f
    };

    static float t[31] = {
        7.673828E-4f,2.30687E-3f,3.860618E-3f,5.438454E-3f,7.0507E-3f,8.708396E-3f,
        1.042357E-2f,1.220953E-2f,1.408125E-2f,1.605579E-2f,1.81529E-2f,2.039573E-2f,
        2.281177E-2f,2.543407E-2f,2.830296E-2f,3.146822E-2f,3.499233E-2f,3.895483E-2f,
        4.345878E-2f,4.864035E-2f,5.468334E-2f,6.184222E-2f,7.047983E-2f,8.113195E-2f,
        9.462444E-2f,0.1123001f,0.136498f,0.1716886f,0.2276241f,0.330498f,0.5847031f
    };

    static float h[31] = {
        3.920617E-2f,3.932705E-2f,3.951E-2f,3.975703E-2f,4.007093E-2f,4.045533E-2f,
        4.091481E-2f,4.145507E-2f,4.208311E-2f,4.280748E-2f,4.363863E-2f,4.458932E-2f,
        4.567523E-2f,4.691571E-2f,4.833487E-2f,4.996298E-2f,5.183859E-2f,5.401138E-2f,
        5.654656E-2f,5.95313E-2f,6.308489E-2f,6.737503E-2f,7.264544E-2f,7.926471E-2f,
        8.781922E-2f,9.930398E-2f,0.11556f,0.1404344f,0.1836142f,0.2790016f,0.7010474f
    };

    int i;
    float snorm, u, s, ustar, aa, w, y, tt;

    u = RNG::GenerateUniformFloat01();
    s = 0.0;
    if (u > 0.5) s = 1.0;
    u += (u - s);
    u *= 32.0f;
    i = (int)(u);
    if (i == 32) i = 31;
    if (i == 0) goto S100;

    /*
                                    START CENTER
    */
    ustar = u - (float)i;
    aa    = a[i-1];

S40:
    if (ustar > t[i-1])
    {
        w = (ustar - t[i-1]) * h[i-1];

S50:
        /*
        EXIT   (BOTH CASES)
        */
        y     = aa + w;
        snorm = y;
        if (s == 1.0) snorm = -y;
        return snorm;
    }

/*
                                CENTER CONTINUED
*/
    u = RNG::GenerateUniformFloat01();
    w = u * (a[i] - aa);
    tt = (0.5f * w + aa) * w;
    goto S80;

S70:
    tt = u;
    ustar = RNG::GenerateUniformFloat01();

S80:
    if (ustar > tt) goto S50;
    u = RNG::GenerateUniformFloat01();
    if (ustar >= u) goto S70;
    ustar = RNG::GenerateUniformFloat01();
    goto S40;

S100:
/*
                                START TAIL
*/
    i = 6;
    aa = a[31];
    goto S120;

S110:
    aa += d[i-1];
    i++;

S120:
    u += u;
    if (u < 1.0) goto S110;
    u -= 1.0;

S140:
    w  = u * d[i-1];
    tt = (0.5f * w + aa) * w;
    goto S160;

S150:
    tt = u;

S160:
    ustar = RNG::GenerateUniformFloat01();
    if (ustar > tt) goto S50;
    u = RNG::GenerateUniformFloat01();
    if (ustar >= u) goto S150;
    u = RNG::GenerateUniformFloat01();
    goto S140;
}

/* Transfers sign of argument sign to argument num */
float RNG::fsign(float num, float sign )
{
    if ((sign > 0.0f && num < 0.0f) || (sign < 0.0f && num>0.0f))
        return -num;
    else
        return num;
}

/************************************************************************
FTNSTOP:
Prints msg to standard error and then exits
************************************************************************/
void RNG::ftnstop(char* msg)
/* msg - error message */
{
    if (msg != NULL) fprintf(stderr,"%s\n",msg);
    exit(-1);
}

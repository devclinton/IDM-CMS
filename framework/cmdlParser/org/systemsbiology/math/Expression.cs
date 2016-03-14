/*
* Copyright (C) 2003 by Institute for Systems Biology,
* Seattle, Washington, USA.  All rights reserved.
*
* This source code is distributed under the GNU Lesser
* General Public License, the text of which is available at:
*   http://www.gnu.org/copyleft/lesser.html
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

using org.systemsbiology.util;

//UPGRADE_TODO: The package 'java.util.regex' could not be found. If it was not included in the conversion, there may be compiler issues. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1262'"
//using java.util.regex;

namespace org.systemsbiology.math
{

    /// <summary> This class is a general-purpose facility for parsing simple mathematical
    /// expressions in floating-point arithmetic involving [b]symbols[/b], [b]functions[/b],
    /// and simple [b]operators[/b].
    /// [p /]
    /// [b]operators:[/b] &nbsp;&nbsp;&nbsp; The allowed binary operator symbols are [code]*[/code],
    /// [code]-[/code], [code]/[/code], [code]+[/code], and [code]^[/code], [code]%[/code]
    /// corresponding to multiplication, subtraction, division, addition,
    /// exponentiation, and modulo division, respectively.  The only allowed unary operator
    /// is [code]-[/code], which is negation.  Modular division is special in that
    /// it is computed with floating point operands, just like the &quot;[code]%[/code]&quot;
    /// operator in the Java language.  Integer modulo division is simply a special case
    /// of the more general floating-point modulo division.
    /// [p /]
    /// [b]functions:[/b] &nbsp;&nbsp;&nbsp; A small library of built-in functions
    /// is supported; the function names are [em]case-sensitive[/em], and parentheses
    /// required in order for the parser to detect a function call:
    /// [code]exp()[/code], [code]ln()[/code], [code]sin()[/code],
    /// [code]cos()[/code], [code]tan()[/code], [code]asin()[/code],
    /// [code]acos()[/code], [code]atan()[/code], [code]abs()[/code],
    /// [code]floor()[/code], [code]ceil()[/code], [code]sqrt()[/code],
    /// [code]theta()[/code], [code]min()[/code], [code]max()[/code].
    /// The arguments of the trigonometric functions ([code]sin, cos, tan[/code])
    /// [em]must[/em] be in radians.  The return values of the inverse trigonometric
    /// functions ([code]asin, acos, atan[/code]) are in radians as well.  For more
    /// details on the built-in functions, please refer to the
    /// [A HREF="http://java.sun.com/j2se/1.4.1/docs/api/java/lang/Math.html"][code]java.lang.Math[/code][/A]
    /// documentation.
    /// [p /]
    /// [b]expressions:[/b] &nbsp;&nbsp;&nbsp; Parentheses may be used
    /// to group elements of a sub-expression.  The operator symbols,
    /// parentheses, and whitespace are used to tokenize the expression
    /// string; the resulting tokens are taken to be &quot;symbols&quot;
    /// on which the operators are to be applied.  The following reserved characters
    /// may not be used in an expression: [code]!@#$%[]|&amp;&gt;&lt;{},[/code].
    /// These characters are
    /// reserved for future features of this expression class.  Function calls are not
    /// allowed within expressions.  Note that bare numbers are permitted in
    /// expressions, provided they parse as floating-point or integer numbers,
    /// but they must not contain plus or minus characters.  For example, the
    /// number &quot;1.5&quot; appearing in an expression is correctly parsed
    /// as a floating-point value, but the number &quot;1.5e-7&quot; is
    /// [em]not[/em] parsed as a floating-point value, because the tokenizer
    /// interprets the hyphen character as denoting either negation or subtraction.
    /// The fact that numbers are interpreted as values means that you [em]cannot[/em]
    /// specify a symbol in an expression that would otherwise be interpreted as
    /// a floating-point number.  For example, if you input the expression
    /// &quot;A + 10&quot;, the &quot;10&quot; will always be interpreted as a
    /// value, and never as a symbol.  It is best if you make it a rule to always
    /// begin symbol names with an alphabetic character.
    /// [p /]
    /// Precedence rules are taken from the
    /// C programming language.  The order of precedence for the above-defined
    /// operators and symbols is:
    /// [ol]
    /// [li]parentheses and function calls[/li]
    /// [li]unary operators (negation)[/li]
    /// [li]power exponentiation[/li]
    /// [li]multiplication and division[/li]
    /// [li]addition and subtraction[/li]
    /// [/ol]
    /// where the increasing number represents [em]decreasing[/em] precedence.
    /// [p /]
    /// Example legal expressions
    /// would be:
    /// [blockquote]
    /// [pre]
    /// A + B + C
    /// exp(A * (B + C))
    /// A^(B - C)
    /// sin(-1.7)
    /// (A + B) * (C + D)
    /// [/pre]
    /// [/blockquote]
    /// In the above, note that left-right associativity is used by the parser,
    /// so that the first example would parse as: [code](A + B) + C[/code].
    /// The following examples of [em]illegal[/em] expression statements, and
    /// would generate a parser exception:
    /// [blockquote]
    /// [pre]
    /// A (B + C)
    /// A B
    /// exp A
    /// A * ln(B)
    /// [/pre]
    /// [/blockquote]
    /// In the above, note that [code]ln(B)[/code] is not allowed, because
    /// the [code]MathExpression[/code] class does not support function calls within
    /// expressions.
    /// [p /]
    /// The following code fragment illustrates a sample use of this class:
    /// [blockquote]
    /// [pre]
    /// MathExpression exp = new MathExpression("(A + B)/C");
    /// System.out.println(exp.toString());
    /// [/pre]
    /// [/blockquote]
    /// the above code fragment will result in [code](A+B)/C[/code]
    /// being printed to standard output.  For a more non-trivial example code
    /// using this class, please refer to the [code]main()[/code] function below,
    /// which serves as a test program for this class.
    /// [p /]
    /// When an expression string has been parsed, the result is a
    /// parse tree rooted at a single object called the &quot;root node&quot;.
    /// As an example, the parse tree for the expression [code]ln(A * (B + C)) + D[/code]
    /// might look (conceptually) like this:
    /// [blockquote]
    /// [pre]
    /// add
    /// / \
    /// ln   D
    /// |
    /// mult
    /// /      \
    /// symb(A)  add
    /// /  \
    /// symb(B) symb(C)
    /// [/pre]
    /// [/blockquote]
    /// where &quot;mult&quot; represents an element whose [b]element code[/b]
    /// is [code]ElementCode.MULT[/code], and &quot;add&quot; represents an
    /// element whose element code is [code]ElementCode.ADD[/code].  Furthermore,
    /// the &quot;edges&quot; on the above graph represent links between an
    /// element and its &quot;operands&quot; (i.e., its child nodes, in the tree).
    /// The &quot;symb(A)&quot; notation means an element whose element code
    /// is [code]ElementCode.SYMBOL[/code], and whose [b]symbol name[/b] field
    /// is set to the string &quot;A&quot;.
    ///
    /// This class is capable of parsing numeric literals in scientific notation,
    /// such as 1.0e-7 and 2.7e+14.  The "e" may be in either lower- or upper-case.
    ///
    /// Thanks to Adam Duguid, for submitting a patch for handing the [code]min()[/code] and
    /// [code]max()[/code] functions.
    ///
    /// </summary>
    /// <author>  Stephen Ramsey
    /// </author>
    public class Expression : System.ICloneable
    {
        /*private*/ public Element RootElement
        {
            get
            {
                return (mRootElement);
            }

            /*========================================*
            * accessor/mutator methods
            *========================================*/

            set
            {
                mRootElement = value;
            }

        }
        virtual public bool SimpleNumber
        {
            get
            {
                return (mRootElement.mCode == ElementCode.NUMBER);
            }

        }
        virtual public double SimpleNumberValue
        {
            get
            {
                if (!SimpleNumber)
                {
                    throw new System.SystemException("not allowed to call getSimpleNumberValue() on non-simple expression");
                }
                return (mRootElement.mNumericValue);
            }

        }
        /*========================================*
        * constants
        *========================================*/
        private const System.String TOKEN_STRING_OPEN_PAREN = "(";
        private const System.String TOKEN_STRING_CLOSE_PAREN = ")";
        private const System.String TOKEN_STRING_MULT = "*";
        private const System.String TOKEN_STRING_PLUS = "+";
        private const System.String TOKEN_STRING_MINUS = "-";
        private const System.String TOKEN_STRING_DIV = "/";
        private const System.String TOKEN_STRING_POW = "^";
        private const System.String TOKEN_STRING_MOD = "%";
        private const System.String TOKEN_STRING_SEP = ",";

        private const System.String TOKEN_DELIMITERS = " *+-/^(),";
        private const System.String TOKEN_RESERVED = "!@#$[]|&><{}=";

        public static readonly Expression ZERO = new Expression("0.0");
        public static readonly Expression ONE = new Expression("1.0");

        /*========================================*
        * inner class
        *========================================*/

        public interface SymbolPrinter
        {
            System.String printSymbol(Symbol pSymbol);
        }

        internal sealed class TokenCode
        {
            private System.String mName;
            internal TokenCode(System.String pName)
            {
                mName = pName;
            }
            public override System.String ToString()
            {
                return (mName);
            }
            public static readonly TokenCode NONE = new TokenCode("none");
            public static readonly TokenCode OPEN_PAREN = new TokenCode("open paren");
            public static readonly TokenCode CLOSE_PAREN = new TokenCode("close paren");
            public static readonly TokenCode NUMBER = new TokenCode("number");
            public static readonly TokenCode MULT = new TokenCode("mult");
            public static readonly TokenCode PLUS = new TokenCode("plus");
            public static readonly TokenCode MINUS = new TokenCode("minus");
            public static readonly TokenCode DIV = new TokenCode("div");
            public static readonly TokenCode POW = new TokenCode("pow");
            public static readonly TokenCode MOD = new TokenCode("mod");
            public static readonly TokenCode SYMBOL = new TokenCode("symbol");
            public static readonly TokenCode EXPRESSION = new TokenCode("expression");
            public static readonly TokenCode SPACE = new TokenCode("space");
            public static readonly TokenCode SEP = new TokenCode("sep");
            public static readonly TokenCode EXPRESSION_PAIR = new TokenCode("expression pair");
        }

        //UPGRADE_NOTE: The access modifier for this class or class field has been changed in order to prevent compilation errors due to the visibility level. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1296'"
        sealed public class ElementCode
        {
            private System.String mName;

            private static Hashtable mFunctionsMap;
            public int mIntCode;
            public int mNumFunctionArgs;

            public const int NULL_FUNCTION_CODE = 0;
            public const int ELEMENT_CODE_NONE = 0;
            public const int ELEMENT_CODE_SYMBOL = 1;
            public const int ELEMENT_CODE_NUMBER = 2;
            public const int ELEMENT_CODE_MULT = 3;
            public const int ELEMENT_CODE_ADD = 4;
            public const int ELEMENT_CODE_SUBT = 5;
            public const int ELEMENT_CODE_DIV = 6;
            public const int ELEMENT_CODE_POW = 7;
            public const int ELEMENT_CODE_MOD = 8;
            public const int ELEMENT_CODE_NEG = 9;
            public const int ELEMENT_CODE_EXP = 10;
            public const int ELEMENT_CODE_LN = 11;
            public const int ELEMENT_CODE_SIN = 12;
            public const int ELEMENT_CODE_COS = 13;
            public const int ELEMENT_CODE_TAN = 14;
            public const int ELEMENT_CODE_ASIN = 15;
            public const int ELEMENT_CODE_ACOS = 16;
            public const int ELEMENT_CODE_ATAN = 17;
            public const int ELEMENT_CODE_ABS = 18;
            public const int ELEMENT_CODE_FLOOR = 19;
            public const int ELEMENT_CODE_CEIL = 20;
            public const int ELEMENT_CODE_SQRT = 21;
            public const int ELEMENT_CODE_THETA = 22;
            public const int ELEMENT_CODE_PAIR = 23;
            public const int ELEMENT_CODE_MIN = 24;
            public const int ELEMENT_CODE_MAX = 25;

            internal ElementCode(System.String pName, int pIntCode):this(pName, pIntCode, false, 0)
            {
            }

            internal ElementCode(System.String pName, int pIntCode, bool pIsFunction, int pNumFunctionArgs)
            {
                mName = pName;
                mIntCode = pIntCode;
                mNumFunctionArgs = pNumFunctionArgs;
                if (pIsFunction)
                {
                    putFunction(this);
                }
            }

            private void  putFunction(ElementCode pElementCode)
            {
                mFunctionsMap[pElementCode.mName] = pElementCode;
            }

            public static ElementCode getFunction(System.String pName)
            {
                //UPGRADE_TODO: Method 'java.util.HashMap.get' was converted to 'Hashtable.Item' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMapget_javalangObject'"
                return (ElementCode) (mFunctionsMap[pName]);
            }

            public bool isFunction()
            {
                return (null != getFunction(mName));
            }
            public override System.String ToString()
            {
                return (mName);
            }
            /// <summary> element code indicating an empty element (this should never
            /// occur in a valid expression parse tree)
            /// </summary>
            //UPGRADE_NOTE: The initialization of  'NONE' was moved to static method 'org.systemsbiology.math.Expression.ElementCode'. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1005'"
            public static readonly ElementCode NONE;


            /// <summary> element code indicating that the element is a &quot;symbol&quot;
            /// (like a variable)
            /// </summary>
            //UPGRADE_NOTE: The initialization of  'SYMBOL' was moved to static method 'org.systemsbiology.math.Expression.ElementCode'. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1005'"
            public static readonly ElementCode SYMBOL;

            /// <summary> element code indicating that the element is a &quot;value&quot;
            /// (like a number)
            /// </summary>
            //UPGRADE_NOTE: The initialization of  'NUMBER' was moved to static method 'org.systemsbiology.math.Expression.ElementCode'. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1005'"
            public static readonly ElementCode NUMBER;

            /// <summary> A pair of expressions.  This element type is a temporary placeholder, used during
            /// expression parsing,  that is not actually allowed in a final parsed expression.
            /// </summary>
            //UPGRADE_NOTE: The initialization of  'PAIR' was moved to static method 'org.systemsbiology.math.Expression.ElementCode'. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1005'"
            public static readonly ElementCode PAIR;

            /// <summary> element code indicating that the element is an operation
            /// element specifying the multiplication of its two child operands
            /// </summary>
            //UPGRADE_NOTE: The initialization of  'MULT' was moved to static method 'org.systemsbiology.math.Expression.ElementCode'. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1005'"
            public static readonly ElementCode MULT;

            /// <summary> element code indicating that the element is an operation
            /// element specifying the addition of its two child operands
            /// </summary>
            //UPGRADE_NOTE: The initialization of  'ADD' was moved to static method 'org.systemsbiology.math.Expression.ElementCode'. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1005'"
            public static readonly ElementCode ADD;

            /// <summary> element code indicating that the element is an operation
            /// element specifying the subtraction of its two child operands
            /// (the second operand is to be subtracted from the first)
            /// </summary>
            //UPGRADE_NOTE: The initialization of  'SUBT' was moved to static method 'org.systemsbiology.math.Expression.ElementCode'. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1005'"
            public static readonly ElementCode SUBT;

            /// <summary> element code indicating that the element is an operation
            /// element specifying the division of its two child operands
            /// </summary>
            //UPGRADE_NOTE: The initialization of  'DIV' was moved to static method 'org.systemsbiology.math.Expression.ElementCode'. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1005'"
            public static readonly ElementCode DIV;

            /// <summary> element code indicating that the element is an operation
            /// element specifying the exponentiation of its first operand
            /// by the value of the second operand
            /// </summary>
            //UPGRADE_NOTE: The initialization of  'POW' was moved to static method 'org.systemsbiology.math.Expression.ElementCode'. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1005'"
            public static readonly ElementCode POW;

            /// <summary> element code indicating that the element is an operation
            /// element specifying the modulus (remainder) of the quotient
            /// of the first and second operands.
            /// </summary>
            //UPGRADE_NOTE: The initialization of  'MOD' was moved to static method 'org.systemsbiology.math.Expression.ElementCode'. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1005'"
            public static readonly ElementCode MOD;

            /// <summary> element code indicating that the element is an operation
            /// element specifying the negation of its first (and only)
            /// operand
            /// </summary>
            //UPGRADE_NOTE: The initialization of  'NEG' was moved to static method 'org.systemsbiology.math.Expression.ElementCode'. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1005'"
            public static readonly ElementCode NEG;



            /// <summary> element code specifying the exponential function of the
            /// first (and only) argument
            /// </summary>
            //UPGRADE_NOTE: The initialization of  'EXP' was moved to static method 'org.systemsbiology.math.Expression.ElementCode'. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1005'"
            public static readonly ElementCode EXP;

            /// <summary> element code specifying the logarithm function (base e) of the
            /// first (and only) argument
            /// </summary>
            //UPGRADE_NOTE: The initialization of  'LN' was moved to static method 'org.systemsbiology.math.Expression.ElementCode'. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1005'"
            public static readonly ElementCode LN;

            /// <summary> element code specifying the sine function of the
            /// first (and only) argument; argument is in radians
            /// </summary>
            //UPGRADE_NOTE: The initialization of  'SIN' was moved to static method 'org.systemsbiology.math.Expression.ElementCode'. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1005'"
            public static readonly ElementCode SIN;

            /// <summary> element code specifying the cosine function of the
            /// first (and only) argument; argument is in radians
            /// </summary>
            //UPGRADE_NOTE: The initialization of  'COS' was moved to static method 'org.systemsbiology.math.Expression.ElementCode'. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1005'"
            public static readonly ElementCode COS;

            /// <summary> element code specifying the tangent function of the
            /// first (and only) argument; argument is in radians
            /// </summary>
            //UPGRADE_NOTE: The initialization of  'TAN' was moved to static method 'org.systemsbiology.math.Expression.ElementCode'. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1005'"
            public static readonly ElementCode TAN;

            /// <summary> element code specifying the inverse sine function of the
            /// first (and only) argument; argument is in the range [-1,1];
            /// return value is in radians
            /// </summary>
            //UPGRADE_NOTE: The initialization of  'ASIN' was moved to static method 'org.systemsbiology.math.Expression.ElementCode'. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1005'"
            public static readonly ElementCode ASIN;

            /// <summary> element code specifying the inverse cosine function of the
            /// first (and only) argument; argument is in the range [-1,1];
            /// return value is in radians
            /// </summary>
            //UPGRADE_NOTE: The initialization of  'ACOS' was moved to static method 'org.systemsbiology.math.Expression.ElementCode'. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1005'"
            public static readonly ElementCode ACOS;

            /// <summary> element code specifying the inverse tangent function of the
            /// first (and only) argument; return value is in radians
            /// </summary>
            //UPGRADE_NOTE: The initialization of  'ATAN' was moved to static method 'org.systemsbiology.math.Expression.ElementCode'. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1005'"
            public static readonly ElementCode ATAN;

            /// <summary> element code specifying the absolute value of the
            /// first (and only) argument
            /// </summary>
            //UPGRADE_NOTE: The initialization of  'ABS' was moved to static method 'org.systemsbiology.math.Expression.ElementCode'. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1005'"
            public static readonly ElementCode ABS;

            /// <summary> element code specifying the greatest integer value that is
            /// less than or equal to the argument
            /// </summary>
            //UPGRADE_NOTE: The initialization of  'FLOOR' was moved to static method 'org.systemsbiology.math.Expression.ElementCode'. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1005'"
            public static readonly ElementCode FLOOR;

            /// <summary> element code specifying the smallest integer value that is
            /// greater than or equal to the argument
            /// </summary>
            //UPGRADE_NOTE: The initialization of  'CEIL' was moved to static method 'org.systemsbiology.math.Expression.ElementCode'. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1005'"
            public static readonly ElementCode CEIL;

            /// <summary> element code specifying the square root of the argument, which
            /// must be nonnegative.
            /// </summary>
            //UPGRADE_NOTE: The initialization of  'SQRT' was moved to static method 'org.systemsbiology.math.Expression.ElementCode'. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1005'"
            public static readonly ElementCode SQRT;

            /// <summary> element code specifying the Heaviside step function ("theta" function)</summary>
            //UPGRADE_NOTE: The initialization of  'THETA' was moved to static method 'org.systemsbiology.math.Expression.ElementCode'. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1005'"
            public static readonly ElementCode THETA;

            /// <summary> element code specifying the min function returning the
            /// smaller of the two arguments
            /// </summary>
            //UPGRADE_NOTE: The initialization of  'MIN' was moved to static method 'org.systemsbiology.math.Expression.ElementCode'. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1005'"
            public static readonly ElementCode MIN;

            /// <summary> element code specifying the max function returning the
            /// larger of the two arguments
            /// </summary>
            //UPGRADE_NOTE: The initialization of  'MAX' was moved to static method 'org.systemsbiology.math.Expression.ElementCode'. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1005'"
            public static readonly ElementCode MAX;
            static ElementCode()
            {
                {

                    mFunctionsMap = new Hashtable();
                }
                NONE = new ElementCode("none", ELEMENT_CODE_NONE);
                SYMBOL = new ElementCode("symbol", ELEMENT_CODE_SYMBOL);
                NUMBER = new ElementCode("number", ELEMENT_CODE_NUMBER);
                PAIR = new ElementCode("pair", ELEMENT_CODE_PAIR);
                MULT = new ElementCode("mult", ELEMENT_CODE_MULT);
                ADD = new ElementCode("add", ELEMENT_CODE_ADD);
                SUBT = new ElementCode("subt", ELEMENT_CODE_SUBT);
                DIV = new ElementCode("div", ELEMENT_CODE_DIV);
                POW = new ElementCode("pow", ELEMENT_CODE_POW);
                MOD = new ElementCode("mod", ELEMENT_CODE_MOD);
                NEG = new ElementCode("neg", ELEMENT_CODE_NEG);
                EXP = new ElementCode("exp", ELEMENT_CODE_EXP, true, 1);
                LN = new ElementCode("ln", ELEMENT_CODE_LN, true, 1);
                SIN = new ElementCode("sin", ELEMENT_CODE_SIN, true, 1);
                COS = new ElementCode("cos", ELEMENT_CODE_COS, true, 1);
                TAN = new ElementCode("tan", ELEMENT_CODE_TAN, true, 1);
                ASIN = new ElementCode("asin", ELEMENT_CODE_ASIN, true, 1);
                ACOS = new ElementCode("acos", ELEMENT_CODE_ACOS, true, 1);
                ATAN = new ElementCode("atan", ELEMENT_CODE_ATAN, true, 1);
                ABS = new ElementCode("abs", ELEMENT_CODE_ABS, true, 1);
                FLOOR = new ElementCode("floor", ELEMENT_CODE_FLOOR, true, 1);
                CEIL = new ElementCode("ceil", ELEMENT_CODE_CEIL, true, 1);
                SQRT = new ElementCode("sqrt", ELEMENT_CODE_SQRT, true, 1);
                THETA = new ElementCode("theta", ELEMENT_CODE_THETA, true, 1);
                MIN = new ElementCode("min", ELEMENT_CODE_MIN, true, 2);
                MAX = new ElementCode("max", ELEMENT_CODE_MAX, true, 2);
            }
        }

        /// <summary> Data structure defining a single element of a parse tree for a
        /// mathematical expression.  An element can be either a [b]symbol element[/b], or an
        /// [b]operation element[/b] (unary or binary operators are allowed).
        /// In the case of a symbol element, the [code]mFirstOperand[/code]
        /// and [code]mSecondOperand[/code] fields are [code]null[/code],
        /// and the symbol name is stored in the string [code]mSymbolName[/code].
        /// In the case of an operation element with a unary operator (the
        /// negation operator is the only such operator allowed), the operand
        /// is an {@link Expression.Element} pointed to by the [code]mFirstOperand[/code]
        /// field.  In the case of an operation element  with a binary
        /// operator, the first operand is pointed to by the [code]mFirstOperand[/code]
        /// field, and the second operand is pointed to by the [code]mSecondOperand[/code]
        /// field.  In both the unary and binary operator cases, the [code]mCode[/code]
        /// field stores an integer [b]element code[/b] indicating the type of operator.
        ///
        /// For a list of element codes, refer to the {@link Expression} class.
        /// </summary>
        /*internal*/public sealed class Element : System.ICloneable
        {
            public bool Atomic
            {
                get
                {
                    return (mCode.Equals(org.systemsbiology.math.Expression.ElementCode.SYMBOL) || mCode.Equals(org.systemsbiology.math.Expression.ElementCode.NUMBER));
                }

            }
            internal static readonly Element ONE = new Element(1.0);
            internal static readonly Element TWO = new Element(2.0);

            public Element(ElementCode pCode)
            {
                mCode = pCode;
            }

            public Element(double pNumericValue)
            {
                mCode = org.systemsbiology.math.Expression.ElementCode.NUMBER;
                mNumericValue = pNumericValue;
            }

            /// <summary> The element code indicating whether the element is a
            /// [b]symbol element[/b] or an [b]operation element[/b], and
            /// (in the case of an operation element) the type of operator.
            /// For a valid parse tree, the [code]mCode[/code] field should
            /// never take the value of [code]ElementCode.NONE = 0[/code], which
            /// is reseved for an unused element field.
            /// </summary>
            public ElementCode mCode;

            /// <summary> For a binary or unary operation element, this field is a
            /// reference pointing to the first operand, which is itself
            /// an {@link Expression.Element}.  This field should be [code]null[/code]
            /// for the case of a symbol element.
            /// </summary>
            public Element mFirstOperand;

            /// <summary> For a binary element, this field is a
            /// reference pointing to the second operand, which is itself
            /// an {@link Expression.Element}.  For a unary operation element or
            /// a symbol element, this field should be null.
            /// </summary>
            public Element mSecondOperand;

            /// <summary> For a symbol element, this field contains the symbol name. </summary>
            public Symbol mSymbol;

            /// <summary> For a number element, this field contains the element value;</summary>
            public double mNumericValue;

            /// <summary> Returns a human-comprehensible textual description of this
            /// mathematical expression.  This method assumes that the expression
            /// has been &quot;set&quot; by calling {@link #setExpression(String)}
            /// or by using the parameterized constructor.
            ///
            /// </summary>
            /// <returns> a human-comprehensible textual description of this
            /// mathematical expression.
            /// </returns>
            public System.String toString(Expression.SymbolPrinter pSymbolPrinter)
            {
                ElementCode code = mCode;
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                if (code == org.systemsbiology.math.Expression.ElementCode.NEG)
                {
                    sb.Append(org.systemsbiology.math.Expression.TOKEN_STRING_MINUS);
                    if (mFirstOperand.mCode != org.systemsbiology.math.Expression.ElementCode.SYMBOL)
                    {
                        sb.Append("(");
                    }
                    sb.Append(mFirstOperand.toString(pSymbolPrinter));
                    if (mFirstOperand.mCode != org.systemsbiology.math.Expression.ElementCode.SYMBOL)
                    {
                        sb.Append(")");
                    }
                }
                else
                {
                    if (mCode.isFunction())
                    {
                        sb.Append(mCode + "(");

                        int numArgs = mCode.mNumFunctionArgs;
                        if (numArgs > 0)
                        {
                            sb.Append(mFirstOperand.toString(pSymbolPrinter));
                            if (numArgs > 1)
                            {
                                sb.Append(", " + mSecondOperand.toString(pSymbolPrinter));
                            }
                        }
                        sb.Append(")");
                    }
                    else
                    {
                        System.String operatorSymbol = org.systemsbiology.math.Expression.getBinaryOperatorSymbol(code);
                        if (null != operatorSymbol)
                        {
                            if (!mFirstOperand.Atomic)
                            {
                                sb.Append("(");
                            }
                            sb.Append(mFirstOperand.toString(pSymbolPrinter));
                            if (!mFirstOperand.Atomic)
                            {
                                sb.Append(")");
                            }
                            sb.Append(operatorSymbol);
                            if (!mSecondOperand.Atomic)
                            {
                                sb.Append("(");
                            }
                            sb.Append(mSecondOperand.toString(pSymbolPrinter));
                            if (!mSecondOperand.Atomic)
                            {
                                sb.Append(")");
                            }
                        }
                        else if (code == org.systemsbiology.math.Expression.ElementCode.PAIR)
                        {
                            throw new System.SystemException("invalid element code: pair");
                        }
                        else if (code == org.systemsbiology.math.Expression.ElementCode.SYMBOL)
                        {
                            if (null != pSymbolPrinter)
                            {
                                sb.Append(pSymbolPrinter.printSymbol(mSymbol));
                            }
                            else
                            {
                                sb.Append(mSymbol.Name);
                            }
                        }
                        else if (code == org.systemsbiology.math.Expression.ElementCode.NUMBER)
                        {
                            sb.Append(mNumericValue);
                        }
                        else
                        {
                            throw new System.SystemException("invalid element code encountered; code is: " + code);
                        }
                    }
                }

                return (sb.ToString());
            }

            public System.Object Clone()
            {
                Element newElement = new Element(mCode);

                if (null != mFirstOperand)
                {
                    newElement.mFirstOperand = (Element) mFirstOperand.Clone();
                }
                else
                {
                    newElement.mFirstOperand = null;
                }

                if (null != mSecondOperand)
                {
                    newElement.mSecondOperand = (Element) mSecondOperand.Clone();
                }
                else
                {
                    newElement.mSecondOperand = null;
                }

                newElement.mNumericValue = mNumericValue;
                if (null != mSymbol)
                {
                    newElement.mSymbol = (Symbol) mSymbol.Clone();
                }
                else
                {
                    newElement.mSymbol = null;
                }

                return (newElement);
            }
        }

        //UPGRADE_NOTE: Field 'EnclosingInstance' was added to class 'Token' to access its enclosing instance. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1019'"
        // represents an element of the token list for a tokenized
        // mathematical epxression
        private class Token
        {
            private void  InitBlock(Expression enclosingInstance)
            {
                this.enclosingInstance = enclosingInstance;
            }
            private Expression enclosingInstance;
            public Expression Enclosing_Instance
            {
                get
                {
                    return enclosingInstance;
                }

            }
            internal TokenCode mCode;
            internal System.String mSymbolName;
            internal Element mParsedExpression;
            internal double mNumericValue;
            internal Element mSecondParsedExpression;

            public override System.String ToString()
            {
                System.String retVal = null;
                if (mCode.Equals(org.systemsbiology.math.Expression.TokenCode.SYMBOL))
                {
                    retVal = mSymbolName;
                }
                else if (mCode.Equals(org.systemsbiology.math.Expression.TokenCode.NUMBER))
                {
                    retVal = mNumericValue.ToString();
                }
                else if (mCode.Equals(org.systemsbiology.math.Expression.TokenCode.EXPRESSION))
                {
                    retVal = mParsedExpression.mCode.ToString();
                }
                else
                {
                    retVal = mCode.ToString();
                }
                return retVal;
            }

            public Token(Expression enclosingInstance)
            {
                InitBlock(enclosingInstance);
                mCode = org.systemsbiology.math.Expression.TokenCode.NONE;
                mSymbolName = null;
                mParsedExpression = null;
                mSecondParsedExpression = null;
            }
        }

        /*========================================*
        * member data
        *========================================*/
        private Element mRootElement;
        private SymbolEvaluatorHashMap mSymbolEvaluator;

        /*========================================*
        * initialization methods
        *========================================*/
        private void  initializeRootElement()
        {
            RootElement = null;
        }

        private void  initialize()
        {
            initializeRootElement();
            mSymbolEvaluator = null;
        }

        /*========================================*
        * constructors
        *========================================*/
        protected internal Expression()
        {
            initialize();
        }

        public Expression(double pValue)
        {
            initialize();
            Element rootElement = new Element(ElementCode.NUMBER);
            rootElement.mNumericValue = pValue;
            RootElement = rootElement;
        }

        public Expression(System.String pExpression)
        {
            initialize();
            RootElement = parseExpression(pExpression);
        }

        /*========================================*
        * private methods
        *========================================*/

        private SymbolEvaluator getSymbolEvaluator(Hashtable pSymbolsMap)
        {
            if (null == mSymbolEvaluator)
            {
                mSymbolEvaluator = new SymbolEvaluatorHashMap(pSymbolsMap);
            }

            return (mSymbolEvaluator);
        }

        private void  checkForReservedCharacters(System.String pFormula)
        {
            System.String tokenReserved = TOKEN_RESERVED;
            int numReservedChars = tokenReserved.Length;
            for (int charCtr = 0; charCtr < numReservedChars; ++charCtr)
            {
                System.String reservedChar = tokenReserved.Substring(charCtr, (charCtr + 1) - (charCtr));
                int index = pFormula.IndexOf(reservedChar);
                if (index != - 1)
                {
                    throw new System.ArgumentException("expression contained reserved character: " + reservedChar + " at position " + index);
                }
            }
        }


/*J
        private System.Double parseDoubleSafe(System.String pString)
        {
            //UPGRADE_TODO: The 'System.Double' structure does not have an equivalent to NULL. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1291'"
            System.Double retVal = null;
            try
            {
                //UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
                retVal = System.Double.Parse(pString);
            }
            catch (System.FormatException e)
            {
                // do nothing
            }
            return (retVal);
        }
*/

/*J
        private System.Int32 parseIntegerSafe(System.String pString)
        {
            //UPGRADE_TODO: The 'System.Int32' structure does not have an equivalent to NULL. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1291'"
            System.Int32 retVal = null;

            try
            {
                retVal = System.Int32.Parse(pString);
            }
            catch (System.FormatException e)
            {
                // do nothing
            }
            return (retVal);
        }
*/

        private void  handleScientificNotationNumericToken(System.String pPrefix, SupportClass.Tokenizer pStringTokenizer, Token pToken, IList pTokenizedFormula, double pMultiplier)
        {
            //J System.Double prefixValue = parseDoubleSafe(pPrefix);
            System.Double prefixValue = 0;
            if (!Double.TryParse(pPrefix, out prefixValue))
                Debug.Assert(false, "invalid scientific notation prefix");

            // get next token, to obtain the power of 10
            if (!pStringTokenizer.HasMoreTokens())
            {
                throw new System.ArgumentException("scientific notation number missing exponent, \"" + pPrefix + "\"");
            }

            System.String nextTokenString = pStringTokenizer.NextToken();

            //J System.Int32 nextTokenInt = parseIntegerSafe(nextTokenString);
            System.Int32 nextTokenInt = 0;
            if (!Int32.TryParse(nextTokenString, out nextTokenInt))
/*J
            //UPGRADE_TODO: The 'System.Int32' structure does not have an equivalent to NULL. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1291'"
            if (null == nextTokenInt)
*/
            {
                throw new System.ArgumentException("scientific notation number missing exponent, \"" + nextTokenString + "\"");
            }
            double value_Renamed = prefixValue * System.Math.Pow(10.0, pMultiplier * ((double) nextTokenInt));
            pTokenizedFormula.RemoveAt(pTokenizedFormula.Count - 1);
            pToken.mCode = TokenCode.NUMBER;
            pToken.mNumericValue = value_Renamed;
        }

        private IList<Token> tokenizeExpression(System.String pFormula)
        {
            checkForReservedCharacters(pFormula);

            List<Token> tokenizedFormula = new List<Token>();
            bool returnDelims = true;
            SupportClass.Tokenizer stringTokenizer = new SupportClass.Tokenizer(pFormula, TOKEN_DELIMITERS, returnDelims);

            //J Pattern scientificNotationPattern = Pattern.compile("(\\d+(\\.\\d*)?)[eE]");
            Regex scientificNotationPattern = new Regex("(\\d+(\\.\\d*)?)[eE]");

            while (stringTokenizer.HasMoreTokens())
            {
                System.String tokenStr = stringTokenizer.NextToken();

                Token token = new Token(this);

                if (0 == tokenStr.Trim().Length)
                {
                    token.mCode = TokenCode.SPACE;
                }
                else if (tokenStr.Equals(TOKEN_STRING_OPEN_PAREN))
                {
                    token.mCode = TokenCode.OPEN_PAREN;
                }
                else if (tokenStr.Equals(TOKEN_STRING_CLOSE_PAREN))
                {
                    token.mCode = TokenCode.CLOSE_PAREN;
                }
                else if (tokenStr.Equals(TOKEN_STRING_MULT))
                {
                    token.mCode = TokenCode.MULT;
                }
                else if (tokenStr.Equals(TOKEN_STRING_PLUS))
                {
                    // check previous token to see if it is a number
                    Token lastToken = null;
                    int formulaSize = tokenizedFormula.Count;
                    if (formulaSize > 0)
                    {
                        lastToken = (Token) tokenizedFormula[formulaSize - 1];
                        Debug.Assert(null != lastToken, "invalid null token found");
                        if (lastToken.mCode.Equals(TokenCode.SYMBOL))
                        {
                            System.String lastTokenName = lastToken.mSymbolName;
/*J
                            Matcher scientificNotationMatcher = scientificNotationPattern.matcher(lastTokenName);
                            if (scientificNotationMatcher.matches())
                            {
                                System.String scientificNotationPrefix = scientificNotationMatcher.group(1);
                                handleScientificNotationNumericToken(scientificNotationPrefix, stringTokenizer, token, tokenizedFormula, 1.0);
                            }
*/
                            Match m = scientificNotationPattern.Match(lastTokenName);
                            if (scientificNotationPattern.IsMatch(lastTokenName))
                            {
                                System.String scientificNotationPrefix = m.Groups[1].ToString();
                                handleScientificNotationNumericToken(scientificNotationPrefix, stringTokenizer, token, tokenizedFormula, 1.0);
                            }
                            else
                            {
                                token.mCode = TokenCode.PLUS;
                            }
                        }
                        else
                        {
                            token.mCode = TokenCode.PLUS;
                        }
                    }
                    else
                    {
                        token.mCode = TokenCode.PLUS;
                    }
                }
                else if (tokenStr.Equals(TOKEN_STRING_MINUS))
                {
                    // check previous token to see if it is a number
                    Token lastToken = null;
                    int formulaSize = tokenizedFormula.Count;
                    if (formulaSize > 0)
                    {
                        lastToken = (Token) tokenizedFormula[formulaSize - 1];
                        Debug.Assert(null != lastToken, "invalid null token found");
                        if (lastToken.mCode.Equals(TokenCode.SYMBOL))
                        {
                            System.String lastTokenName = lastToken.mSymbolName;
/*J
                            Matcher scientificNotationMatcher = scientificNotationPattern.matcher(lastTokenName);
                            if (scientificNotationMatcher.matches())
                            {
                                System.String scientificNotationPrefix = scientificNotationMatcher.group(1);
                                handleScientificNotationNumericToken(scientificNotationPrefix, stringTokenizer, token, tokenizedFormula, - 1.0);
                            }
*/
                            Match m = scientificNotationPattern.Match(lastTokenName);
                            if (scientificNotationPattern.IsMatch(lastTokenName))
                            {
                                System.String scientificNotationPrefix = m.Groups[1].ToString();
                                handleScientificNotationNumericToken(scientificNotationPrefix, stringTokenizer, token, tokenizedFormula, -1.0);
                            }
                            else
                            {
                                token.mCode = TokenCode.MINUS;
                            }
                        }
                        else
                        {
                            token.mCode = TokenCode.MINUS;
                        }
                    }
                    else
                    {
                        token.mCode = TokenCode.MINUS;
                    }
                }
                else if (tokenStr.Equals(TOKEN_STRING_DIV))
                {
                    token.mCode = TokenCode.DIV;
                }
                else if (tokenStr.Equals(TOKEN_STRING_POW))
                {
                    token.mCode = TokenCode.POW;
                }
                else if (tokenStr.Equals(TOKEN_STRING_MOD))
                {
                    token.mCode = TokenCode.MOD;
                }
                else if (tokenStr.Equals(TOKEN_STRING_SEP))
                {
                    token.mCode = TokenCode.SEP;
                }
                else
                {
                    System.Double valueObj = 0;
                    if (Double.TryParse(tokenStr, out valueObj))
                    //J if (null != valueObj)
                    {
                        double value_Renamed = valueObj;
                        token.mCode = TokenCode.NUMBER;
                        token.mNumericValue = value_Renamed;
                    }
                    else
                    {
                        token.mCode = TokenCode.SYMBOL;
                        token.mSymbolName = tokenStr;
                    }
                }
                tokenizedFormula.Add(token);
            }

/*
            // strip all the whitespace tokens out of the final tokenized formula
            IEnumerator listIter = tokenizedFormula.GetEnumerator();

            while (listIter.MoveNext())
            {

                Token token = (Token) listIter.Current;
                if (token.mCode.Equals(TokenCode.SPACE))
                {
                    //UPGRADE_ISSUE: Method 'java.util.ListIterator.remove' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javautilListIteratorremove'"
                    listIter.remove();
                }
            }

            return (tokenizedFormula);
*/
            return new List<Token>(from token in tokenizedFormula where !token.mCode.Equals(TokenCode.SPACE) select token);
        }

        private Element convertTokenToElement(Token pToken)
        {
            TokenCode tokCode = pToken.mCode;
            if (tokCode != TokenCode.EXPRESSION && tokCode != TokenCode.SYMBOL && tokCode != TokenCode.NUMBER && tokCode != TokenCode.EXPRESSION_PAIR)
            {
                throw new System.ArgumentException("expected a sub-expression, but instead found unexpected token: " + tokCode);
            }
            Element retVal;
            if (tokCode == TokenCode.EXPRESSION)
            {
                retVal = pToken.mParsedExpression;
            }
            else if (tokCode.Equals(TokenCode.EXPRESSION_PAIR))
            {
                retVal = new Element(ElementCode.PAIR);
                retVal.mFirstOperand = (Element) pToken.mParsedExpression;
                retVal.mSecondOperand = (Element) pToken.mSecondParsedExpression;
            }
            else
            {
                if (tokCode == TokenCode.NUMBER)
                {
                    retVal = new Element(ElementCode.NUMBER);
                    retVal.mNumericValue = pToken.mNumericValue;
                }
                else
                {
                    retVal = new Element(ElementCode.SYMBOL);
                    retVal.mSymbol = new Symbol(pToken.mSymbolName);
                }
            }
            return (retVal);
        }


        static private System.String getBinaryOperatorSymbol(ElementCode pElementOperatorCode)
        {
            System.String retVal = null;

            if (pElementOperatorCode == ElementCode.MULT)
            {
                retVal = TOKEN_STRING_MULT;
            }
            else if (pElementOperatorCode == ElementCode.ADD)
            {
                retVal = TOKEN_STRING_PLUS;
            }
            else if (pElementOperatorCode == ElementCode.SUBT)
            {
                retVal = TOKEN_STRING_MINUS;
            }
            else if (pElementOperatorCode == ElementCode.DIV)
            {
                retVal = TOKEN_STRING_DIV;
            }
            else if (pElementOperatorCode == ElementCode.POW)
            {
                retVal = TOKEN_STRING_POW;
            }
            else if (pElementOperatorCode == ElementCode.MOD)
            {
                retVal = TOKEN_STRING_MOD;
            }
            return (retVal);
        }

        static private ElementCode parseFunctionName(System.String pFunctionName)
        {
            return (ElementCode.getFunction(pFunctionName));
        }

        private void  parseParentheses(IList<Token> pTokenizedExpression)
        {
            int parenDepth = 0;
            int tokenCtr = 0;
            IEnumerator<Token> iter = pTokenizedExpression.GetEnumerator();
            IList<Token> subFormula = null;

            Token tok = null;
            Token prevTok = null;

            // parse parentheses first, since they have the highest level of precedence
            for (int iToken = 0; iToken < pTokenizedExpression.Count; iToken++)
            {
                prevTok = tok;

                tok = pTokenizedExpression[iToken];
                TokenCode tokenCode = tok.mCode;

                if (parenDepth > 1 || (parenDepth == 1 && tokenCode != TokenCode.CLOSE_PAREN))
                {
                    //UPGRADE_ISSUE: Method 'java.util.ListIterator.remove' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javautilListIteratorremove'"
                    //J iter.remove();
                    pTokenizedExpression.RemoveAt(iToken--);    // decrement to offset loop increment
                    subFormula.Add(tok);
                }

                if (tokenCode == TokenCode.OPEN_PAREN)
                {
                    if (parenDepth == 0)
                    {
                        subFormula = new List<Token>();
                        //UPGRADE_ISSUE: Method 'java.util.ListIterator.remove' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javautilListIteratorremove'"
                        //J iter.remove();
                        pTokenizedExpression.RemoveAt(iToken--);    // decrement to offset loop increment
                    }
                    ++parenDepth;
                }
                else if (tokenCode == TokenCode.CLOSE_PAREN)
                {
                    --parenDepth;
                    if (parenDepth < 0)
                    {
                        throw new System.ArgumentException("invalid parenthesis encountered for token number: " + tokenCtr);
                    }
                    if (parenDepth == 0)
                    {
                        // we just completed a parenthetical expression
                        tok.mCode = TokenCode.EXPRESSION;

                        Element parsedSubFormula = null;
                        if (prevTok == null || !prevTok.mCode.Equals(TokenCode.OPEN_PAREN))
                        {
                            parsedSubFormula = parseTokenizedExpression(subFormula, true);
                        }

                        // this expression is to be added directly to the token stream
                        tok.mParsedExpression = parsedSubFormula;
                    }
                }

                ++tokenCtr;
            }

            if (parenDepth > 0)
            {
                throw new System.ArgumentException("mismatched parentheses found in formula");
            }
        }

        private void  parseSeparators(IList<Token> pTokenizedExpression, bool pAllowArgumentLists)
        {
            Token tok      = null;
            int parenDepth = 0;
            bool hasSep    = false;

            foreach (Token token in pTokenizedExpression)
            {
                TokenCode tokenCode = token.mCode;

                if (tokenCode.Equals(TokenCode.OPEN_PAREN))
                {
                    ++parenDepth;
                }
                else if (tokenCode.Equals(TokenCode.CLOSE_PAREN))
                {
                    --parenDepth;
                    if (parenDepth < 0)
                    {
                        throw new System.ArgumentException("mismatched parentheses");
                    }
                }
                else if (parenDepth == 0 && tokenCode.Equals(TokenCode.SEP))
                {
                    if (!pAllowArgumentLists)
                    {
                        throw new System.ArgumentException("argument list not allowed in this context");
                    }

                    hasSep = true;
                    break;
                }
            }

            if (hasSep)
            {
                IList<Token> subExpression = new List<Token>();
                Token newToken      = new Token(this);
                newToken.mCode      = TokenCode.EXPRESSION_PAIR;
                Element exp1        = null;
                Element exp2        = null;

                parenDepth = 0;

                for (int iToken = 0; iToken < pTokenizedExpression.Count;  iToken++)
                {
                    tok = pTokenizedExpression[iToken];
                    TokenCode tokenCode = tok.mCode;
                    if (tokenCode.Equals(TokenCode.OPEN_PAREN))
                    {
                        ++parenDepth;
                    }
                    else if (tokenCode.Equals(TokenCode.CLOSE_PAREN))
                    {
                        --parenDepth;
                        if (parenDepth < 0)
                        {
                            throw new System.ArgumentException("mismatched parentheses");
                        }
                    }
                    if (!tokenCode.Equals(TokenCode.SEP))
                    {
                        subExpression.Add(tok);
                    }
                    else
                    {
                        if (++iToken >= pTokenizedExpression.Count)
                        {
                            throw new System.ArgumentException("invalid argument list; missing last argument");
                        }
                        else if (parenDepth == 0)
                        {
                            // handle first argument
                            if (subExpression.Count == 0)
                            {
                                throw new System.ArgumentException("invalid argument list; empty argument");
                            }
                            Element parsedSubExpression = parseTokenizedExpression(subExpression, false);
                            subExpression.Clear();
                            if (null != exp1)
                            {
                                throw new System.ArgumentException("more than two arguments are not allowed");
                            }
                            exp1 = parsedSubExpression;
                        }
                    }

                    if (++iToken >= pTokenizedExpression.Count)
                    {
                        // handle second argument
                        if (subExpression.Count == 0)
                        {
                            throw new System.ArgumentException("invalid argument list; empty argument");
                        }
                        Element parsedSubExpression = parseTokenizedExpression(subExpression, false);
                        subExpression.Clear();
                        exp2 = parsedSubExpression;
                    }
                    //UPGRADE_ISSUE: Method 'java.util.ListIterator.remove' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javautilListIteratorremove'"
                    //J iter.remove();
                    pTokenizedExpression.RemoveAt(iToken--);    // decrement to offset loop increment
                }

                newToken.mParsedExpression = exp1;
                newToken.mSecondParsedExpression = exp2;

                if (parenDepth != 0)
                {
                    throw new System.ArgumentException("mismatched number of parentheses");
                }

                //UPGRADE_ISSUE: Method 'java.util.ListIterator.add' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javautilListIteratoradd_javalangObject'"
                //J iter.add(newToken);
                pTokenizedExpression.Add(newToken);
            }
        }

        private void  parseFunctionCalls(IList<Token> pTokenizedExpression)
        {
            Token tok = null;

            // parse parentheses first, since they have the highest level of precedence
            for (int iToken = 0; iToken < pTokenizedExpression.Count;  iToken++)
            {
                tok = pTokenizedExpression[iToken];

                if (tok.mCode == TokenCode.SYMBOL)
                {
                    System.String symbolName = tok.mSymbolName;
                    ElementCode elementCodeFunction = parseFunctionName(symbolName);

                    // check to see if this is a function call
                    if ((iToken+1) < pTokenizedExpression.Count)
                    {
                        // check to see if next token is an expression token
                        Token nextTok = pTokenizedExpression[++iToken];

                        if (nextTok.mCode.Equals(TokenCode.EXPRESSION) || nextTok.mCode.Equals(TokenCode.EXPRESSION_PAIR))
                        {
                            if (null == elementCodeFunction)
                            {
                                throw new System.ArgumentException("unknown symbol used as function name: " + symbolName);
                            }

                            Element functionCallElement = new Element(elementCodeFunction);
                            int numArgs = elementCodeFunction.mNumFunctionArgs;

                            if (numArgs == 0)
                            {
                                if (!nextTok.mCode.Equals(TokenCode.EXPRESSION))
                                {
                                    throw new System.ArgumentException("expected an expression token, instead found token: " + nextTok.mCode);
                                }

                                functionCallElement.mFirstOperand = nextTok.mParsedExpression;

                                if (functionCallElement.mFirstOperand != null)
                                {
                                    throw new System.ArgumentException("function does not allow any arguments: " + elementCodeFunction);
                                }
                            }
                            else if (numArgs == 1)
                            {
                                if (!nextTok.mCode.Equals(TokenCode.EXPRESSION))
                                {
                                    throw new System.ArgumentException("expected an expression token, instead found token: " + nextTok.mCode);
                                }

                                functionCallElement.mFirstOperand = nextTok.mParsedExpression;

                                if (functionCallElement.mFirstOperand.mCode.Equals(ElementCode.PAIR))
                                {
                                    throw new System.ArgumentException("two arguments for single-argument function call: " + symbolName);
                                }
                            }
                            else if (numArgs == 2)
                            {
                                if (!nextTok.mCode.Equals(TokenCode.EXPRESSION))
                                {
                                    throw new System.ArgumentException("expected an expression token, instead found token: " + nextTok.mCode);
                                }

                                Element pairElement = nextTok.mParsedExpression;

                                if (!pairElement.mCode.Equals(ElementCode.PAIR))
                                {
                                    throw new System.ArgumentException("expected an argument pair; instead found token type:  " + pairElement.mCode);
                                }

                                functionCallElement.mFirstOperand = pairElement.mFirstOperand;
                                functionCallElement.mSecondOperand = pairElement.mSecondOperand;
                                nextTok.mCode = TokenCode.EXPRESSION;
                                nextTok.mSecondParsedExpression = null;
                            }
                            else
                            {
                                throw new System.SystemException("illegal number of function arguments");
                            }

                            nextTok.mParsedExpression = functionCallElement;
/*J
                            //UPGRADE_ISSUE: Method 'java.util.ListIterator.previous' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javautilListIteratorprevious'"
                            iter.previous();
                            //UPGRADE_ISSUE: Method 'java.util.ListIterator.previous' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javautilListIteratorprevious'"
                            iter.previous();
                            //UPGRADE_ISSUE: Method 'java.util.ListIterator.remove' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javautilListIteratorremove'"
                            iter.remove(); // remove the previous symbol token, since it is not needed anymore

                            System.Object generatedAux = iter.Current;
*/
                            pTokenizedExpression.RemoveAt(--iToken);
                        }
                        else
                        {
                            if (null != elementCodeFunction)
                            {
                                throw new System.ArgumentException("reserved function name used as symbol: " + symbolName);
                            }
                        }
                    }
                    else
                    {
                        if (null != elementCodeFunction)
                        {
                            throw new System.ArgumentException("reserved function name used as symbol: " + symbolName);
                        }
                    }
                }
                else
                {
                    if (tok.mCode.Equals(TokenCode.EXPRESSION))
                    {
                        if (tok.mParsedExpression.mCode.Equals(ElementCode.PAIR))
                        {
                            throw new System.ArgumentException("argument pair not allowed in this context");
                        }
                    }
//                else if(tok.mCode.equals(TokenCode.EXPRESSION_LIST))
//                {
//                    throw new IllegalArgumentException("argument pair not allowed in this context");
//                }
// do nothing
                }
            }
        }

        // From a list of tokens, parse all unary operator expressions (of the specified type) that occur.
        // The pMask variable is a logical OR of token code values of unary operators to look for.

        private void  parseUnaryOperator(Hashtable pTokenCodeMap, IList<Token> pTokenizedExpression)
        {
            // parse for unary operators
            Token lastTok = null;
            Token token = null;

            for (int iToken = 0; iToken < pTokenizedExpression.Count;  iToken++)
            {
                lastTok = token;

                token = pTokenizedExpression[iToken];
                TokenCode tokenCode = token.mCode;

                if (tokenCode.Equals(TokenCode.EXPRESSION))
                {
                    continue;
                }

/*J
                //UPGRADE_TODO: Method 'java.util.HashMap.keySet' was converted to 'SupportClass.HashSetSupport' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMapkeySet'"
                SupportClass.SetSupport tokenCodeSet = new SupportClass.HashSetSupport(pTokenCodeMap.Keys);

                //J if (tokenCodeSet.Contains(tokenCode))
*/

                if (pTokenCodeMap.ContainsKey(tokenCode))
                {
                    // check previous token to make sure this operator is not
                    // being used as a binary operator (e.g., negation)
                    if (lastTok == null || (lastTok.mCode != TokenCode.SYMBOL && lastTok.mCode != TokenCode.EXPRESSION && lastTok.mCode != TokenCode.NUMBER))
                    {
                        // must be using minus as a unary operator

                        if (++iToken >= pTokenizedExpression.Count)
                        {
                            throw new System.ArgumentException("last token in the list is a minus, this is not allowed");
                        }

                        Token nextTok = pTokenizedExpression[iToken];

                        Element operand = convertTokenToElement(nextTok);

                        Element opElement = null;
                        //UPGRADE_TODO: Method 'java.util.HashMap.get' was converted to 'Hashtable.Item' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMapget_javalangObject'"
                        ElementCode elementCode = (ElementCode) pTokenCodeMap[tokenCode];

                        // check for negative number
                        if (!elementCode.Equals(ElementCode.NEG) || !operand.mCode.Equals(ElementCode.NUMBER))
                        {
                            opElement = new Element(elementCode);
                            opElement.mFirstOperand = operand;
                        }
                        else
                        {
                            opElement = new Element(ElementCode.NUMBER);
                            opElement.mNumericValue = (- 1.0) * operand.mNumericValue;
                        }


/*J
                        //UPGRADE_ISSUE: Method 'java.util.ListIterator.previous' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javautilListIteratorprevious'"
                        iter.previous();
                        //UPGRADE_ISSUE: Method 'java.util.ListIterator.remove' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javautilListIteratorremove'"
                        iter.remove();
*/
                        pTokenizedExpression.RemoveAt(iToken--);    // decrement to offset loop increment

                        token.mCode = TokenCode.EXPRESSION;
                        token.mParsedExpression = opElement;
                    }
                }
            }
        }

        // From a list of tokens, parse all binary operator expressions (of the specified type) that occur.
        // The pMask variable is a logical OR of token code values of binary operators to look for.

        private void  parseBinaryOperator(Hashtable pTokenCodeMap, IList<Token> pTokenizedExpression)
        {
            Token lastTok = null;
            Token token = null;

            for (int iToken = 0; iToken < pTokenizedExpression.Count; iToken++)
            //J while (iter.MoveNext())
            {
                lastTok = token;

                token = pTokenizedExpression[iToken];
                TokenCode tokenCode = token.mCode;

                if (tokenCode.Equals(TokenCode.EXPRESSION))
                {
                    continue;
                }

/*J
                //UPGRADE_TODO: Method 'java.util.HashMap.keySet' was converted to 'SupportClass.HashSetSupport' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMapkeySet'"
                SupportClass.SetSupport tokenCodeSet = new SupportClass.HashSetSupport(pTokenCodeMap.Keys);

                if (tokenCodeSet.Contains(tokenCode))
*/
                if (pTokenCodeMap.ContainsKey(tokenCode))
                {
                    if (lastTok == null)
                    {
                        throw new System.ArgumentException("encountered binary operator with no first operand found");
                    }

                    //J if (!iter.hasNext())
                    if ((iToken+1) >= pTokenizedExpression.Count)
                    {
                        throw new System.ArgumentException("encountered binary operator with no second operand found");
                    }

                    Token nextTok = pTokenizedExpression[++iToken];
                    Element op1 = convertTokenToElement(lastTok);
                    Element op2 = convertTokenToElement(nextTok);
                    //UPGRADE_TODO: Method 'java.util.HashMap.get' was converted to 'Hashtable.Item' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMapget_javalangObject'"
                    ElementCode elementCode = (ElementCode) pTokenCodeMap[tokenCode];
                    Element product = new Element(elementCode);
                    product.mFirstOperand = op1;
                    product.mSecondOperand = op2;
/*J
                    //UPGRADE_ISSUE: Method 'java.util.ListIterator.remove' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javautilListIteratorremove'"
                    iter.remove();
                    //UPGRADE_ISSUE: Method 'java.util.ListIterator.previous' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javautilListIteratorprevious'"
                    iter.previous();
                    //UPGRADE_ISSUE: Method 'java.util.ListIterator.previous' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javautilListIteratorprevious'"
                    iter.previous();
                    //UPGRADE_ISSUE: Method 'java.util.ListIterator.remove' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javautilListIteratorremove'"
                    iter.remove();
*/
                    // Remove op2
                    pTokenizedExpression.RemoveAt(iToken);
                    // Remove op1
                    iToken -= 2;
                    pTokenizedExpression.RemoveAt(iToken);

                    // Replace binary op token with expression
                    token.mCode = TokenCode.EXPRESSION;
                    token.mParsedExpression = product;
                }
            }
        }

        /*
        * IMPORTANT:  this method contains PERFORMANCE-CRITICAL code
        */
        private static double valueOfSubtreeNonSimple(Element pElement, SymbolEvaluator pSymbolEvaluator)
        {
            double valueOfFirstOperand;
            switch (pElement.mFirstOperand.mCode.mIntCode)
            {

                case ElementCode.ELEMENT_CODE_SYMBOL:
                    valueOfFirstOperand = pSymbolEvaluator.getValue(pElement.mFirstOperand.mSymbol);
                    break;

                case ElementCode.ELEMENT_CODE_NUMBER:
                    valueOfFirstOperand = pElement.mFirstOperand.mNumericValue;
                    break;

                default:
                    valueOfFirstOperand = valueOfSubtreeNonSimple(pElement.mFirstOperand, pSymbolEvaluator);
                    break;

            }

            if (null != pElement.mSecondOperand)
            {
                double valueOfSecondOperand;
                switch (pElement.mSecondOperand.mCode.mIntCode)
                {

                    case ElementCode.ELEMENT_CODE_SYMBOL:
                        valueOfSecondOperand = pSymbolEvaluator.getValue(pElement.mSecondOperand.mSymbol);
                        break;

                    case ElementCode.ELEMENT_CODE_NUMBER:
                        valueOfSecondOperand = pElement.mSecondOperand.mNumericValue;
                        break;

                    default:
                        valueOfSecondOperand = valueOfSubtreeNonSimple(pElement.mSecondOperand, pSymbolEvaluator);
                        break;

                }

                switch (pElement.mCode.mIntCode)
                {

                    case ElementCode.ELEMENT_CODE_MULT:
                        return (valueOfFirstOperand * valueOfSecondOperand);


                    case ElementCode.ELEMENT_CODE_ADD:
                        return (valueOfFirstOperand + valueOfSecondOperand);


                    case ElementCode.ELEMENT_CODE_DIV:
                        return (valueOfFirstOperand / valueOfSecondOperand);


                    case ElementCode.ELEMENT_CODE_SUBT:
                        return (valueOfFirstOperand - valueOfSecondOperand);


                    case ElementCode.ELEMENT_CODE_POW:
                        return (System.Math.Pow(valueOfFirstOperand, valueOfSecondOperand));


                    case ElementCode.ELEMENT_CODE_MOD:
                        return (valueOfFirstOperand % valueOfSecondOperand);


                    case ElementCode.ELEMENT_CODE_MIN:
                        return System.Math.Min(valueOfFirstOperand, valueOfSecondOperand);


                    case ElementCode.ELEMENT_CODE_MAX:
                        return System.Math.Max(valueOfFirstOperand, valueOfSecondOperand);


                    default:
                        throw new System.SystemException("unknown function code: " + pElement.mCode);

                }
            }
            else
            {
                switch (pElement.mCode.mIntCode)
                {

                    case ElementCode.ELEMENT_CODE_NEG:
                        return (- valueOfFirstOperand);


                    case ElementCode.ELEMENT_CODE_EXP:
                        return (System.Math.Exp(valueOfFirstOperand));


                    case ElementCode.ELEMENT_CODE_LN:
                        return (System.Math.Log(valueOfFirstOperand));


                    case ElementCode.ELEMENT_CODE_SIN:
                        return (System.Math.Sin(valueOfFirstOperand));


                    case ElementCode.ELEMENT_CODE_COS:
                        return (System.Math.Cos(valueOfFirstOperand));


                    case ElementCode.ELEMENT_CODE_TAN:
                        return (System.Math.Tan(valueOfFirstOperand));


                    case ElementCode.ELEMENT_CODE_ASIN:
                        return (System.Math.Asin(valueOfFirstOperand));


                    case ElementCode.ELEMENT_CODE_ACOS:
                        return (System.Math.Acos(valueOfFirstOperand));


                    case ElementCode.ELEMENT_CODE_ATAN:
                        return (System.Math.Atan(valueOfFirstOperand));


                    case ElementCode.ELEMENT_CODE_ABS:
                        return (System.Math.Abs(valueOfFirstOperand));


                    case ElementCode.ELEMENT_CODE_FLOOR:
                        return (System.Math.Floor(valueOfFirstOperand));


                    case ElementCode.ELEMENT_CODE_CEIL:
                        return (System.Math.Ceiling(valueOfFirstOperand));


                    case ElementCode.ELEMENT_CODE_SQRT:
                        return (System.Math.Sqrt(valueOfFirstOperand));


                    case ElementCode.ELEMENT_CODE_THETA:
                        return (MathFunctions.thetaFunction(valueOfFirstOperand));


                    default:
                        throw new System.SystemException("unknown function code: " + pElement.mCode);

                }
            }
        }

        /*
        * IMPORTANT:  this method contains PERFORMANCE-CRITICAL code.
        */
        private static double valueOfSubtree(Element pElement, SymbolEvaluator pSymbolEvaluator)
        {
            int elementCodeInt = pElement.mCode.mIntCode;

            switch (elementCodeInt)
            {

                // first handle all the cases that have no operands:
                case ElementCode.ELEMENT_CODE_SYMBOL:
                    Symbol symbol = pElement.mSymbol;
                    return (pSymbolEvaluator.getValue(symbol));


                case ElementCode.ELEMENT_CODE_NUMBER:
                    return (pElement.mNumericValue);


                default:
                    return (valueOfSubtreeNonSimple(pElement, pSymbolEvaluator));

            }
        }

        private Element parseTokenizedExpression(IList<Token> pFormula, bool pAllowArgumentLists)
        {
            // parse for parentheses (sub-expressions)
            parseParentheses(pFormula);

            parseSeparators(pFormula, pAllowArgumentLists);

            // parse for built-in function calls (exp, ln, sin, cos, tan, etc.)
            parseFunctionCalls(pFormula);

            // parse for unary operators
            Hashtable unaryMap = new Hashtable();
            unaryMap[TokenCode.MINUS] = ElementCode.NEG;
            parseUnaryOperator(unaryMap, pFormula);

            // parse for pow
            Hashtable binaryMap = new Hashtable();
            binaryMap[TokenCode.POW] = ElementCode.POW;
            parseBinaryOperator(binaryMap, pFormula);

            // parse for mult and div
            binaryMap.Clear();
            binaryMap[TokenCode.MULT] = ElementCode.MULT;
            binaryMap[TokenCode.DIV] = ElementCode.DIV;
            binaryMap[TokenCode.MOD] = ElementCode.MOD;
            parseBinaryOperator(binaryMap, pFormula);

            // parse for add and subt
            binaryMap.Clear();
            binaryMap[TokenCode.PLUS] = ElementCode.ADD;
            binaryMap[TokenCode.MINUS] = ElementCode.SUBT;
            parseBinaryOperator(binaryMap, pFormula);

            IEnumerator iter = pFormula.GetEnumerator();

            if (!iter.MoveNext())
            {
                throw new System.ArgumentException("no elements found in the parse tree for this expression");
            }

            Token finalToken = (Token) iter.Current;

            if (iter.MoveNext())
            {
                throw new System.ArgumentException("found more than one element at the root of the parsed formula tree");
            }

            return (convertTokenToElement(finalToken));
        }

        /*========================================*
        * protected methods
        *========================================*/
        /// <summary> Returns an {@link Expression.Element} which is the root of a tree of
        /// [code]Element[/code] objects representing the parse tree for
        /// a mathematical expression defined by the string [code]pExpressionString[/code].
        /// The parse tree is not stored in the [code]Expression[/code] object.
        ///
        /// </summary>
        /// <param name="pExpressionString">the string definition of the mathematical
        /// expression to be parsed
        ///
        /// </param>
        /// <returns> an {@link Expression.Element} which is the root of a tree of
        /// [code]Element[/code] objects representing the parse tree for
        /// a mathematical expression defined by the string [code]pExpressionString[/code].
        /// </returns>
        internal virtual Element parseExpression(System.String pExpressionString)
        {
            IList<Token> tokenizedExpression = tokenizeExpression(pExpressionString);
            return (parseTokenizedExpression(tokenizedExpression, false));
        }

        /*========================================*
        * public methods
        *========================================*/

        /// <summary> Returns true of the string token passed as [code]pToken[/code]
        /// is a valid &quot;symbol&quot;, meaning that it does not contain
        /// any of the operators [code]*[/code], [code]-[/code], [code]/[/code],
        /// [code]+[/code], [code]^[/code], or any parentheses.  In addition, the
        /// following characters may not appear in symbols because they are reserved for
        /// future use in mathematical expressions:  [code]!@#$%[]|&amp;&gt;&lt;{}[/code].
        ///
        /// [p /]
        /// Examples of valid symbols would be:
        /// [blockquote]
        /// [pre]
        /// X1
        /// SY2C
        /// [/pre]
        /// [/blockquote]
        /// Examples of [em]invalid[/em] symbols would be:
        /// [blockquote]
        /// [pre]
        /// X1-2
        /// X(1)
        /// -X
        /// (X)
        /// X*B
        /// X+C
        /// X B
        /// exp
        /// 1.7
        /// [/pre]
        /// [/blockquote]
        ///
        /// </summary>
        /// <param name="pToken">the string token to be checked for validity as a symbol
        ///
        /// </param>
        /// <returns> true of the string token passed as [code]pToken[/code]
        /// is a valid &quot;symbol&quot;.
        /// </returns>
        static public bool isValidSymbol(System.String pToken)
        {
            bool isValidSymbol = false;
            try
            {
                Expression testExp = new Expression(pToken);
                Element elem = testExp.RootElement;
                if (ElementCode.SYMBOL == elem.mCode)
                {
                    // check for parentheses and whitespace, which are not allowed in symbols
                    bool returnDelims = true;
                    SupportClass.Tokenizer tok = new SupportClass.Tokenizer(pToken, " ()", returnDelims);
                    int numTokens = tok.Count;
                    if (numTokens == 1)
                    {
                        isValidSymbol = true;
                    }
                }
            }
            catch (System.Exception)
            {
                isValidSymbol = false;
            }
            return (isValidSymbol);
        }

        /// <summary> Returns a human-comprehensible representation of this
        /// mathematical expression.
        ///
        /// </summary>
        /// <returns> a human-comprehensible representation of this
        /// mathematical expression.
        /// </returns>
        public override System.String ToString()
        {
            try
            {
                return (toString(null));
            }
            catch (DataNotFoundException e)
            {
                //UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Throwable.getMessage' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
                throw new System.SystemException(e.Message);
            }
        }

        public virtual System.String toString(Expression.SymbolPrinter pSymbolPrinter)
        {
            System.String retStr;
            Element rootElement = RootElement;
            if (null != rootElement)
            {
                retStr = rootElement.toString(pSymbolPrinter);
            }
            else
            {
                retStr = "null";
            }
            return (retStr);
        }

        /// <summary> Returns true if the argument is a valid function name, or
        /// false otherwise.
        /// </summary>
        public static bool isFunctionName(System.String pName)
        {
            ElementCode function = ElementCode.getFunction(pName);
            return (null != function && function.isFunction());
        }

        /// <summary> Parses the mathematical expression defined by the string
        /// [code]pExpressionString[/code] and stores the parse tree within
        /// this [code]Expression[/code] object.  This method can be
        /// called even if the [code]Expression[/code] object was
        /// initialized with a different expression string, in which
        /// case the string specified by [code]pExpressionString[/code]
        /// is parsed and the parse tree replaces the parse tree created
        /// when the object was constructed.
        ///
        /// </summary>
        /// <param name="pExpressionString">the string definition of the mathematical
        /// expression to be parsed
        ///
        /// </param>
        public virtual void  setExpression(System.String pExpressionString)
        {
            RootElement = parseExpression(pExpressionString);
        }


        public virtual double computeValue(Hashtable pSymbolsMap)
        {
            SymbolEvaluator symbolEvaluator = getSymbolEvaluator(pSymbolsMap);
            return (computeValue(symbolEvaluator));
        }

        public interface IVisitor
        {
            void  visit(Symbol pSymbol);
        }

        private static void  visit(Expression.IVisitor pVisitor, Element pElement)
        {
            if (null != pElement.mSymbol)
            {
                pVisitor.visit(pElement.mSymbol);
            }
            if (null != pElement.mFirstOperand)
            {
                visit(pVisitor, pElement.mFirstOperand);
            }
            if (null != pElement.mSecondOperand)
            {
                visit(pVisitor, pElement.mSecondOperand);
            }
        }

        public virtual void  visit(Expression.IVisitor pVisitor)
        {
            Element rootElement = RootElement;
            if (null == rootElement)
            {
                throw new System.SystemException("attempted to compute value of a math expression object that has no expression defined");
            }
            visit(pVisitor, rootElement);
        }

        /// <summary> Return the computed value of the expression (must have been defined
        /// already in the constructor, or in a call to {@link #setExpression(String)}),
        /// using the symbol values defined in the map [code]ISymbolValueMap[/code].
        /// </summary>
        public virtual double computeValue(SymbolEvaluator pSymbolEvaluator)
        {
            Element rootElement = RootElement;
            if (null == rootElement)
            {
                throw new System.SystemException("attempted to compute value of a math expression object that has no expression defined");
            }
            try
            {
                valueOfSubtree(rootElement, pSymbolEvaluator);
            }
            //UPGRADE_TODO: Class 'java.lang.StackOverflowError' was converted to 'System.StackOverflowException' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javalangStackOverflowError'"
            catch (System.StackOverflowException)
            {
                throw new System.ArgumentException("circular expression encountered while attempting to parse expression: " + ToString());
            }


            return (valueOfSubtree(rootElement, pSymbolEvaluator));
        }

        public virtual System.Object Clone()
        {
            Expression newExpression = new Expression();
            if (mRootElement != null)
            {
                newExpression.mRootElement = (Element) mRootElement.Clone();
            }
            else
            {
                newExpression.mRootElement = null;
            }
            return (newExpression);
        }

        private Element computePartialDerivative(Element pElement, Symbol pSymbol, SymbolEvaluator pSymbolEvaluator)
        {
            Element retElement = null;
            ElementCode code = pElement.mCode;
            int intCode = code.mIntCode;

            Element firstOperand = pElement.mFirstOperand;
            Element firstOperandDerivExpression = null;
            bool firstOperandDerivZero = false;
            bool firstOperandDerivUnity = false;
            if (null != firstOperand)
            {
                firstOperandDerivExpression = computePartialDerivative(firstOperand, pSymbol, pSymbolEvaluator);
                if (firstOperandDerivExpression.mCode.Equals(ElementCode.NUMBER))
                {
                    double derivValue = firstOperandDerivExpression.mNumericValue;
                    if (derivValue == 0.0)
                    {
                        firstOperandDerivZero = true;
                    }
                    else if (derivValue == 1.0)
                    {
                        firstOperandDerivUnity = true;
                    }
                }
            }

            Element secondOperand = pElement.mSecondOperand;
            Element secondOperandDerivExpression = null;
            bool secondOperandDerivZero = false;
            bool secondOperandDerivUnity = false;
            if (null != secondOperand)
            {
                secondOperandDerivExpression = computePartialDerivative(secondOperand, pSymbol, pSymbolEvaluator);
                if (secondOperandDerivExpression.mCode.Equals(ElementCode.NUMBER))
                {
                    double derivValue = secondOperandDerivExpression.mNumericValue;
                    if (derivValue == 0.0)
                    {
                        secondOperandDerivZero = true;
                    }
                    else if (derivValue == 1.0)
                    {
                        secondOperandDerivUnity = true;
                    }
                }
            }

            switch (intCode)
            {

                case ElementCode.ELEMENT_CODE_NUMBER:
                    // partial derivative of a simple number with respect to a symbol is always zero
                    retElement = new Element(ElementCode.NUMBER);
                    retElement.mNumericValue = 0.0;
                    break;


                case ElementCode.ELEMENT_CODE_SYMBOL:
                    Symbol symbol = pElement.mSymbol;
                    if (symbol.Name.Equals(pSymbol.Name))
                    {
                        // partial derivative of a symbol with respect to itself, is unity
                        retElement = new Element(ElementCode.NUMBER);
                        retElement.mNumericValue = 1.0;
                    }
                    else
                    {
                        Expression symbolExpression = pSymbolEvaluator.getExpressionValue(symbol);
                        if (null != symbolExpression)
                        {
                            // in this case, the symbol within the expression being differentiated is itself an expression
                            Expression derivExpression = symbolExpression.computePartialDerivative(pSymbol, pSymbolEvaluator);
                            retElement = derivExpression.mRootElement;
                        }
                        else
                        {
                            // in this case, the symbol within the expression being differentiated is a simple number
                            retElement = new Element(ElementCode.NUMBER);
                            retElement.mNumericValue = 0.0;
                        }
                    }
                    break;


                case ElementCode.ELEMENT_CODE_NONE:
                    throw new System.ArgumentException("element code NONE should never occur in a valid expression tree");


                case ElementCode.ELEMENT_CODE_MOD:
                    throw new System.ArgumentException("unable to compute the derivative of the modulo division operator");


                case ElementCode.ELEMENT_CODE_ABS:
                    throw new System.ArgumentException("unable to compute the derivative of the abs() function");


                case ElementCode.ELEMENT_CODE_FLOOR:
                    throw new System.ArgumentException("unable to compute the derivative of the floor() function");


                case ElementCode.ELEMENT_CODE_CEIL:
                    throw new System.ArgumentException("unable to compute the derivative of the ceil() function");


                case ElementCode.ELEMENT_CODE_THETA:
                    throw new System.ArgumentException("unable to compute the derivative of the theta() function");


                case ElementCode.ELEMENT_CODE_MIN:
                    throw new System.ArgumentException("unable to compute the derivative of the min() function");


                case ElementCode.ELEMENT_CODE_MAX:
                    throw new System.ArgumentException("unable to compute the derivative of the max() function");


                case ElementCode.ELEMENT_CODE_ACOS:
                    if (!firstOperandDerivZero)
                    {
                        retElement = new Element(ElementCode.NEG);

                        Element ratio = new Element(ElementCode.DIV);
                        retElement.mFirstOperand = ratio;

                        ratio.mFirstOperand = firstOperandDerivExpression;

                        Element sqrtOneMinusXSquared = new Element(ElementCode.SQRT);
                        ratio.mSecondOperand = sqrtOneMinusXSquared;

                        Element oneMinusXSquared = new Element(ElementCode.SUBT);
                        sqrtOneMinusXSquared.mFirstOperand = oneMinusXSquared;

                        oneMinusXSquared.mFirstOperand = Element.ONE;

                        Element xSquared = new Element(ElementCode.POW);
                        oneMinusXSquared.mSecondOperand = xSquared;

                        xSquared.mFirstOperand = firstOperand;
                        xSquared.mSecondOperand = Element.TWO;
                    }
                    else
                    {
                        retElement = firstOperandDerivExpression;
                    }
                    break;


                case ElementCode.ELEMENT_CODE_ASIN:
                    if (!firstOperandDerivZero)
                    {
                        retElement = new Element(ElementCode.DIV);

                        retElement.mFirstOperand = firstOperandDerivExpression;

                        Element sqrtOneMinusXSquared = new Element(ElementCode.SQRT);
                        retElement.mSecondOperand = sqrtOneMinusXSquared;

                        Element oneMinusXSquared = new Element(ElementCode.SUBT);
                        sqrtOneMinusXSquared.mFirstOperand = oneMinusXSquared;

                        oneMinusXSquared.mFirstOperand = Element.ONE;

                        Element xSquared = new Element(ElementCode.POW);
                        oneMinusXSquared.mSecondOperand = xSquared;

                        xSquared.mFirstOperand = firstOperand;
                        xSquared.mSecondOperand = Element.TWO;
                    }
                    else
                    {
                        retElement = firstOperandDerivExpression;
                    }
                    break;


                case ElementCode.ELEMENT_CODE_ATAN:
                    if (!firstOperandDerivZero)
                    {
                        retElement = new Element(ElementCode.DIV);

                        retElement.mFirstOperand = firstOperandDerivExpression;

                        Element onePlusXSquared = new Element(ElementCode.ADD);
                        retElement.mSecondOperand = onePlusXSquared;

                        onePlusXSquared.mFirstOperand = Element.ONE;

                        Element xSquared = new Element(ElementCode.POW);
                        xSquared.mFirstOperand = firstOperand;
                        xSquared.mSecondOperand = Element.TWO;

                        onePlusXSquared.mSecondOperand = xSquared;

                        retElement.mSecondOperand = onePlusXSquared;
                    }
                    else
                    {
                        retElement = firstOperandDerivExpression;
                    }
                    break;


                case ElementCode.ELEMENT_CODE_SQRT:
                    if (!firstOperandDerivZero)
                    {
                        retElement = new Element(ElementCode.DIV);

                        retElement.mFirstOperand = firstOperandDerivExpression;
                        Element twoSqrt = new Element(ElementCode.MULT);
                        twoSqrt.mFirstOperand = Element.TWO;
                        twoSqrt.mSecondOperand = pElement;
                        retElement.mSecondOperand = twoSqrt;
                    }
                    else
                    {
                        retElement = firstOperandDerivExpression;
                    }

                    break;


                case ElementCode.ELEMENT_CODE_LN:
                    if (!firstOperandDerivZero)
                    {
                        retElement = new Element(ElementCode.DIV);
                        retElement.mFirstOperand = firstOperandDerivExpression;
                        retElement.mSecondOperand = firstOperand;
                    }
                    else
                    {
                        retElement = firstOperandDerivExpression;
                    }
                    break;


                case ElementCode.ELEMENT_CODE_POW:
                    if (!firstOperandDerivZero)
                    {
                        if (!secondOperandDerivZero)
                        {
                            retElement = new Element(ElementCode.MULT);
                            Element sum = new Element(ElementCode.ADD);
                            retElement.mFirstOperand = sum;

                            Element xlogx = new Element(ElementCode.MULT);
                            xlogx.mFirstOperand = firstOperand;
                            Element logx = new Element(ElementCode.LN);
                            logx.mFirstOperand = firstOperand;
                            xlogx.mSecondOperand = logx;

                            if (!secondOperandDerivUnity)
                            {
                                Element sumFirstTerm = new Element(ElementCode.MULT);
                                sumFirstTerm.mFirstOperand = secondOperandDerivExpression;
                                sumFirstTerm.mSecondOperand = xlogx;
                                sum.mFirstOperand = sumFirstTerm;
                            }
                            else
                            {
                                sum.mFirstOperand = xlogx;
                            }

                            if (!firstOperandDerivUnity)
                            {
                                Element sumSecondTerm = new Element(ElementCode.MULT);
                                sum.mSecondOperand = sumSecondTerm;
                                sumSecondTerm.mFirstOperand = firstOperandDerivExpression;
                                sumSecondTerm.mSecondOperand = secondOperand;
                            }
                            else
                            {
                                sum.mSecondOperand = secondOperand;
                            }

                            Element yminus1 = null;

                            if (!secondOperand.mCode.Equals(ElementCode.NUMBER))
                            {
                                yminus1 = new Element(ElementCode.SUBT);
                                yminus1.mFirstOperand = secondOperand;
                                yminus1.mSecondOperand = Element.ONE;
                            }
                            else
                            {
                                double newVal = secondOperand.mNumericValue - 1.0;
                                if (newVal != 1.0)
                                {
                                    yminus1 = new Element(ElementCode.NUMBER);
                                    yminus1.mNumericValue = newVal;
                                }
                                else
                                {
                                    // do nothing
                                }
                            }

                            Element xtoyminus1 = null;

                            if (null != yminus1)
                            {
                                xtoyminus1 = new Element(ElementCode.POW);
                                xtoyminus1.mFirstOperand = firstOperand;
                                xtoyminus1.mSecondOperand = yminus1;
                            }
                            else
                            {
                                xtoyminus1 = firstOperand;
                            }

                            retElement.mSecondOperand = xtoyminus1;
                        }
                        else
                        {
                            retElement = new Element(ElementCode.MULT);

                            if (!firstOperandDerivUnity)
                            {
                                Element xprimey = new Element(ElementCode.MULT);
                                xprimey.mFirstOperand = firstOperandDerivExpression;
                                xprimey.mSecondOperand = secondOperand;
                                retElement.mFirstOperand = xprimey;
                            }
                            else
                            {
                                retElement.mFirstOperand = secondOperand;
                            }

                            Element yminus1 = null;

                            if (!secondOperand.mCode.Equals(ElementCode.NUMBER))
                            {
                                yminus1 = new Element(ElementCode.SUBT);
                                yminus1.mFirstOperand = secondOperand;
                                yminus1.mSecondOperand = Element.ONE;
                            }
                            else
                            {
                                double newExp = secondOperand.mNumericValue - 1.0;
                                if (newExp != 1.0)
                                {
                                    yminus1 = new Element(ElementCode.NUMBER);
                                    yminus1.mNumericValue = newExp;
                                }
                                else
                                {
                                    // do nothing
                                }
                            }

                            Element xtoyminus1 = null;
                            if (null != yminus1)
                            {
                                xtoyminus1 = new Element(ElementCode.POW);
                                xtoyminus1.mFirstOperand = firstOperand;
                                xtoyminus1.mSecondOperand = yminus1;
                            }
                            else
                            {
                                xtoyminus1 = firstOperand;
                            }

                            retElement.mSecondOperand = xtoyminus1;
                        }
                    }
                    else
                    {
                        if (!secondOperandDerivZero)
                        {
                            retElement = new Element(ElementCode.MULT);

                            Element logx = new Element(ElementCode.LN);
                            logx.mFirstOperand = firstOperand;

                            if (!secondOperandDerivUnity)
                            {
                                Element yprimelogx = new Element(ElementCode.MULT);
                                yprimelogx.mFirstOperand = secondOperandDerivExpression;
                                yprimelogx.mSecondOperand = logx;
                                retElement.mFirstOperand = yprimelogx;
                            }
                            else
                            {
                                retElement.mFirstOperand = logx;
                            }

                            retElement.mSecondOperand = pElement;
                        }
                        else
                        {
                            retElement = firstOperandDerivExpression;
                        }
                    }
                    break;


                case ElementCode.ELEMENT_CODE_EXP:
                    if (!firstOperandDerivZero)
                    {
                        if (!firstOperandDerivUnity)
                        {
                            retElement = new Element(ElementCode.MULT);
                            retElement.mFirstOperand = firstOperandDerivExpression;
                            retElement.mSecondOperand = pElement;
                        }
                        else
                        {
                            retElement = pElement;
                        }
                    }
                    else
                    {
                        retElement = firstOperandDerivExpression;
                    }
                    break;


                case ElementCode.ELEMENT_CODE_NEG:
                    if (!firstOperandDerivZero)
                    {
                        if (!firstOperandDerivUnity)
                        {
                            retElement = new Element(ElementCode.NEG);
                            retElement.mFirstOperand = firstOperandDerivExpression;
                        }
                        else
                        {
                            retElement = new Element(ElementCode.NUMBER);
                            retElement.mNumericValue = - 1.0;
                        }
                    }
                    else
                    {
                        retElement = firstOperandDerivExpression;
                    }
                    break;


                case ElementCode.ELEMENT_CODE_TAN:
                    if (!firstOperandDerivZero)
                    {
                        retElement = new Element(ElementCode.DIV);
                        retElement.mFirstOperand = firstOperandDerivExpression;
                        Element cosSq = new Element(ElementCode.POW);
                        Element cos = new Element(ElementCode.COS);
                        cos.mFirstOperand = firstOperand;
                        cosSq.mFirstOperand = cos;
                        cosSq.mSecondOperand = Element.TWO;
                        retElement.mSecondOperand = cosSq;
                    }
                    else
                    {
                        retElement = firstOperandDerivExpression;
                    }
                    break;


                case ElementCode.ELEMENT_CODE_COS:
                    if (!firstOperandDerivZero)
                    {
                        retElement = new Element(ElementCode.NEG);
                        if (!firstOperandDerivUnity)
                        {
                            Element prod = new Element(ElementCode.MULT);
                            retElement.mFirstOperand = prod;
                            Element sinFunc = new Element(ElementCode.SIN);
                            sinFunc.mFirstOperand = firstOperand;
                            prod.mFirstOperand = sinFunc;
                            prod.mSecondOperand = firstOperandDerivExpression;
                        }
                        else
                        {
                            Element sinFunc = new Element(ElementCode.SIN);
                            sinFunc.mFirstOperand = firstOperand;
                            retElement.mFirstOperand = sinFunc;
                        }
                    }
                    else
                    {
                        retElement = firstOperandDerivExpression;
                    }

                    break;


                case ElementCode.ELEMENT_CODE_SIN:
                    if (!firstOperandDerivZero)
                    {
                        if (!firstOperandDerivUnity)
                        {
                            retElement = new Element(ElementCode.MULT);
                            Element cosFunc = new Element(ElementCode.COS);
                            cosFunc.mFirstOperand = firstOperand;
                            retElement.mFirstOperand = cosFunc;
                            retElement.mSecondOperand = firstOperandDerivExpression;
                        }
                        else
                        {
                            retElement = new Element(ElementCode.COS);
                            retElement.mFirstOperand = firstOperand;
                        }
                    }
                    else
                    {
                        retElement = firstOperandDerivExpression;
                    }

                    break;


                case ElementCode.ELEMENT_CODE_ADD:
                    if (!firstOperandDerivZero)
                    {
                        if (!secondOperandDerivZero)
                        {
                            retElement = new Element(ElementCode.ADD);
                            retElement.mFirstOperand = firstOperandDerivExpression;
                            retElement.mSecondOperand = secondOperandDerivExpression;
                        }
                        else
                        {
                            retElement = firstOperandDerivExpression;
                        }
                    }
                    else
                    {
                        if (!secondOperandDerivZero)
                        {
                            retElement = secondOperandDerivExpression;
                        }
                        else
                        {
                            retElement = firstOperandDerivExpression;
                        }
                    }

                    break;


                case ElementCode.ELEMENT_CODE_SUBT:
                    if (!firstOperandDerivZero)
                    {
                        if (!secondOperandDerivZero)
                        {
                            retElement = new Element(ElementCode.SUBT);
                            retElement.mFirstOperand = firstOperandDerivExpression;
                            retElement.mSecondOperand = secondOperandDerivExpression;
                        }
                        else
                        {
                            retElement = firstOperandDerivExpression;
                        }
                    }
                    else
                    {
                        if (!secondOperandDerivZero)
                        {
                            retElement = new Element(ElementCode.NEG);
                            retElement.mFirstOperand = secondOperandDerivExpression;
                        }
                        else
                        {
                            retElement = firstOperandDerivExpression;
                        }
                    }

                    break;


                case ElementCode.ELEMENT_CODE_DIV:
                    if (!firstOperandDerivZero)
                    {
                        if (!secondOperandDerivZero)
                        {
                            retElement = new Element(ElementCode.SUBT);
                            Element firstTerm = new Element(ElementCode.DIV);
                            firstTerm.mFirstOperand = firstOperandDerivExpression;
                            firstTerm.mSecondOperand = secondOperand;

                            retElement.mFirstOperand = firstTerm;

                            Element secondTerm = new Element(ElementCode.DIV);

                            Element secondTermNum = null;

                            if (!secondOperandDerivUnity)
                            {
                                secondTermNum = new Element(ElementCode.MULT);
                                secondTermNum.mFirstOperand = firstOperand;
                                secondTermNum.mSecondOperand = secondOperandDerivExpression;
                            }
                            else
                            {
                                secondTermNum = firstOperand;
                            }

                            secondTerm.mFirstOperand = secondTermNum;

                            Element secondTermDenom = new Element(ElementCode.POW);
                            secondTermDenom.mFirstOperand = secondOperand;
                            secondTermDenom.mSecondOperand = Element.TWO;
                            secondTerm.mSecondOperand = secondTermDenom;

                            retElement.mSecondOperand = secondTerm;
                        }
                        else
                        {
                            retElement = new Element(ElementCode.DIV);
                            retElement.mFirstOperand = firstOperandDerivExpression;
                            retElement.mSecondOperand = secondOperand;
                        }
                    }
                    else
                    {
                        if (!secondOperandDerivZero)
                        {
                            if (!secondOperandDerivUnity)
                            {
                                retElement = new Element(ElementCode.NEG);

                                Element secondTermArg = new Element(ElementCode.DIV);
                                retElement.mFirstOperand = secondTermArg;

                                Element secondTermNum = new Element(ElementCode.MULT);
                                secondTermNum.mFirstOperand = firstOperand;
                                secondTermNum.mSecondOperand = secondOperandDerivExpression;
                                secondTermArg.mFirstOperand = secondTermNum;

                                Element secondTermDenom = new Element(ElementCode.POW);
                                secondTermDenom.mFirstOperand = secondOperand;
                                secondTermDenom.mSecondOperand = Element.TWO;
                                secondTermArg.mSecondOperand = secondTermDenom;
                            }
                            else
                            {
                                retElement = new Element(ElementCode.NEG);

                                Element secondTermArg = new Element(ElementCode.DIV);
                                retElement.mFirstOperand = secondTermArg;

                                Element secondTermNum = firstOperand;
                                secondTermArg.mFirstOperand = secondTermNum;

                                Element secondTermDenom = new Element(ElementCode.POW);
                                secondTermDenom.mFirstOperand = secondOperand;
                                secondTermDenom.mSecondOperand = Element.TWO;
                                secondTermArg.mSecondOperand = secondTermDenom;
                            }
                        }
                        else
                        {
                            retElement = secondOperandDerivExpression;
                        }
                    }
                    break;


                case ElementCode.ELEMENT_CODE_MULT:

                    if (!firstOperandDerivZero)
                    {
                        if (!secondOperandDerivZero)
                        {
                            retElement = new Element(ElementCode.ADD);

                            if (!firstOperandDerivUnity)
                            {
                                if (!secondOperandDerivUnity)
                                {
                                    Element firstTerm = new Element(ElementCode.MULT);
                                    firstTerm.mFirstOperand = firstOperandDerivExpression;
                                    firstTerm.mSecondOperand = secondOperand;
                                    Element secondTerm = new Element(ElementCode.MULT);
                                    secondTerm.mFirstOperand = firstOperand;
                                    secondTerm.mSecondOperand = secondOperandDerivExpression;
                                    retElement.mFirstOperand = firstTerm;
                                    retElement.mSecondOperand = secondTerm;
                                }
                                else
                                {
                                    Element firstTerm = new Element(ElementCode.MULT);
                                    firstTerm.mFirstOperand = firstOperandDerivExpression;
                                    firstTerm.mSecondOperand = secondOperand;
                                    Element secondTerm = firstOperand;
                                    retElement.mFirstOperand = firstTerm;
                                    retElement.mSecondOperand = secondTerm;
                                }
                            }
                            else
                            {
                                if (!secondOperandDerivUnity)
                                {
                                    Element firstTerm = secondOperand;
                                    Element secondTerm = new Element(ElementCode.MULT);
                                    secondTerm.mFirstOperand = firstOperand;
                                    secondTerm.mSecondOperand = secondOperandDerivExpression;
                                    retElement.mFirstOperand = firstTerm;
                                    retElement.mSecondOperand = secondTerm;
                                }
                                else
                                {
                                    retElement.mFirstOperand = firstOperand;
                                    retElement.mSecondOperand = secondOperand;
                                }
                            }
                        }
                        else
                        {
                            if (!firstOperandDerivUnity)
                            {
                                retElement = new Element(ElementCode.MULT);
                                retElement.mFirstOperand = firstOperandDerivExpression;
                                retElement.mSecondOperand = secondOperand;
                            }
                            else
                            {
                                retElement = secondOperand;
                            }
                        }
                    }
                    else
                    {
                        if (!secondOperandDerivZero)
                        {
                            if (!secondOperandDerivUnity)
                            {
                                retElement = new Element(ElementCode.MULT);
                                retElement.mFirstOperand = firstOperand;
                                retElement.mSecondOperand = secondOperandDerivExpression;
                            }
                            else
                            {
                                retElement = firstOperand;
                            }
                        }
                        else
                        {
                            retElement = secondOperandDerivExpression;
                        }
                    }

                    break;


                default:
                    throw new System.SystemException("unable to differentiate element code: " + pElement.mCode);

            }


            return (retElement);
        }

        public virtual Expression computePartialDerivative(Symbol pSymbol, SymbolEvaluator pSymbolEvaluator)
        {
            Expression expression = new Expression();
            expression.mRootElement = computePartialDerivative(mRootElement, pSymbol, pSymbolEvaluator);
            return (expression);
        }


        public virtual Expression computePartialDerivative(Symbol pSymbol, Hashtable pSymbolsMap)
        {
            SymbolEvaluator symbolEvaluator = getSymbolEvaluator(pSymbolsMap);
            Expression expression = new Expression();
            expression.mRootElement = computePartialDerivative(mRootElement, pSymbol, symbolEvaluator);
            return (expression);
        }

        public static Expression square(Expression A)
        {
            Expression retVal = null;
            if (A.SimpleNumber)
            {
                double value_Renamed = A.mRootElement.mNumericValue;
                retVal = new Expression(value_Renamed * value_Renamed);
            }
            else
            {
                retVal = new Expression();
                Element rootElement = new Element(ElementCode.POW);
                rootElement.mFirstOperand = A.mRootElement;
                rootElement.mSecondOperand = Element.TWO;
                retVal.mRootElement = rootElement;
            }
            return (retVal);
        }

        public static Expression multiply(Expression A, Expression B)
        {
            Expression retVal = null;
            if (A.SimpleNumber)
            {
                if (B.SimpleNumber)
                {
                    double value_Renamed = A.mRootElement.mNumericValue * B.mRootElement.mNumericValue;
                    retVal = new Expression(value_Renamed);
                }
                else
                {
                    if (A.mRootElement.mNumericValue == 0.0)
                    {
                        retVal = A;
                    }
                    else if (A.mRootElement.mNumericValue == 1.0)
                    {
                        retVal = B;
                    }
                    else if (A.mRootElement.mNumericValue == - 1.0)
                    {
                        if (!B.mRootElement.mCode.Equals(ElementCode.NEG))
                        {
                            Element rootElement = new Element(ElementCode.NEG);
                            rootElement.mFirstOperand = B.mRootElement;
                            retVal = new Expression();
                            retVal.mRootElement = rootElement;
                        }
                        else
                        {
                            retVal = new Expression();
                            retVal.mRootElement = B.mRootElement.mFirstOperand;
                        }
                    }
                }
            }
            if (null == retVal)
            {
                if (B.SimpleNumber)
                {
                    if (B.mRootElement.mNumericValue == 0.0)
                    {
                        retVal = B;
                    }
                    else if (B.mRootElement.mNumericValue == 1.0)
                    {
                        retVal = A;
                    }
                    else if (B.mRootElement.mNumericValue == - 1.0)
                    {
                        if (!A.mRootElement.mCode.Equals(ElementCode.NEG))
                        {
                            Element rootElement = new Element(ElementCode.NEG);
                            rootElement.mFirstOperand = A.mRootElement;
                            retVal = new Expression();
                            retVal.mRootElement = rootElement;
                        }
                        else
                        {
                            retVal = new Expression();
                            retVal.mRootElement = A.mRootElement.mFirstOperand;
                        }
                    }
                }
            }
            if (null == retVal)
            {
                retVal = new Expression();
                Element prod = new Element(ElementCode.MULT);
                prod.mFirstOperand = A.mRootElement;
                prod.mSecondOperand = B.mRootElement;
                retVal.mRootElement = prod;
            }
            return (retVal);
        }


        public static Expression divide(Expression A, Expression B)
        {
            Expression retVal = null;

            if (A.SimpleNumber && A.mRootElement.mNumericValue == 0.0)
            {
                retVal = A;
            }
            else
            {
                retVal = new Expression();
                Element quotient = new Element(ElementCode.DIV);
                quotient.mFirstOperand = A.mRootElement;
                quotient.mSecondOperand = B.mRootElement;
                retVal.mRootElement = quotient;
            }

            return (retVal);
        }

        public static Expression negate(Expression A)
        {
            Expression retVal = null;
            if (A.SimpleNumber)
            {
                double value_Renamed = (- 1.0) * A.mRootElement.mNumericValue;
                retVal = new Expression(value_Renamed);
            }
            else
            {
                if (A.mRootElement.mCode.Equals(ElementCode.NEG))
                {
                    retVal = new Expression();
                    retVal.mRootElement = A.mRootElement.mFirstOperand;
                }
                else
                {
                    retVal = new Expression();
                    Element rootElement = new Element(ElementCode.NEG);
                    rootElement.mFirstOperand = A.mRootElement;
                    retVal.mRootElement = rootElement;
                }
            }
            return (retVal);
        }

        // returns A - B
        public static Expression subtract(Expression A, Expression B)
        {
            Expression retVal = null;
            if (A.SimpleNumber && A.mRootElement.mNumericValue == 0.0)
            {
                if (B.SimpleNumber)
                {
                    retVal = new Expression((- 1.0) * B.mRootElement.mNumericValue);
                }
                else
                {
                    if (!B.mRootElement.mCode.Equals(ElementCode.NEG))
                    {
                        retVal = new Expression();
                        retVal.mRootElement = new Element(ElementCode.NEG);
                        retVal.mRootElement.mFirstOperand = B.mRootElement;
                    }
                    else
                    {
                        retVal = new Expression();
                        retVal.mRootElement = B.mRootElement.mFirstOperand;
                    }
                }
            }
            else
            {
                if (B.SimpleNumber && B.mRootElement.mNumericValue == 0.0)
                {
                    retVal = A;
                }
                else
                {
                    if (A.SimpleNumber && B.SimpleNumber)
                    {
                        double value_Renamed = A.mRootElement.mNumericValue - B.mRootElement.mNumericValue;
                        retVal = new Expression(value_Renamed);
                    }
                    else
                    {
                        retVal = new Expression();
                        Element diff = new Element(ElementCode.SUBT);
                        diff.mFirstOperand = A.mRootElement;
                        diff.mSecondOperand = B.mRootElement;
                        retVal.mRootElement = diff;
                    }
                }
            }
            return (retVal);
        }

        public static Expression add(Expression A, Expression B)
        {
            Expression retVal = null;
            if (A.SimpleNumber && A.mRootElement.mNumericValue == 0.0)
            {
                retVal = B;
            }
            else
            {
                if (B.SimpleNumber && B.mRootElement.mNumericValue == 0.0)
                {
                    retVal = A;
                }
                else
                {
                    if (A.SimpleNumber && B.SimpleNumber)
                    {
                        double value_Renamed = A.mRootElement.mNumericValue + B.mRootElement.mNumericValue;
                        retVal = new Expression(value_Renamed);
                    }
                    else
                    {
                        retVal = new Expression();
                        Element sum = new Element(ElementCode.ADD);
                        sum.mFirstOperand = A.mRootElement;
                        sum.mSecondOperand = B.mRootElement;
                        retVal.mRootElement = sum;
                    }
                }
            }
            return (retVal);
        }

        [STAThread]
        public static void  Main(System.String[] pArgs)
        {
            try
            {
                System.IO.Stream in_Renamed = System.Console.OpenStandardInput();
                System.IO.StreamReader reader = new System.IO.StreamReader(in_Renamed, System.Text.Encoding.Default);
                //UPGRADE_TODO: The differences in the expected value  of parameters for constructor 'java.io.BufferedReader.BufferedReader'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
                System.IO.StreamReader bufReader = new System.IO.StreamReader(reader.BaseStream, reader.CurrentEncoding);
                System.String line = null;
                while (null != (line = bufReader.ReadLine()))
                {
                    Expression expression = new Expression(line);
                    System.Console.Out.WriteLine(expression.ToString());
                }
            }
            catch (System.Exception e)
            {
                SupportClass.WriteStackTrace(e, System.Console.Error);
            }
        }
    }
}
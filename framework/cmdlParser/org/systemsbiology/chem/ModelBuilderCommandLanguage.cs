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
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

//UPGRADE_TODO: The package 'java.util.regex' could not be found. If it was not included in the conversion, there may be compiler issues. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1262'"
//J using java.util.regex;
//J using java.nio.charset;

using org.systemsbiology.util;
using org.systemsbiology.math;

using CeeEmDeeEl;

namespace org.systemsbiology.chem
{

    /// <summary> Builds a {@link Model} object from a CMDL model description.
    ///
    /// </summary>
    /// <author>  sramsey
    /// </author>

    public class ModelBuilderCommandLanguage : IModelBuilder, IAliasableClass
    {
        //J private Pattern SearchPatternMath
        private Regex SearchPatternMath
        {
            get
            {
                return (mSearchPatternMath);
            }

            set
            {
                mSearchPatternMath = value;
            }

        }
        virtual public String FileRegex
        {
            get
            {
                return (".*\\.(dizzy|cmdl)$");
            }

        }

        private Hashtable mDefaultModelSymbols;

        public const String CLASS_ALIAS = "command-language";
        private const String DEFAULT_MODEL_NAME = "model";
        private const String REACTION_MODIFIER_STEPS = "steps:";
        private const String REACTION_MODIFIER_DELAY = "delay:";

        private const String STATEMENT_KEYWORD_INCLUDE = "include";
        private const String STATEMENT_KEYWORD_MODEL = "model";
        private const String STATEMENT_KEYWORD_REF = "ref";
        private const String STATEMENT_KEYWORD_DEFINE = "define";
        private const String KEYWORD_LOOP = "loop";
        private const String VALID_SYMBOL_REGEX = "^[a-zA-Z]([_a-zA-Z0-9])*$";
        //J private static readonly Pattern VALID_SYMBOL_PATTERN = Pattern.compile(VALID_SYMBOL_REGEX);
        private static readonly Regex VALID_SYMBOL_PATTERN = new Regex(VALID_SYMBOL_REGEX);
        private const String REQUIRED_CHAR_SET = "UTF-8";
        private const String COMPARTMENT_NAME_DEFAULT = "univ";


        private String mNamespace;

        //J private static Charset sCharset;

        internal class Macro:SymbolValue
        {
            public String mMacroName;
            public ArrayList mExternalSymbols;

            public List<Token> mTokenList;

            public Macro(String pMacroName):base(pMacroName)
            {
                mMacroName = pMacroName;
            }
        }

        internal class DummySymbol:SymbolValue
        {
            public Object mInstanceSymbolObject;

            public DummySymbol(String pDummySymbolName, Object pInstanceSymbolObject):base(pDummySymbolName)
            {
                mInstanceSymbolObject = pInstanceSymbolObject;
            }
        }

        internal class Token
        {
            //UPGRADE_NOTE: The access modifier for this class or class field has been changed in order to prevent compilation errors due to the visibility level. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1296'"
            public class Code
            {
                private String mName;
                internal Code(String pName)
                {
                    mName = pName;
                }
                public override String ToString()
                {
                    return mName;
                }
                public static readonly Code POUNDSIGN = new Code("#");
                public static readonly Code ATSIGN = new Code("@");
                public static readonly Code EQUALS = new Code("=");
                public static readonly Code SYMBOL = new Code("symbol");
                public static readonly Code HYPHEN = new Code("-");
                public static readonly Code COMMA = new Code(",");
                public static readonly Code GREATER_THAN = new Code(">");
                public static readonly Code PLUS = new Code("+");
                public static readonly Code BRACKET_BEGIN = new Code("[");
                public static readonly Code BRACKET_END = new Code("]");
                public static readonly Code PAREN_BEGIN = new Code("(");
                public static readonly Code PAREN_END = new Code(")");
                public static readonly Code BRACE_BEGIN = new Code("{");
                public static readonly Code BRACE_END = new Code("}");
                public static readonly Code QUOTE = new Code("\"");
                public static readonly Code SEMICOLON = new Code(";");
                public static readonly Code DOLLAR = new Code("$");
                public static readonly Code ASTERISK = new Code("*");
                public static readonly Code PERCENT = new Code("%");
                public static readonly Code RIGHT_SLASH = new Code("/");
                public static readonly Code CARET = new Code("^");
            }

            internal Code mCode;
            internal String mSymbol;
            internal int mLine;

            public Token(Code pCode)
            {
                mCode = pCode;
            }

            public override String ToString()
            {
                String string_Renamed = null;
                if (mCode.Equals(Code.SYMBOL))
                {
                    string_Renamed = mSymbol;
                }
                else
                {
                    //J string_Renamed = mCode.mName;
                    string_Renamed = mCode.ToString();
                }
                return (string_Renamed);
            }
        }

        internal class LoopIndex:SymbolValue
        {
            public LoopIndex(String pIndexName, int pValue):base(pIndexName)
            {
                setValue(new Value((double) pValue));
            }

            public virtual void  setValue(int pValue)
            {
                getValue().setValue((double) pValue);
            }

            public override String ToString()
            {
                //UPGRD_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
                int value_Renamed = (int) getValue().getValue();
                return (System.Convert.ToString(value_Renamed));
            }
        }

        //J private Pattern mSearchPatternMath;
        private Regex mSearchPatternMath;

        private void  initializeSearchPatternMath()
        {
            String searchRegex = "\\[([^\\[\\]]+)\\]";
            //J Pattern searchPattern = Pattern.compile(searchRegex);
            Regex searchPattern = new Regex(searchRegex);
            SearchPatternMath = searchPattern;
        }

        private void  initializeDefaultModelSymbols()
        {

            mDefaultModelSymbols = new Hashtable();
            Compartment compartment = new Compartment(COMPARTMENT_NAME_DEFAULT);
            mDefaultModelSymbols[compartment.Name] = compartment;
        }

        public ModelBuilderCommandLanguage()
        {
            initializeSearchPatternMath();
            initializeDefaultModelSymbols();
            mNamespace = null;
        }

        private Token getNextToken(IListIterator<Token> pTokenIter)
        {
            if (!pTokenIter.HasNext)
            {
                throw new InvalidInputException("expected a token, but no token was found");
            }

            Token token = pTokenIter.Next;
            Debug.Assert(null != token, "unexpected null token");

            return (token);
        }



        private void  defineParameters(Hashtable pSymbolMap, Model pModel)
        {
            IEnumerator symbolValueIter = pSymbolMap.Values.GetEnumerator();

            while (symbolValueIter.MoveNext())
            {
                SymbolValue symbolValue = (SymbolValue) symbolValueIter.Current;
                Type getClass           = symbolValue.GetType();
                Type superClass         = getClass.BaseType;
                Type objectClass        = typeof(Object);

                if (superClass.Equals(objectClass))
                {
                    Parameter parameter = new Parameter(symbolValue);
                    pModel.addParameter(parameter);
                }
            }
        }

        private void  tokenizeStatement(String pStatement, IList<Token> pTokens, int pStartingLineNumber)
        {
            SupportClass.Tokenizer st = new SupportClass.Tokenizer(pStatement, "=\", \t[]{}()->+;@#$*/%^\n", true);

            String tokenString = null;
            bool inQuote = false;
            StringBuilder symbolTokenBuffer = new StringBuilder();
            int lineCtr = pStartingLineNumber;

            while (st.HasMoreTokens())
            {
                tokenString = st.NextToken();

                Token token = null;

                if (tokenString.Equals("\n"))
                {
                    ++lineCtr;
                }
                else
                {
                    if (tokenString.Equals("\""))
                    {
                        if (inQuote)
                        {
                            inQuote = false;
                        }
                        else
                        {
                            inQuote = true;
                        }

                        token = new Token(Token.Code.QUOTE);
                    }
                    else
                    {
                        if (!inQuote)
                        {
                            if (tokenString.Equals("="))
                            {
                                token = new Token(Token.Code.EQUALS);
                            }
                            else if (tokenString.Equals(","))
                            {
                                token = new Token(Token.Code.COMMA);
                            }
                            else if (tokenString.Equals("="))
                            {
                                token = new Token(Token.Code.EQUALS);
                            }
                            else if (tokenString.Equals(" "))
                            {
                                // this is whitespace, just ignore
                            }
                            else if (tokenString.Equals("\t"))
                            {
                                // this is a tab character, just ignore
                            }
                            else if (tokenString.Equals("("))
                            {
                                token = new Token(Token.Code.PAREN_BEGIN);
                            }
                            else if (tokenString.Equals(")"))
                            {
                                token = new Token(Token.Code.PAREN_END);
                            }
                            else if (tokenString.Equals("["))
                            {
                                token = new Token(Token.Code.BRACKET_BEGIN);
                            }
                            else if (tokenString.Equals("]"))
                            {
                                token = new Token(Token.Code.BRACKET_END);
                            }
                            else if (tokenString.Equals("{"))
                            {
                                token = new Token(Token.Code.BRACE_BEGIN);
                            }
                            else if (tokenString.Equals("}"))
                            {
                                token = new Token(Token.Code.BRACE_END);
                            }
                            else if (tokenString.Equals("-"))
                            {
                                token = new Token(Token.Code.HYPHEN);
                            }
                            else if (tokenString.Equals(">"))
                            {
                                token = new Token(Token.Code.GREATER_THAN);
                            }
                            else if (tokenString.Equals("+"))
                            {
                                token = new Token(Token.Code.PLUS);
                            }
                            else if (tokenString.Equals(";"))
                            {
                                token = new Token(Token.Code.SEMICOLON);
                            }
                            else if (tokenString.Equals("@"))
                            {
                                token = new Token(Token.Code.ATSIGN);
                            }
                            else if (tokenString.Equals("#"))
                            {
                                token = new Token(Token.Code.POUNDSIGN);
                            }
                            else if (tokenString.Equals("$"))
                            {
                                token = new Token(Token.Code.DOLLAR);
                            }
                            else if (tokenString.Equals("/"))
                            {
                                token = new Token(Token.Code.RIGHT_SLASH);
                            }
                            else if (tokenString.Equals("*"))
                            {
                                token = new Token(Token.Code.ASTERISK);
                            }
                            else if (tokenString.Equals("%"))
                            {
                                token = new Token(Token.Code.PERCENT);
                            }
                            else if (tokenString.Equals("^"))
                            {
                                token = new Token(Token.Code.CARET);
                            }
                            else
                            {
                                token = new Token(Token.Code.SYMBOL);
                                token.mSymbol = tokenString;
                            }
                        }
                        else
                        {
                            // we are in a quoted environment; just save the token string
                            symbolTokenBuffer.Append(tokenString);
                        }
                    }
                }

                if (tokenString.Equals("\"") && (!inQuote))
                {
                    String symbolTokenString = symbolTokenBuffer.ToString();
                    if (symbolTokenString.Length > 0)
                    {
                        Token symbolToken = new Token(Token.Code.SYMBOL);
                        symbolToken.mSymbol = symbolTokenString;
                        symbolTokenBuffer.Remove(0, symbolTokenString.Length - 0);
                        pTokens.Add(symbolToken);
                    }
                }

                if (null != token)
                {
                    token.mLine = lineCtr;
                    pTokens.Add(token);
                    token = null;
                }
                else
                {
                    // do nothing
                }
            }
        }

        private void  checkSymbolValidity(String pSymbolName)
        {
            if (ReservedSymbolMapperChemCommandLanguage.isReservedSymbol(pSymbolName))
            {
                throw new InvalidInputException("attempt to define a reserved symbol: " + pSymbolName);
            }
            //UPGRADE_TODO: Method 'java.util.HashMap.get' was converted to 'Hashtable.Item' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMapget_javalangObject'"
/*
            if (null != mDefaultModelSymbols[pSymbolName])
            {
                throw new InvalidInputException("symbol name is reserved as a default model element: " + pSymbolName);
            }
*/
            if (mDefaultModelSymbols.ContainsKey(pSymbolName))
                throw new InvalidInputException("symbol name is reserved as a default model element: " + pSymbolName);

            if (!isValidSymbol(pSymbolName))
            {
                throw new InvalidInputException("invalid symbol definition: " + pSymbolName);
            }
        }


        private void  processDefaultModelElements(Hashtable pSymbolMap)
        {
            IEnumerator defaultModelSymbolsIter = mDefaultModelSymbols.Values.GetEnumerator();

            while (defaultModelSymbolsIter.MoveNext())
            {

                SymbolValue defaultModelSymbolValue = (SymbolValue) defaultModelSymbolsIter.Current;
                pSymbolMap[defaultModelSymbolValue.Symbol.Name] = defaultModelSymbolValue;
            }
        }

        internal class SymbolEvaluatorNamespaced:SymbolEvaluatorHashMap
        {
            private String mNamespace;


            public SymbolEvaluatorNamespaced(Hashtable pSymbolMap, String pNamespace):base(pSymbolMap)
            {
                mNamespace = pNamespace;
            }

            public override double getUnindexedValue(Symbol pSymbol)
            {
                Object convertedSymbol = org.systemsbiology.chem.ModelBuilderCommandLanguage.performSymbolTokenMacroTranslationLookup(pSymbol.Name, mSymbolMap, mNamespace);
                double retVal = 0.0;
                if (convertedSymbol is String)
                {
                    String symbolName = (String) convertedSymbol;
                    retVal = base.getValue(symbolName);
                }
                else if (convertedSymbol is Value)
                {
                    Value symbolValue = (Value) convertedSymbol;
                    retVal = symbolValue.getValue(this);
                }
                else
                {
                    Debug.Assert(false, "unknown converted symbol type, for symbol: " + pSymbol.Name);
                }

                return (retVal);
            }
        }

        private static String addNamespaceToSymbol(String pSymbolName, String pNamespace)
        {
            String retName = pSymbolName;
            if (null != pNamespace)
            {
                retName = pNamespace + Model.NAMESPACE_IDENTIFIER + pSymbolName;
            }
            return (retName);
        }

        // When the parser encounters a symbol token, it must check to see if this
        // symbol is one of the special "dummy symbols" in a macro reference.  If so,
        // the "dummy symbol" must be translated in accordance with the macro's translation
        // table.  It can get translated into another symbol (most common), or into a
        // value.  The return type of the function is either String or Value, depending
        // on whether the translation returns a symbol or a value.

        private static Object performSymbolTokenMacroTranslationLookup(String pSymbol, Hashtable pSymbolMap, String pNamespace)
        {
            Object retObj = pSymbol;
            if (null != pNamespace)
            {
                if (!ReservedSymbolMapperChemCommandLanguage.isReservedSymbol(pSymbol))
                {
                    pSymbol = addNamespaceToSymbol(pSymbol, pNamespace);
                    retObj = pSymbol;

                    // check to see if this is a dummy symbol
                    //UPGRADE_TODO: Method 'java.util.HashMap.get' was converted to 'Hashtable.Item' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMapget_javalangObject'"
                    SymbolValue symbolValue = (SymbolValue) pSymbolMap[pSymbol];
                    if (null != symbolValue && symbolValue is DummySymbol)
                    {
                        DummySymbol dummySymbol = (DummySymbol) symbolValue;
                        Object dummySymbolInstance = dummySymbol.mInstanceSymbolObject;
                        if (dummySymbolInstance is String)
                        {
                            retObj = dummySymbolInstance;
                        }
                        else if (dummySymbolInstance is Value)
                        {
                            retObj = dummySymbolInstance;
                        }
                        else
                        {
                            Debug.Assert(false, "unknown dummy symbol instance class, for symbol: " + pSymbol);
                        }
                    }
                }
                else
                {
                    // do nothing; reserved symbols do not get namespace scoping
                }
            }
            return (retObj);
        }

        // Handle the occurrence of embedded mathematical expression(s)
        // within a quoted string; if an expression occurs, translate it
        // and truncate the resulting value to an integer, before embedding
        // the result into the string at the point where the expression occurs.

        private String translateMathExpressionsInString(String pInputString, Hashtable pSymbolMap)
        {
/*J
            Pattern searchPatternMath = SearchPatternMath;
            Matcher matcher = searchPatternMath.matcher(pInputString);
            while (matcher.find())
*/
            Regex searchPatternMath = SearchPatternMath;
            Match m = searchPatternMath.Match(pInputString);
            if (searchPatternMath.IsMatch(pInputString))
            {
                //J String matchedSubsequence = matcher.group(1);
                String matchedSubsequence = m.Groups[1].ToString();
                Expression exp = new Expression(matchedSubsequence);
                SymbolEvaluatorNamespaced evaluator = new SymbolEvaluatorNamespaced(pSymbolMap, mNamespace);
                double value_Renamed = exp.computeValue(evaluator);
                //UPGRD_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
                String formattedExp = System.Convert.ToString((long) value_Renamed);
                //J pInputString = matcher.replaceFirst(formattedExp);
                //J matcher = searchPatternMath.matcher(pInputString);
                pInputString = Regex.Replace(pInputString, "\\[([^\\[\\]]+)\\]", formattedExp);
            }

            return (pInputString);
        }

        // This function is used by the handleStatementMacroReference
        // function, to grab all of the tokens in the token stream up to
        // the next comma "," token or end-parenthesis ")" token.  The
        // reason why we cannot just use "obtainSymbol" is that in a
        // macro reference, either a symbol *or* a value can be provided
        // in the comma-separated list.  It is the only place in the CMDL
        // where either a symbol, or a value/expression is allowed.  So
        // special code is needed to handle this case, since we do not
        // know definitively whether to expect a symbol or a value/expression.


        private void grabTokensToNextCommaOrParen(IListIterator<Token> pTokenIter, List<Token> pTokens, Hashtable pSymbolMap)
        {
            Token token = null;
            bool inQuote = false;
            StringBuilder quotedString = new StringBuilder();
            int parenDepth = 0;

            while (pTokenIter.HasNext)
            {
                token = getNextToken(pTokenIter);
                if (!inQuote)
                {
                    if (token.mCode.Equals(Token.Code.PAREN_END))
                    {
                        if (0 == parenDepth)
                        {
                            //UPGRADE_ISSUE: Method 'java.util.ListIterator.previous' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javautilListIteratorprevious'"
                            pTokenIter.GetPrevious();
                            break;
                        }
                        else
                        {
                            --parenDepth;
                            if (parenDepth < 0)
                            {
                                throw new InvalidInputException("mismatched number of parentheses detected within expression");
                            }
                            pTokens.Add(token);
                        }
                    }
                    else if (token.mCode.Equals(Token.Code.PAREN_BEGIN))
                    {
                        ++parenDepth;
                        pTokens.Add(token);
                    }
                    else if (token.mCode.Equals(Token.Code.SEMICOLON))
                    {
                        throw new InvalidInputException("end of statement reached when parenthesis expected");
                    }
                    else if (token.mCode.Equals(Token.Code.COMMA))
                    {
                        if (parenDepth == 0)
                        {
                            //UPGRADE_ISSUE: Method 'java.util.ListIterator.previous' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javautilListIteratorprevious'"
                            pTokenIter.GetPrevious();
                            break;
                        }
                        else
                        {
                            pTokens.Add(token);
                        }
                    }
                    else if (token.mCode.Equals(Token.Code.QUOTE))
                    {
                        inQuote = true;
                    }
                    else
                    {
                        pTokens.Add(token);
                    }
                }
                else
                {
                    if (token.mCode.Equals(Token.Code.QUOTE))
                    {
                        inQuote = false;
                        Token newToken = new Token(Token.Code.SYMBOL);
                        String symbolName = quotedString.ToString();
                        quotedString.Remove(0, symbolName.Length - 0);
                        symbolName = translateMathExpressionsInString(symbolName, pSymbolMap);
                        newToken.mSymbol = symbolName;
                        pTokens.Add(newToken);
                    }
                    else
                    {
                        quotedString.Append(token.ToString());
                    }
                }
            }

            if (pTokens.Count == 0)
            {
                throw new InvalidInputException("a symbol token was expected in list environment");
            }
        }


        private String obtainSymbol(IListIterator<Token> pTokenIter, Hashtable pSymbolMap)
        {
            Token token = getNextToken(pTokenIter);
            String symbolName = null;
            if (token.mCode.Equals(Token.Code.SYMBOL))
            {
                symbolName = token.mSymbol;
            }
            else
            {
                if (token.mCode.Equals(Token.Code.QUOTE))
                {
                    token = getNextToken(pTokenIter);
                    if (!token.mCode.Equals(Token.Code.SYMBOL))
                    {
                        throw new InvalidInputException("expected symbol token after quote");
                    }

                    symbolName = translateMathExpressionsInString(token.mSymbol, pSymbolMap);

                    token = getNextToken(pTokenIter);
                    if (!token.mCode.Equals(Token.Code.QUOTE))
                    {
                        throw new InvalidInputException("expected end quote token");
                    }
                }
                else
                {
                    throw new InvalidInputException("expected symbol or quoted string; instead found token: " + token);
                }
            }

            checkSymbolValidity(symbolName);

            return (symbolName);
        }



        private String obtainSymbolWithNamespace(IListIterator<Token> pTokenIter, Hashtable pSymbolMap)
        {
            String symbol = obtainSymbol(pTokenIter, pSymbolMap);
            Object convertedSymbolObj = performSymbolTokenMacroTranslationLookup(symbol, pSymbolMap, mNamespace);
            String retSymbol = null;
            if (convertedSymbolObj is String)
            {
                retSymbol = ((String) convertedSymbolObj);
            }
            else
            {
                throw new InvalidInputException("where expected a symbol, macro reference translated into a value: " + symbol);
            }
            return (retSymbol);
        }

        private bool isNumericLiteral(String pTokenString)
        {
            double test;
            return Double.TryParse(pTokenString, out test);
        }

        // For places where the parser expects to be able to obtain a value, this
        // function walks the token list and attempts to parse a value.  The value
        // may be either a "deferred evaluation" expression, or a simple numeric value.

        private Value obtainValue(IListIterator<Token> pTokenIter, Hashtable pSymbolMap, bool pAllowDeferred)
        {
            bool allowTerminateOnParen = false;
            return obtainValue(pTokenIter, pSymbolMap, pAllowDeferred, allowTerminateOnParen);
        }

        // For places where the parser expects to be able to obtain a value, this
        // function walks the token list and attempts to parse a value.  The value
        // may be either a "deferred evaluation" expression, or a simple numeric value.

        private Value obtainValue(IListIterator<Token> pTokenIter, Hashtable pSymbolMap, bool pAllowDeferred, bool pAllowTerminateOnParen)
        {
            bool firstToken = true;
            StringBuilder expressionBuffer = new StringBuilder();
            bool deferredExpression = false;

            StringBuilder quoteBuffer = new StringBuilder();
            bool inQuote = false;
            int parenDepth = 0;

            while (pTokenIter.HasNext)
            {
                Token token = getNextToken(pTokenIter);

                if (inQuote)
                {
                    // we are in a quotation; therefore, this can't be the first token
                    Debug.Assert(!firstToken, "first toke, and within a quotation");

                    if (token.mCode.Equals(Token.Code.QUOTE))
                    {
                        String quotedString = quoteBuffer.ToString();
                        quoteBuffer.Remove(0, quoteBuffer.Length - 0);
                        quotedString = translateMathExpressionsInString(quotedString, pSymbolMap);
                        expressionBuffer.Append(quotedString);
                        inQuote = false;
                    }
                    else
                    {
                        quoteBuffer.Append(token.ToString());
                    }
                }
                else
                {
                    // we are not within a quotation

                    if (token.mCode.Equals(Token.Code.SEMICOLON) || (token.mCode.Equals(Token.Code.COMMA) && parenDepth == 0))
                    {
                        if (firstToken)
                        {
                            throw new InvalidInputException("expression was expected, but instead, a comma or semicolon was found");
                        }
                        else
                        {
                            if (parenDepth > 0)
                            {
                                throw new InvalidInputException("expression ended with mismatched parentheses");
                            }

                            //UPGRADE_ISSUE: Method 'java.util.ListIterator.previous' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javautilListIteratorprevious'"
                            pTokenIter.GetPrevious();
                            break;
                        }
                    }

                    // we are not within a quotation, and the token is not a comma, or semicolon

                    if (token.mCode.Equals(Token.Code.BRACKET_BEGIN))
                    {
                        if (!pAllowDeferred)
                        {
                            throw new InvalidInputException("deferred evaluation expression not allowed here, so this token was unexpected: \"" + token.mCode + "\"");
                        }
                        if (firstToken)
                        {
                            deferredExpression = true;
                        }
                        else
                        {
                            throw new InvalidInputException("unexpected token encountered in an expression: \"" + token.mCode + "\"");
                        }
                    }
                    else if (token.mCode.Equals(Token.Code.QUOTE))
                    {
                        inQuote = true;
                    }
                    else if (token.mCode.Equals(Token.Code.BRACKET_END))
                    {
                        if (!pAllowDeferred)
                        {
                            throw new InvalidInputException("deferred evaluation expression not allowed here, so this token was unexpected: \"" + token.mCode + "\"");
                        }
                        if (!deferredExpression)
                        {
                            throw new InvalidInputException("encountered closing bracket without an opening bracket");
                        }

                        if (inQuote)
                        {
                            throw new InvalidInputException("missing quotation mark in expression");
                        }

                        break;
                    }
                    else if (token.mCode.Equals(Token.Code.PAREN_BEGIN))
                    {
                        expressionBuffer.Append(token.ToString());
                        ++parenDepth;
                    }
                    else if (token.mCode.Equals(Token.Code.PAREN_END))
                    {
                        if (parenDepth == 0 && pAllowTerminateOnParen)
                        {
                            //UPGRADE_ISSUE: Method 'java.util.ListIterator.previous' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javautilListIteratorprevious'"
                            pTokenIter.GetPrevious();
                            break;
                        }
                        else
                        {
                            expressionBuffer.Append(token.ToString());
                            --parenDepth;
                            if (parenDepth < 0)
                            {
                                throw new InvalidInputException("mismatched number of parentheses detected within an expression");
                            }
                        }
                    }
                    else if (token.mCode.Equals(Token.Code.SYMBOL))
                    {
                        String symbolString = token.mSymbol;
                        if (!isNumericLiteral(symbolString))
                        {
                            Object translatedObject = performSymbolTokenMacroTranslationLookup(symbolString, pSymbolMap, mNamespace);
                            if (translatedObject is String)
                            {
                                symbolString = ((String) translatedObject);
                            }
                            else if (translatedObject is Value)
                            {
                                Value symbolValueObj = (Value) translatedObject;
                                if (symbolValueObj.IsExpression)
                                {
                                    symbolString = symbolValueObj.getExpressionString();
                                }
                                else
                                {
                                    double value_Renamed = symbolValueObj.getValue();
                                    symbolString = value_Renamed.ToString();
                                }
                            }
                        }
                        else
                        {
                            // do nothing
                        }

                        expressionBuffer.Append(symbolString);
                    }
                    else
                    {
                        expressionBuffer.Append(token.ToString());
                    }

                    if (firstToken)
                    {
                        firstToken = false;
                    }
                }
            }

            if (inQuote)
            {
                throw new InvalidInputException("expression ended without a matching quotation character");
            }

            String expressionString = expressionBuffer.ToString();
            Expression expression = null;
            try
            {
                expression = new Expression(expressionString);
            }
            catch (ArgumentException e)
            {
                //UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Throwable.getMessage' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
                throw new InvalidInputException("invalid mathematical formula \"" + expressionString + "\"; cause is: " + e.Message + ";", e);
            }
            Value value_Renamed2 = null;
            if (deferredExpression)
            {
                value_Renamed2 = new Value(expression);
            }
            else
            {
                try
                {
                    value_Renamed2 = new Value(expression.computeValue(pSymbolMap));
                }
                catch (DataNotFoundException e)
                {
                    throw new InvalidInputException("unable to determine value for expression: " + expressionString, e);
                }
            }

            return (value_Renamed2);
        }


        private void handleStatementAssociate(IListIterator<Token> pTokenIter, Model pModelHashMap, Hashtable pSymbolMap)
        {
            String symbolName = obtainSymbolWithNamespace(pTokenIter, pSymbolMap);
            Debug.Assert(null != symbolName, "null symbol string for symbol token");

            Token token = getNextToken(pTokenIter);
            if (!token.mCode.Equals(Token.Code.ATSIGN))
            {
                if (token.mCode.Equals(Token.Code.BRACKET_BEGIN))
                {
                    throw new InvalidInputException("encountered begin bracket token when expected at-sign token; perhaps you forgot to put double quotes around your symbol definition?");
                }
                else
                {
                    throw new InvalidInputException("encountered unknown token when expected at-sign token");
                }
            }

            String associatedSymbolName = obtainSymbolWithNamespace(pTokenIter, pSymbolMap);
            //UPGRADE_TODO: Method 'java.util.HashMap.get' was converted to 'Hashtable.Item' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMapget_javalangObject'"
            SymbolValue associatedSymbolValue = (SymbolValue) pSymbolMap[associatedSymbolName];
            Compartment compartment = null;
            if (!(associatedSymbolValue is Compartment))
            {
                if (!associatedSymbolValue.GetType().BaseType.Equals(typeof(Object)))
                {
                    throw new InvalidInputException("symbol \"" + associatedSymbolName + "\" is already defined as an incompatible symbol type");
                }
                compartment = new Compartment(associatedSymbolValue);
                pSymbolMap[associatedSymbolName] = compartment;
            }
            else
            {
                compartment = (Compartment) associatedSymbolValue;
            }

            //UPGRADE_TODO: Method 'java.util.HashMap.get' was converted to 'Hashtable.Item' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMapget_javalangObject'"
            SymbolValue symbolValue = (SymbolValue) pSymbolMap[symbolName];
            Species species = null;
            if (!(symbolValue is Species))
            {
                if (!symbolValue.GetType().BaseType.Equals(typeof(Object)))
                {
                    throw new InvalidInputException("symbol \"" + symbolName + "\" is already defined as an incompatible symbol type");
                }
                species = new Species(symbolValue, compartment);
                pSymbolMap[symbolName] = species;
            }
            else
            {
                species = (Species) symbolValue;
                if (!species.Compartment.equals(compartment))
                {
                    throw new InvalidInputException("species \"" + symbolName + "\" is already assigned to a different compartment: " + compartment.Name);
                }
            }

            getEndOfStatement(pTokenIter);
        }


        private void handleStatementSymbolDefinition(IListIterator<Token> pTokenIter, Model pModelHashMap, Hashtable pSymbolMap)
        {
            String symbolName = obtainSymbolWithNamespace(pTokenIter, pSymbolMap);

            Debug.Assert(null != symbolName, "null symbol string for symbol token");

            Token token = getNextToken(pTokenIter);

            if (!token.mCode.Equals(Token.Code.EQUALS))
            {
                if (token.mCode.Equals(Token.Code.BRACKET_BEGIN))
                {
                    throw new InvalidInputException("encountered begin bracket token when expected equals token; perhaps you forgot to put double quotes around your symbol definition?");
                }
                else
                {
                    throw new InvalidInputException("encountered unknown token when expected equals token");
                }
            }

            bool allowDeferred = true;
            Value value_Renamed = obtainValue(pTokenIter, pSymbolMap, allowDeferred);
            SymbolValue symbolValue = new SymbolValue(symbolName, value_Renamed);
            //UPGRADE_TODO: Method 'java.util.HashMap.get' was converted to 'Hashtable.Item' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMapget_javalangObject'"
/*
            SymbolValue foundSymbolValue = (SymbolValue) pSymbolMap[symbolName];
            if (null != foundSymbolValue)
            {
                // :TODO: this is a hack until I can get an "if/then/else" construct working
                // in the CMDL.  When defining a vector of species inside a loop, it is
                // often useful to be able to have a nonzero value for just one element of
                // the vector.  With multiple nested loops, this is difficult to accomplish.
                // So for now, we make it legal to redefine the Value after the loop, so
                // long as the symbol value has not yet been turned into a derived class.
                if (null == foundSymbolValue.getValue() || foundSymbolValue.GetType().BaseType.Equals(typeof(Object)))
                {
                    // this SymbolValue either has no Value assigned yet, or it
                    // is just a plain symbol value; it is ok to redefine its value
                    foundSymbolValue.setValue(symbolValue.getValue());
                }
                else
                {
                    throw new InvalidInputException("a symbol of type " + foundSymbolValue.GetType().FullName + " cannot have its value redefined; symbol name is: " + symbolName);
                }
            }
            else
            {
                pSymbolMap[symbolName] = symbolValue;
            }
*/
            if (pSymbolMap.ContainsKey(symbolName))
            {
                SymbolValue foundSymbolValue = (SymbolValue)pSymbolMap[symbolName];

                if (null == foundSymbolValue.getValue() || foundSymbolValue.GetType().BaseType.Equals(typeof(Object)))
                {
                    // this SymbolValue either has no Value assigned yet, or it
                    // is just a plain symbol value; it is ok to redefine its value
                    foundSymbolValue.setValue(symbolValue.getValue());
                }
                else
                {
                    throw new InvalidInputException("a symbol of type " + foundSymbolValue.GetType().FullName + " cannot have its value redefined; symbol name is: " + symbolName);
                }
            }
            else
            {
                pSymbolMap[symbolName] = symbolValue;
            }

            getEndOfStatement(pTokenIter);
        }


        internal static Compartment getDefaultCompartment(Hashtable pSymbolMap)
        {
            //UPGRADE_TODO: Method 'java.util.HashMap.get' was converted to 'Hashtable.Item' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMapget_javalangObject'"
            Compartment compartment = (Compartment) pSymbolMap[COMPARTMENT_NAME_DEFAULT];
            return (compartment);
        }


        private void getReactionParticipants(IListIterator<Token> pTokenIter, Hashtable pSymbolMap, Hashtable pSpeciesStoicMap, Hashtable pSpeciesDynamicMap, ReactionParticipant.Type pParticipantType)
        {
            while (pTokenIter.HasNext)
            {
                bool dynamic = true;
                // get symbol
                Token token = getNextToken(pTokenIter);
                if (token.mCode.Equals(Token.Code.DOLLAR))
                {
                    dynamic = false;
                }
                else
                {
                    //UPGRADE_ISSUE: Method 'java.util.ListIterator.previous' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javautilListIteratorprevious'"
                    pTokenIter.GetPrevious();
                }

                String speciesName = obtainSymbolWithNamespace(pTokenIter, pSymbolMap);
                //UPGRADE_TODO: Method 'java.util.HashMap.get' was converted to 'Hashtable.Item' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMapget_javalangObject'"
/*
                MutableInteger speciesStoic = (MutableInteger) pSpeciesStoicMap[speciesName];
                if (null == speciesStoic)
                {
                    speciesStoic = new MutableInteger(1);
                    pSpeciesStoicMap[speciesName] = speciesStoic;
                }
                else
                {
                    int stoic = speciesStoic.Value + 1;
                    speciesStoic.Value = stoic;
                }
*/
                int speciesStoic = 1;
                if (!pSpeciesStoicMap.ContainsKey(speciesName))
                {
                    pSpeciesStoicMap.Add(speciesName, speciesStoic);
                }
                else
                {
                    speciesStoic = (int)pSpeciesStoicMap[speciesName];
                    speciesStoic++;
                    pSpeciesStoicMap[speciesName] = speciesStoic;
                }
                //UPGRADE_TODO: Method 'java.util.HashMap.get' was converted to 'Hashtable.Item' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMapget_javalangObject'"
/*J
                MutableBoolean speciesDynamic = (MutableBoolean) pSpeciesDynamicMap[speciesName];
                if (null != speciesDynamic)
                {
                    if (speciesDynamic.booleanValue() == dynamic)
                    {
                        // everything is cool
                    }
                    else
                    {
                        throw new InvalidInputException("species " + speciesName + " is defined both as dynamic and boundary, for the same reaction");
                    }
                }
                else
                {
                    speciesDynamic = new MutableBoolean(dynamic);
                    pSpeciesDynamicMap[speciesName] = speciesDynamic;
                }
*/
                if (pSpeciesDynamicMap.ContainsKey(speciesName))
                {
                    if ((bool)pSpeciesDynamicMap[speciesName] == dynamic)
                    {
                        // everything is good
                    }
                    else
                        throw new InvalidInputException("species " + speciesName + " is defined both as dynamic and boundary, for the same reaction");
                }
                else
                {
                    pSpeciesDynamicMap.Add(speciesName, dynamic);
                }

                token = getNextToken(pTokenIter);
                if (pParticipantType.Equals(ReactionParticipant.Type.REACTANT) && token.mCode.Equals(Token.Code.HYPHEN))
                {
                    token = getNextToken(pTokenIter);
                    Debug.Assert(token.mCode.Equals(Token.Code.GREATER_THAN), "expected greater-than symbol");
                    break;
                }
                else if (token.mCode.Equals(Token.Code.PLUS))
                {
                    continue;
                }
                else if (pParticipantType.Equals(ReactionParticipant.Type.PRODUCT) && token.mCode.Equals(Token.Code.COMMA))
                {
                    break;
                }
                else
                {
                    throw new InvalidInputException("invalid token type encountered in reaction definition: \"" + token.ToString() + "\"");
                }
            }
        }

        private void  handleSpeciesDefinitions(Reaction pReaction, ReactionParticipant.Type pReactionParticipantType, Hashtable pSymbolMap, Hashtable pSpeciesStoicMap, Hashtable pSpeciesDynamicMap)
        {
            //UPGRADE_TODO: Method 'java.util.HashMap.keySet' was converted to 'SupportClass.HashSetSupport' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMapkeySet'"
            IEnumerator speciesIter = new SupportClass.HashSetSupport(pSpeciesStoicMap.Keys).GetEnumerator();

            while (speciesIter.MoveNext())
            {

                String speciesName = (String) speciesIter.Current;
                //UPGRADE_TODO: Method 'java.util.HashMap.get' was converted to 'Hashtable.Item' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMapget_javalangObject'"
/*J
                MutableBoolean speciesDynamic = (MutableBoolean) pSpeciesDynamicMap[speciesName];
                Debug.Assert(null != speciesDynamic, "expected to find non-null object for species: " + speciesName);

                bool dynamic = speciesDynamic.booleanValue();
*/
                bool dynamic = (bool)pSpeciesDynamicMap[speciesName];

                //UPGRADE_TODO: Method 'java.util.HashMap.get' was converted to 'Hashtable.Item' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMapget_javalangObject'"
/*J
                MutableInteger speciesStoic = (MutableInteger) pSpeciesStoicMap[speciesName];
                Debug.Assert(null != speciesStoic, "expected to find non-null object for species: " + speciesName);
                int stoichiometry = speciesStoic.Value;
*/
                int stoichiometry = (int)pSpeciesStoicMap[speciesName];

                //UPGRADE_TODO: Method 'java.util.HashMap.get' was converted to 'Hashtable.Item' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMapget_javalangObject'"
                //J SymbolValue speciesSymbolValue = (SymbolValue) pSymbolMap[speciesName];
                //J if (null == speciesSymbolValue)
                if (!pSymbolMap.ContainsKey(speciesName))
                {
                    throw new InvalidInputException("species \"" + speciesName + "\" was referenced in a reaction definition, but was not previously defined");
                }

                SymbolValue speciesSymbolValue = (SymbolValue)pSymbolMap[speciesName];
                Species species = null;

                if (!(speciesSymbolValue is Species))
                {
                    if (!speciesSymbolValue.GetType().BaseType.Equals(typeof(Object)))
                    {
                        throw new InvalidInputException("symbol: \"" + speciesName + "\" is already defined as a different (non-species) symbol of type \"" + speciesSymbolValue.GetType().FullName + "\"");
                    }
                    Compartment compartment = getDefaultCompartment(pSymbolMap);
                    species = new Species(speciesSymbolValue, compartment);
                    pSymbolMap[speciesName] = species;
                }
                else
                {
                    species = (Species) speciesSymbolValue;
                }

                pReaction.addSpecies(species, stoichiometry, dynamic, pReactionParticipantType);
            }
        }

        //J private void  handleStatementReaction(IEnumerator pTokenIter, Model pModel, Hashtable pSymbolMap, MutableInteger pNumReactions)
        private void  handleStatementReaction(IListIterator<Token> pTokenIter, Model pModel, Hashtable pSymbolMap, int pNumReactions)
        {
            Token token = null;

            bool hasName = false;
            bool hasReactants = false;

            // advance token iterator to reaction symbol "->"
            bool gotHyphen = false;

            while (pTokenIter.HasNext)
            {
                token = getNextToken(pTokenIter);
                if (token.mCode.Equals(Token.Code.HYPHEN))
                {
                    gotHyphen = true;
                    break;
                }
            }
            Debug.Assert(gotHyphen, "unable to locate the \"->\" reaction symbol within reaction statement");

            // back up to the beginning of the reaction statement
            //UPGRADE_ISSUE: Method 'java.util.ListIterator.hasPrevious' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javautilListIteratorhasPrevious'"
            while (pTokenIter.HasPrevious)
            {
                //UPGRADE_ISSUE: Method 'java.util.ListIterator.previous' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javautilListIteratorprevious'"
                token = pTokenIter.Previous;
                if (token.mCode.Equals(Token.Code.COMMA))
                {
                    hasName = true;
                }
                else if (token.mCode.Equals(Token.Code.PLUS))
                {
                    hasReactants = true;
                }
                else if (token.mCode.Equals(Token.Code.SYMBOL))
                {
                    if (!hasName)
                    {
                        hasReactants = true;
                    }
                }
                else if (token.mCode.Equals(Token.Code.DOLLAR))
                {
                    hasReactants = true;
                }
                else if (token.mCode.Equals(Token.Code.SEMICOLON))
                {
                    pTokenIter.GetNext();
                    break;
                }
                else if (token.mCode.Equals(Token.Code.BRACE_END))
                {
                    pTokenIter.GetNext();
                    break;
                }
            }

            String reactionName = null;

            if (hasName)
            {
                reactionName = obtainSymbolWithNamespace(pTokenIter, pSymbolMap);
                token = getNextToken(pTokenIter);
                if (!token.mCode.Equals(Token.Code.COMMA))
                {
                    throw new InvalidInputException("expected comma after reaction name token");
                }
            }
            else
            {
/*J
                int numReactions = pNumReactions.Value + 1;
                pNumReactions.Value = numReactions;
                reactionName = Model.INTERNAL_SYMBOL_PREFIX + "r" + numReactions;
*/
                pNumReactions++;
                reactionName = Model.INTERNAL_SYMBOL_PREFIX + "r" + pNumReactions;
            }

            Reaction reaction           = new Reaction(reactionName);
            Hashtable speciesStoicMap   = new Hashtable();
            Hashtable speciesDynamicMap = new Hashtable();

            if (hasReactants)
            {
                getReactionParticipants(pTokenIter, pSymbolMap, speciesStoicMap, speciesDynamicMap, ReactionParticipant.Type.REACTANT);
                handleSpeciesDefinitions(reaction, ReactionParticipant.Type.REACTANT, pSymbolMap, speciesStoicMap, speciesDynamicMap);
            }
            else
            {
                pTokenIter.GetNext();
                pTokenIter.GetNext();
            }

            token = getNextToken(pTokenIter);
            bool hasProducts = false;
            if (token.mCode.Equals(Token.Code.SYMBOL) || token.mCode.Equals(Token.Code.QUOTE))
            {
                hasProducts = true;
                //UPGRADE_ISSUE: Method 'java.util.ListIterator.previous' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javautilListIteratorprevious'"
                pTokenIter.GetPrevious();
            }
            else
            {
                if (!token.mCode.Equals(Token.Code.COMMA))
                {
                    throw new InvalidInputException("expected comma separator between reaction and rate; token is: " + token);
                }
            }

            speciesStoicMap.Clear();

            if (hasProducts)
            {
                getReactionParticipants(pTokenIter, pSymbolMap, speciesStoicMap, speciesDynamicMap, ReactionParticipant.Type.PRODUCT);
                handleSpeciesDefinitions(reaction, ReactionParticipant.Type.PRODUCT, pSymbolMap, speciesStoicMap, speciesDynamicMap);
            }

            if (!pTokenIter.HasNext)
            {
                throw new InvalidInputException("incomplete reaction definition; expected to find reaction rate specifier");
            }

            bool allowDeferred = true;
            Value rateValue = obtainValue(pTokenIter, pSymbolMap, allowDeferred);
            reaction.Rate = rateValue;

            //UPGRADE_TODO: Method 'java.util.HashMap.get' was converted to 'Hashtable.Item' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMapget_javalangObject'"
/*
            if (null != pSymbolMap[reactionName])
            {
                throw new InvalidInputException("already found a symbol defined with name: " + reactionName + "; cannot process reaction definition of same name");
            }
*/
            if (pSymbolMap.ContainsKey(reactionName))
                throw new InvalidInputException("already found a symbol defined with name: " + reactionName + "; cannot process reaction definition of same name");

            pSymbolMap[reactionName] = reaction;
            pModel.addReaction(reaction);

            Token nextToken = getNextToken(pTokenIter);
            if (nextToken.mCode.Equals(Token.Code.COMMA))
            {
                // check if next token is a symbol
                nextToken = getNextToken(pTokenIter);
                if (nextToken.mCode.Equals(Token.Code.SYMBOL))
                {
                    if (nextToken.mSymbol.Equals(REACTION_MODIFIER_STEPS))
                    {
                        handleMultistepReaction(reaction, pSymbolMap, pTokenIter);
                    }
                    else if (nextToken.mSymbol.Equals(REACTION_MODIFIER_DELAY))
                    {
                        handleDelayedReaction(reaction, pSymbolMap, pTokenIter);
                    }
                    else
                    {
                        throw new InvalidInputException("unknown reaction modifier symbol: " + nextToken.mSymbol + "; did you forget to include the reaction modifier \"" + REACTION_MODIFIER_STEPS + "\" or \"" + REACTION_MODIFIER_DELAY + "\"?");
                    }
                }
                else
                {
                    // For compatibility with early versions of Dizzy, allow for a number
                    // to be specified without a modifier; this is taken to represent
                    // the number of steps in the reaction
                    //UPGRADE_ISSUE: Method 'java.util.ListIterator.previous' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javautilListIteratorprevious'"
                    pTokenIter.GetPrevious();
                    handleMultistepReaction(reaction, pSymbolMap, pTokenIter);
                }
            }
            else
            {
                //UPGRADE_ISSUE: Method 'java.util.ListIterator.previous' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javautilListIteratorprevious'"
                pTokenIter.GetPrevious();
            }

            getEndOfStatement(pTokenIter);
        }


        private void handleDelayedReaction(Reaction pReaction, Hashtable pSymbolMap, IListIterator<Token> pTokenIter)
        {
            Value delayValue = obtainValue(pTokenIter, pSymbolMap, false);
            if (delayValue.IsExpression)
            {
                throw new InvalidInputException("reaction delay must be specified as a number, not a deferred-evaluation expression");
            }
            double delay = delayValue.getValue();
            if (delay < 0.0)
            {
                throw new InvalidInputException("reaction delay must be a nonnegative number");
            }
            pReaction.Delay = delay;
        }


        private void handleMultistepReaction(Reaction pReaction, Hashtable pSymbolMap, IListIterator<Token> pTokenIter)
        {
            bool allowDeferred = false;
            Value stepsValue = obtainValue(pTokenIter, pSymbolMap, allowDeferred);
            if (stepsValue.IsExpression)
            {
                throw new InvalidInputException("number of reaction steps must be specified as a number, not a deferred-evaluation expression");
            }
            //UPGRD_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
            int numSteps = (int) stepsValue.getValue();
            if (numSteps <= 0)
            {
                throw new InvalidInputException("invalid number of steps specified");
            }
            else if (numSteps > 1)
            {
                pReaction.NumSteps = numSteps;
            }
            else
            {
                // number of steps is exactly one; so there is nothing to do
            }
        }

        private void getEndOfStatement(IListIterator<Token> pTokenIter)
        {
            Token token = getNextToken(pTokenIter);
            if (!token.mCode.Equals(Token.Code.SEMICOLON))
            {
                throw new InvalidInputException("expected statement-ending semicolon; instead encountered token \"" + token + "\"");
            }
        }

        private String getQuotedString(IListIterator<Token> pTokenIter)
        {
            Token token = getNextToken(pTokenIter);
            if (!token.mCode.Equals(Token.Code.QUOTE))
            {
                throw new InvalidInputException("expected quote symbol");
            }

            token = getNextToken(pTokenIter);
            if (!token.mCode.Equals(Token.Code.SYMBOL))
            {
                throw new InvalidInputException("expected quoted string");
            }

            String string_Renamed = token.mSymbol;

            token = getNextToken(pTokenIter);

            Debug.Assert(token.mCode.Equals(Token.Code.QUOTE), "missing terminating quote");

            return (string_Renamed);
        }


        private void handleStatementModel(IListIterator<Token> pTokenIter, Model pModel, Hashtable pSymbolMap)
        {
            Token token = getNextToken(pTokenIter);
            Debug.Assert(token.mCode.Equals(Token.Code.POUNDSIGN), "where expected a pound sign, got an unexpected token \"" + token + "\"");

            token = getNextToken(pTokenIter);
            Debug.Assert(token.mCode.Equals(Token.Code.SYMBOL), "where expected a symbol token, got an unexpected token: " + token);
            Debug.Assert(token.mSymbol.Equals(STATEMENT_KEYWORD_MODEL), "where expected the model keyword, got an unexpected symbol token: " + token.mSymbol);

            if (null != mNamespace)
            {
                throw new InvalidInputException("it is illegal to define a model name inside a macro reference");
            }
            String modelName = obtainSymbol(pTokenIter, pSymbolMap);
            pModel.Name = modelName;
            getEndOfStatement(pTokenIter);
        }




        //J private void handleStatementMacroDefinition(IEnumerator pTokenIter, Model pModel, Hashtable pSymbolMap, MutableInteger pNumReactions)
        private void handleStatementMacroDefinition(IListIterator<Token> pTokenIter, Model pModel, Hashtable pSymbolMap, int pNumReactions)
        {
            Token token = getNextToken(pTokenIter);
            Debug.Assert(token.mCode.Equals(Token.Code.POUNDSIGN), "where expected a pound sign, got an unexpected token: " + token);

            token = getNextToken(pTokenIter);
            Debug.Assert(token.mCode.Equals(Token.Code.SYMBOL), "where expected a symbol token, got an unexpected token: " + token);
            Debug.Assert(token.mSymbol.Equals(STATEMENT_KEYWORD_DEFINE), "where expected the define keyword, got an unexpected symbol token: " + token.mSymbol);

            String macroName = obtainSymbol(pTokenIter, pSymbolMap);
            //UPGRADE_TODO: Method 'java.util.HashMap.get' was converted to 'Hashtable.Item' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMapget_javalangObject'"
/*
            if (null != pSymbolMap[macroName])
            {
                throw new InvalidInputException("symbol " + macroName + " was defined more than once in the same model");
            }
*/
            if (pSymbolMap.ContainsKey(macroName))
                throw new InvalidInputException("symbol " + macroName + " was defined more than once in the same model");

            if (- 1 != macroName.IndexOf(Model.NAMESPACE_IDENTIFIER))
            {
                throw new InvalidInputException("macro name may not contain the namespace identifier \"" + Model.NAMESPACE_IDENTIFIER + "\"; macro name is: " + macroName);
            }

            ArrayList externalSymbolsList = new ArrayList();


            if (!pTokenIter.HasNext)
            {
                throw new InvalidInputException("expected parenthesis or curly brace after macro definition statement");
            }

            token = getNextToken(pTokenIter);

            if (token.mCode.Equals(Token.Code.PAREN_BEGIN))
            {
                bool expectSymbol = true;
                bool gotEndParen = false;

                while (pTokenIter.HasNext)
                {
                    if (expectSymbol)
                    {
                        String symbol = obtainSymbol(pTokenIter, pSymbolMap);
                        externalSymbolsList.Add(symbol);
                        expectSymbol = false;
                    }
                    else
                    {
                        token = getNextToken(pTokenIter);
                        if (token.mCode.Equals(Token.Code.PAREN_END))
                        {
                            gotEndParen = true;
                            break;
                        }
                        else if (token.mCode.Equals(Token.Code.SEMICOLON))
                        {
                            throw new InvalidInputException("end of statement token encountered inside macro definition symbol list");
                        }
                        else if (token.mCode.Equals(Token.Code.COMMA))
                        {
                            if (expectSymbol)
                            {
                                throw new InvalidInputException("comma encountered unexpectedly in macro definition symbol list");
                            }
                            expectSymbol = true;
                        }
                        else
                        {
                            throw new InvalidInputException("unexpected token encountered in macro definition symbol list \"" + token + "\"");
                        }
                    }
                }
                if (!gotEndParen)
                {
                    throw new InvalidInputException("failed to find end parenthesis");
                }
            }
            else if (token.mCode.Equals(Token.Code.BRACE_BEGIN))
            {
                //UPGRADE_ISSUE: Method 'java.util.ListIterator.previous' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javautilListIteratorprevious'"
                pTokenIter.GetPrevious();
            }
            else
            {
                throw new InvalidInputException("unknown token found in macro definition statement");
            }


            if (!pTokenIter.HasNext)
            {
                throw new InvalidInputException("expected curly brace token, instead found nothing");
            }

            token = getNextToken(pTokenIter);

            if (!token.mCode.Equals(Token.Code.BRACE_BEGIN))
            {
                throw new InvalidInputException("expected curly brace token in macro definition statement");
            }

            Macro macro = new Macro(macroName);
            macro.mExternalSymbols = externalSymbolsList;


            List<Token> tokenList = new List<Token>();

            bool gotEndBrace = false;
            Token prevToken = null;
            int braceCtr = 1;

            while (pTokenIter.HasNext)
            {
                prevToken = token;
                token = getNextToken(pTokenIter);
                if (token.mCode.Equals(Token.Code.BRACE_END))
                {
                    --braceCtr;
                    if (0 == braceCtr)
                    {
                        gotEndBrace = true;
                        break;
                    }
                }
                else if (token.mCode.Equals(Token.Code.BRACE_BEGIN))
                {
                    ++braceCtr;
                }
                else if (null != prevToken && prevToken.mCode.Equals(Token.Code.POUNDSIGN) && token.mCode.Equals(Token.Code.SYMBOL) && token.mSymbol.Equals(STATEMENT_KEYWORD_DEFINE))
                {
                    throw new InvalidInputException("it is illegal to embed a macro definition inside a macro definition");
                }
                tokenList.Add(token);
            }
            if (!gotEndBrace)
            {
                throw new InvalidInputException("failed to find end brace");
            }

            macro.mTokenList = tokenList;
            pSymbolMap[macroName] = macro;
        }


        //J private void  handleStatementInclude(IEnumerator pTokenIter, Model pModel, Hashtable pSymbolMap, MutableInteger pNumReactions, IncludeHandler pIncludeHandler)
        private void handleStatementInclude(IListIterator<Token> pTokenIter, Model pModel, Hashtable pSymbolMap, int pNumReactions, IncludeHandler pIncludeHandler)
        {
            if (null == pIncludeHandler)
            {
                throw new InvalidInputException("encountered an include statement; include statements are not allowed in this context");
            }

            Token token = getNextToken(pTokenIter);
            Debug.Assert(token.mCode.Equals(Token.Code.POUNDSIGN), "where expected a pound sign, got an unexpected token: " + token);

            token = getNextToken(pTokenIter);
            Debug.Assert(token.mCode.Equals(Token.Code.SYMBOL), "where expected a symbol token, got an unexpected token: " + token);
            Debug.Assert(token.mSymbol.Equals(STATEMENT_KEYWORD_INCLUDE), "where expected the include keyword, got an unexpected symbol token: " + token.mSymbol);

            String fileName = getQuotedString(pTokenIter);

            getEndOfStatement(pTokenIter);

            StreamReader bufferedReader = null;

            try
            {
                //J bufferedReader = pIncludeHandler.openReaderForIncludeFile(fileName, sCharset);
                bufferedReader = pIncludeHandler.openReaderForIncludeFile(fileName);
                if (null != bufferedReader)
                {
                    bool insideInclude = true;
                    parseModelDefinition(bufferedReader, pModel, pIncludeHandler, pSymbolMap, pNumReactions, insideInclude);
                }
            }
            catch (IOException e)
            {
                throw new InvalidInputException("error reading include file \"" + fileName + "\"", e);
            }
            catch (InvalidInputException e)
            {
                //UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Throwable.getMessage' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
                StringBuilder sb = new StringBuilder(e.Message);
                sb.Append(" in file \"" + fileName + "\"; included");
                throw new InvalidInputException(sb.ToString(), e);
            }
        }

        private String getErrorMessage(Exception e, int pLineCtr)
        {
            //UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Throwable.getMessage' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
            StringBuilder message = new StringBuilder(e.Message);
            message.Append(" at line " + pLineCtr);
            return (message.ToString());
        }

        private void synchIterators(IListIterator<Token> master, IListIterator<Token> slave)
        {
            //UPGRADE_ISSUE: Method 'java.util.ListIterator.nextIndex' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javautilListIteratornextIndex'"
            while (slave.NextIndex < master.NextIndex)
            {
                slave.GetNext();
            }
        }

        //J private void executeStatementBlock(IList pTokens, Model pModel, IncludeHandler pIncludeHandler, Hashtable pSymbolMap, MutableInteger pNumReactions)
        private void  executeStatementBlock(IList<Token> pTokens, Model pModel, IncludeHandler pIncludeHandler, Hashtable pSymbolMap, int pNumReactions)
        {
            IListIterator<Token> tokenIter     = new ListIterator<Token>(pTokens);
            IListIterator<Token> tokenIterExec = new ListIterator<Token>(pTokens);

            Token prevToken = null;

            Token token = null;
            int lineCtr = 0;

            try
            {
                while (tokenIter.HasNext)
                {
                    token = tokenIter.Next;
                    Debug.Assert(null != token, "unexpected null token");
                    lineCtr = token.mLine;

                    if (token.mCode.Equals(Token.Code.EQUALS))
                    {
                        // if an "=" token is detected, this is definitely a symbol definition statement
                        handleStatementSymbolDefinition(tokenIterExec, pModel, pSymbolMap);
                        synchIterators(tokenIterExec, tokenIter);
                    }
                    else if (token.mCode.Equals(Token.Code.ATSIGN))
                    {
                        // if an "@" token is detected, this is definitely a compartment association statement
                        handleStatementAssociate(tokenIterExec, pModel, pSymbolMap);
                        synchIterators(tokenIterExec, tokenIter);
                    }
                    else if (token.mCode.Equals(Token.Code.GREATER_THAN))
                    {
                        // if a ">" token immediately follows a "-" token, this is definitely a reaction statement
                        if (null != prevToken)
                        {
                            if (prevToken.mCode.Equals(Token.Code.HYPHEN))
                            {
                                handleStatementReaction(tokenIterExec, pModel, pSymbolMap, pNumReactions);
                                synchIterators(tokenIterExec, tokenIter);
                            }
                            else
                            {
                                throw new InvalidInputException("encountered \">\" unexpectedly");
                            }
                        }
                        else
                        {
                            throw new InvalidInputException("encountered \">\" with no preceding hyphen and outside of an expression context");
                        }
                    }
                    else if (token.mCode.Equals(Token.Code.SYMBOL) && null != prevToken && prevToken.mCode.Equals(Token.Code.POUNDSIGN))
                    {
                        Debug.Assert(null != token.mSymbol, "null symbol string found in symbol token");
                        if (token.mSymbol.Equals(STATEMENT_KEYWORD_INCLUDE))
                        {
                            handleStatementInclude(tokenIterExec, pModel, pSymbolMap, pNumReactions, pIncludeHandler);
                            synchIterators(tokenIterExec, tokenIter);
                        }
                        else if (token.mSymbol.Equals(STATEMENT_KEYWORD_MODEL))
                        {
                            handleStatementModel(tokenIterExec, pModel, pSymbolMap);
                            synchIterators(tokenIterExec, tokenIter);
                        }
                        else if (token.mSymbol.Equals(STATEMENT_KEYWORD_DEFINE))
                        {
                            handleStatementMacroDefinition(tokenIterExec, pModel, pSymbolMap, pNumReactions);
                            synchIterators(tokenIterExec, tokenIter);
                        }
                        else if (token.mSymbol.Equals(STATEMENT_KEYWORD_REF))
                        {
                            handleStatementMacroReference(tokenIterExec, pModel, pIncludeHandler, pSymbolMap, pNumReactions);
                            synchIterators(tokenIterExec, tokenIter);
                        }
                        else
                        {
                            throw new InvalidInputException("unknown command keyword \"" + token.mSymbol + "\"");
                        }
                    }
                    else if (token.mCode.Equals(Token.Code.PAREN_BEGIN))
                    {
                        if (null != prevToken)
                        {
                            if (prevToken.mCode.Equals(Token.Code.SYMBOL))
                            {
                                if (prevToken.mSymbol.Equals(KEYWORD_LOOP))
                                {
                                    handleStatementLoop(tokenIterExec, pModel, pIncludeHandler, pSymbolMap, pNumReactions);
                                    synchIterators(tokenIterExec, tokenIter);
                                }
                                else
                                {
                                    throw new InvalidInputException("parenthesis following unknown keyword: " + prevToken.mSymbol);
                                }
                            }
                            else
                            {
                                throw new InvalidInputException("parenthesis following unexpected token \"" + prevToken.mCode + "\"");
                            }
                        }
                        else
                        {
                            throw new InvalidInputException("statement began with a parenthesis");
                        }
                    }
                    else if (token.mCode.Equals(Token.Code.SEMICOLON))
                    {
                        throw new InvalidInputException("unknown statement type");
                    }
                    prevToken = token;
                }
            }
            catch (DataNotFoundException e)
            {
                throw new InvalidInputException(getErrorMessage(e, lineCtr), e);
            }
            catch (InvalidInputException e)
            {
                throw new InvalidInputException(getErrorMessage(e, lineCtr), e);
            }
        }


        //J private void handleStatementLoop(IEnumerator pTokenIter, Model pModel, IncludeHandler pIncludeHandler, Hashtable pSymbolMap, MutableInteger pNumReactions)
        private void handleStatementLoop(IListIterator<Token> pTokenIter, Model pModel, IncludeHandler pIncludeHandler, Hashtable pSymbolMap, int pNumReactions)
        {
            Token token = getNextToken(pTokenIter);
            Debug.Assert(token.mCode.Equals(Token.Code.SYMBOL), "expected a symbol token, unexpectedly got token: " + token);
            Debug.Assert(token.mSymbol.Equals(KEYWORD_LOOP), "expected loop keyword; unexpectedly got symbol: " + token.mSymbol);

            token = getNextToken(pTokenIter);
            if (!token.mCode.Equals(Token.Code.PAREN_BEGIN))
            {
                throw new InvalidInputException("unexpected token found when expected beginning parenthesis; token is \"" + token + "\"");
            }

            token = getNextToken(pTokenIter);
            if (!token.mCode.Equals(Token.Code.SYMBOL))
            {
                throw new InvalidInputException("unexpected token found when expected loop index symbol; token is \"" + token + "\"");
            }

            String loopIndexSymbolName = token.mSymbol;

            if (ReservedSymbolMapperChemCommandLanguage.isReservedSymbol(loopIndexSymbolName))
            {
                throw new InvalidInputException("cannot use a reserved symbol as a loop index; you used \"" + loopIndexSymbolName + "\"");
            }

            loopIndexSymbolName = addNamespaceToSymbol(loopIndexSymbolName, mNamespace);

            token = getNextToken(pTokenIter);

            if (!token.mCode.Equals(Token.Code.COMMA))
            {
                throw new InvalidInputException("unexpected token found when expected comma separator; token is \"" + token + "\"");
            }


            if (!pTokenIter.HasNext)
            {
                throw new InvalidInputException("missing loop starting value");
            }

            bool allowDeferred = false;
            Value startValueObj = obtainValue(pTokenIter, pSymbolMap, allowDeferred);

            //UPGRD_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
            int startValue = (int) (startValueObj.getValue());

            if (!pTokenIter.HasNext)
            {
                throw new InvalidInputException("missing loop ending value");
            }

            token = getNextToken(pTokenIter);
            if (!token.mCode.Equals(Token.Code.COMMA))
            {
                throw new InvalidInputException("expected a comma separating the start expression from the end expression, in a loop statement; instead found the token \"" + token.mCode + "\"");
            }

            bool allowTerminateOnParen = true;
            Value stopValueObj = obtainValue(pTokenIter, pSymbolMap, allowDeferred, allowTerminateOnParen);

            //UPGRD_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
            int endValue = (int) (stopValueObj.getValue());


            if (!pTokenIter.HasNext)
            {
                throw new InvalidInputException("missing paren at end of loop statement");
            }
            token = getNextToken(pTokenIter);
            if (!token.mCode.Equals(Token.Code.PAREN_END))
            {
                throw new InvalidInputException("expected a paren at the end of the loop statement; instead found the token \"" + token.mCode + "\"");
            }


            if (!pTokenIter.HasNext)
            {
                throw new InvalidInputException("missing curly brace after loop statement");
            }
            token = getNextToken(pTokenIter);
            if (!token.mCode.Equals(Token.Code.BRACE_BEGIN))
            {
                throw new InvalidInputException("expected a curly brace after the loop statement; instead found the token \"" + token.mCode + "\"");
            }

            //UPGRADE_TODO: Method 'java.util.HashMap.get' was converted to 'Hashtable.Item' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMapget_javalangObject'"
            LoopIndex loopIndexObj = null;
/*
            SymbolValue loopIndexSymbolValue = (SymbolValue) pSymbolMap[loopIndexSymbolName];
            if (null != loopIndexSymbolValue)
            {
                if (loopIndexSymbolValue is LoopIndex)
                {
                    loopIndexObj = (LoopIndex) loopIndexSymbolValue;
                }
                else
                {
                    throw new InvalidInputException("loop index \"" + loopIndexSymbolName + "\" has already been used as a symbol elsewhere");
                }
            }
            else
            {
                loopIndexSymbolValue = new LoopIndex(loopIndexSymbolName, startValue);
                pSymbolMap[loopIndexSymbolName] = loopIndexSymbolValue;
                loopIndexObj = (LoopIndex) loopIndexSymbolValue;
            }
*/
            if (pSymbolMap.ContainsKey(loopIndexSymbolName))
            {
                SymbolValue loopIndexSymbolValue = (SymbolValue)pSymbolMap[loopIndexSymbolName];

                if (loopIndexSymbolValue is LoopIndex)
                {
                    loopIndexObj = (LoopIndex)loopIndexSymbolValue;
                }
                else
                {
                    throw new InvalidInputException("loop index \"" + loopIndexSymbolName + "\" has already been used as a symbol elsewhere");
                }
            }
            else
            {
                SymbolValue loopIndexSymbolValue = new LoopIndex(loopIndexSymbolName, startValue);
                pSymbolMap[loopIndexSymbolName] = loopIndexSymbolValue;
                loopIndexObj = (LoopIndex)loopIndexSymbolValue;
            }

            IList<Token> subTokenList = new List<Token>();
            int braceCtr = 1;

            while (pTokenIter.HasNext)
            {
                token = getNextToken(pTokenIter);
                if (token.mCode.Equals(Token.Code.BRACE_BEGIN))
                {
                    braceCtr++;
                }
                else if (token.mCode.Equals(Token.Code.BRACE_END))
                {
                    braceCtr--;
                }

                if (braceCtr > 0)
                {
                    subTokenList.Add(token);
                }
                if (braceCtr == 0)
                {
                    break;
                }
            }

            if (braceCtr > 0)
            {
                throw new InvalidInputException("end-of-file encountered without matching end brace");
            }

            for (int loopIndex = startValue; loopIndex <= endValue; ++loopIndex)
            {
                loopIndexObj.setValue(loopIndex);
                try
                {
                    executeStatementBlock(subTokenList, pModel, pIncludeHandler, pSymbolMap, pNumReactions);
                }
                catch (InvalidInputException e)
                {
                    //UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Throwable.getMessage' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
                    StringBuilder messageBuf = new StringBuilder(e.Message);
                    messageBuf.Append(" in loop block beginning");
                    throw new InvalidInputException(messageBuf.ToString(), e);
                }
            }

            // nuke the loop index object
            pSymbolMap.Remove(loopIndexSymbolName);
        }


        //J private void handleStatementMacroReference(IEnumerator pTokenIter, Model pModel, IncludeHandler pIncludeHandler, Hashtable pSymbolMap, MutableInteger pNumReactions)
        private void handleStatementMacroReference(IListIterator<Token> pTokenIter, Model pModel, IncludeHandler pIncludeHandler, Hashtable pSymbolMap, int pNumReactions)
        {
            Token token = getNextToken(pTokenIter);
            Debug.Assert(token.mCode.Equals(Token.Code.POUNDSIGN), "where expected a pound sign, got an unexpected token: " + token);

            token = getNextToken(pTokenIter);
            Debug.Assert(token.mCode.Equals(Token.Code.SYMBOL), "where expected a symbol token, got an unexpected token: " + token);
            Debug.Assert(token.mSymbol.Equals(STATEMENT_KEYWORD_REF), "where expected the ref keyword, got an unexpected symbol token: " + token.mSymbol);

            String macroName = obtainSymbol(pTokenIter, pSymbolMap);

            //UPGRADE_TODO: Method 'java.util.HashMap.get' was converted to 'Hashtable.Item' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMapget_javalangObject'"
/*
            SymbolValue symbolValue = (SymbolValue) pSymbolMap[macroName];
            if (null == symbolValue)
            {
                throw new InvalidInputException("unknown macro referenced: " + macroName);
            }
*/
            if (!pSymbolMap.ContainsKey(macroName))
                throw new InvalidInputException("unknown macro referenced: " + macroName);

            SymbolValue symbolValue = (SymbolValue)pSymbolMap[macroName];

            if (!(symbolValue is Macro))
            {
                throw new InvalidInputException("symbol referenced is not a macro: " + macroName);
            }

            Macro macro = (Macro) symbolValue;

            String macroInstanceName = obtainSymbol(pTokenIter, pSymbolMap);

            ArrayList externalSymbolsList = new ArrayList();


            if (!pTokenIter.HasNext)
            {
                throw new InvalidInputException("expected parenthesis or curly brace after macro definition statement");
            }

            token = getNextToken(pTokenIter);

            if (token.mCode.Equals(Token.Code.PAREN_BEGIN))
            {

                bool expectSymbol = true;
                bool gotEndParen = false;
                bool allowDeferred = true;

                List<Token> tokenList = new List<Token>();

                while (pTokenIter.HasNext)
                {
                    if (expectSymbol)
                    {
                        // grab all symbols until the comma or paren end or semicolon
                        tokenList.Clear();
                        grabTokensToNextCommaOrParen(pTokenIter, tokenList, pSymbolMap);
                        if (tokenList.Count > 1)
                        {
                            // cannot be a single symbol; must be an expression
                            Value valueObj = obtainValue(new ListIterator<Token>(tokenList), pSymbolMap, allowDeferred);
                            externalSymbolsList.Add(valueObj);
                        }
                        else
                        {
                            Token firstToken = (Token) tokenList[0];
                            if (firstToken.mCode.Equals(Token.Code.SYMBOL))
                            {
                                // at this stage, we don't know if the string token is a literal numeric, or a symbol

                                // test to see if it is a numeric literal
                                Object symbolObj = null;
                                String symbolName = firstToken.mSymbol;
                                // try to parse as a number
                                double value_Renamed = 0.0;
                                try
                                {
                                    value_Renamed = Double.Parse(symbolName);
                                    Value symbolValueObj = new Value(value_Renamed);
                                    symbolObj = symbolValueObj;
                                }
                                catch (FormatException)
                                {
                                    // the string token is not a numeric literal; therefore, it is a symbol

                                    // scope the symbol name based on our namespace
                                    symbolObj = addNamespaceToSymbol(symbolName, mNamespace);

                                    // IMPORTANT:  if this macro reference is embedded within a macro
                                    // definition, it is possible that this symbol (after scoping with the
                                    // current namespace) is actually a "dummy symbol".  If it *is* a dummy
                                    // symbol, we use the dummy symbol's instance object.
                                    //UPGRADE_TODO: Method 'java.util.HashMap.get' was converted to 'Hashtable.Item' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMapget_javalangObject'"
                                    Object symbolTableEntry = pSymbolMap[symbolObj];
                                    if (null != symbolTableEntry && symbolTableEntry is DummySymbol)
                                    {
                                        DummySymbol dummySymbol = (DummySymbol) symbolTableEntry;
                                        symbolObj = dummySymbol.mInstanceSymbolObject;
                                        // in this case, just use the value of the dummysymbol
                                    }
                                }
                                externalSymbolsList.Add(symbolObj);
                            }
                            else
                            {
                                throw new InvalidInputException("unknown token in list of symbols: " + firstToken);
                            }
                        }

                        //                    String symbol = obtainSymbol(pTokenIter, pSymbolMap);
                        //                    externalSymbolsList.add(symbol);
                        expectSymbol = false;
                    }
                    else
                    {
                        token = getNextToken(pTokenIter);
                        if (token.mCode.Equals(Token.Code.PAREN_END))
                        {
                            gotEndParen = true;
                            break;
                        }
                        else if (token.mCode.Equals(Token.Code.SEMICOLON))
                        {
                            throw new InvalidInputException("end of statement token encountered inside parentheses");
                        }
                        else if (token.mCode.Equals(Token.Code.COMMA))
                        {
                            if (expectSymbol)
                            {
                                throw new InvalidInputException("comma encountered unexpectedly");
                            }
                            expectSymbol = true;
                        }
                        else
                        {
                            throw new InvalidInputException("unknown symbol encountered " + token);
                        }
                    }
                }
                if (!gotEndParen)
                {
                    throw new InvalidInputException("failed to find end parenthesis");
                }
            }
            else
            {
                //UPGRADE_ISSUE: Method 'java.util.ListIterator.previous' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javautilListIteratorprevious'"
                pTokenIter.GetPrevious();
            }

            getEndOfStatement(pTokenIter);

            if (externalSymbolsList.Count != macro.mExternalSymbols.Count)
            {
                throw new InvalidInputException("number of symbols is mismatched, for macro reference: " + macroName);
            }

            String oldNamespace = mNamespace;
            mNamespace = addNamespaceToSymbol(macroInstanceName, mNamespace);

            // add the dummy symbols to the global symbols map
            int numExtSym = externalSymbolsList.Count;
            for (int i = 0; i < numExtSym; ++i)
            {
                String dummySymbolName = (String) macro.mExternalSymbols[i];
                Debug.Assert(null != dummySymbolName, "unexpected null array element");


                // add namespace to the dummy symbol
                String translatedDummySymbolName = addNamespaceToSymbol(dummySymbolName, mNamespace);
                //J Debug.Assert(null == pSymbolMap.get(dummySymbolName), "unexpectedly found dummy symbol in global symbol table: " + dummySymbolName);
                Debug.Assert(!pSymbolMap.ContainsKey(dummySymbolName), "unexpectedly found dummy symbol in global symbol table: " + dummySymbolName);

                Object extSymValueObj = externalSymbolsList[i];
                Debug.Assert(null != extSymValueObj, "unexpected null array element");

                DummySymbol dummySymbol = new DummySymbol(translatedDummySymbolName, extSymValueObj);
                pSymbolMap[translatedDummySymbolName] = dummySymbol;
            }

            try
            {
                executeStatementBlock(macro.mTokenList, pModel, pIncludeHandler, pSymbolMap, pNumReactions);
            }
            catch (InvalidInputException e)
            {
                //UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Throwable.getMessage' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
                StringBuilder messageBuffer = new StringBuilder(e.Message);
                messageBuffer.Append(" in macro referenced");
                throw new InvalidInputException(messageBuffer.ToString(), e);
            }

            // remove the dummy symbols from the global symbols map
            for (int i = 0; i < numExtSym; ++i)
            {
                String extSymDummy = (String) macro.mExternalSymbols[i];
                extSymDummy = addNamespaceToSymbol(extSymDummy, mNamespace);
                pSymbolMap.Remove(extSymDummy);
            }

            mNamespace = oldNamespace;
        }


        //J private void tokenizeAndExecuteStatementBuffer(StringBuilder pStatementBuffer, IList pTokenList, Model pModel, IncludeHandler pIncludeHandler, Hashtable pSymbolMap, MutableInteger pNumReactions, int pLineNumber)
        private void  tokenizeAndExecuteStatementBuffer(StringBuilder pStatementBuffer, IList<Token> pTokenList, Model pModel, IncludeHandler pIncludeHandler, Hashtable pSymbolMap, int pNumReactions, int pLineNumber)
        {
            String statement = pStatementBuffer.ToString();

            tokenizeStatement(statement, pTokenList, pLineNumber);

            pStatementBuffer.Remove(0, statement.Length - 0);

            executeStatementBlock(pTokenList, pModel, pIncludeHandler, pSymbolMap, pNumReactions);

            pTokenList.Clear();
        }


        //J private void parseModelDefinition(StreamReader pInputReader, Model pModel, IncludeHandler pIncludeHandler, Hashtable pSymbolMap, MutableInteger pNumReactions, bool pInsideInclude)
        private void  parseModelDefinition(StreamReader pInputReader, Model pModel, IncludeHandler pIncludeHandler, Hashtable pSymbolMap, int pNumReactions, bool pInsideInclude)
        {
            SupportClass.StreamTokenizerSupport streamTokenizer = new SupportClass.StreamTokenizerSupport(pInputReader);

            streamTokenizer.SlashSlashComments(true);
            streamTokenizer.SlashStarComments(true);
            streamTokenizer.LowerCaseMode(false);

            // our quote character is the "double-quote" mark,
            streamTokenizer.QuoteChar('\"');

            // we want to preserve newlines
            streamTokenizer.EOLIsSignificant(true);

            // we want to preserve whitespace
            streamTokenizer.OrdinaryChars(' ', ' ');
            streamTokenizer.OrdinaryChars('\t', '\t');

            // disable parsing of numbers
            streamTokenizer.OrdinaryChars('0', '9');
            streamTokenizer.OrdinaryChars('.', '.');
            streamTokenizer.OrdinaryChars('-', '-');

            // disable interpretation of a single slash character as a comment
            streamTokenizer.OrdinaryChars('/', '/');

            // if we are inside an include file, the model elements have already been initialized
            if (!pInsideInclude)
            {
                processDefaultModelElements(pSymbolMap);
            }

            int lineCtr = 1;
            StringBuilder statementBuffer = new StringBuilder();

            IList<Token> tokenList = new List<Token>();

            int braceLevel = 0;
            while (true)
            {
                bool executeStatement = false;
                int tokenType = streamTokenizer.NextToken();
                if (SupportClass.StreamTokenizerSupport.TT_EOF == tokenType)
                {
                    break;
                }
                if (tokenType == '\"')
                {
                    String quotedString = streamTokenizer.sval;
                    statementBuffer.Append("\"" + quotedString + "\"");
                }
                else if (tokenType == SupportClass.StreamTokenizerSupport.TT_EOL)
                {
                    statementBuffer.Append("\n");
                }
                else if (tokenType == '{')
                {
                    braceLevel++;
                    statementBuffer.Append("{");
                }
                else if (tokenType == '}')
                {
                    if (0 == braceLevel)
                    {
                        throw new InvalidInputException("mismatched braces, encountered \"}\" brace without matching \"{\" brace previously");
                    }
                    braceLevel--;
                    statementBuffer.Append("}");
                    if (0 == braceLevel)
                    {
                        executeStatement = true;
                    }
                }
                else if (tokenType == ';')
                {
                    statementBuffer.Append(";");
                    if (0 == braceLevel)
                    {
                        executeStatement = true;
                    }
                }
                else if (tokenType == SupportClass.StreamTokenizerSupport.TT_WORD)
                {
                    statementBuffer.Append(streamTokenizer.sval);
                }
                else if (tokenType == '\t')
                {
                    statementBuffer.Append(" ");
                }
                else if (tokenType == SupportClass.StreamTokenizerSupport.TT_NUMBER)
                {
                    double value_Renamed = streamTokenizer.nval;
                    statementBuffer.Append(value_Renamed);
                }
                else
                {
                    //J statementBuffer.Append(Character.toString((char)tokenType));
                    statementBuffer.Append((char)tokenType);
                }

                if (executeStatement)
                {
                    tokenizeAndExecuteStatementBuffer(statementBuffer, tokenList, pModel, pIncludeHandler, pSymbolMap, pNumReactions, lineCtr);
                    lineCtr = streamTokenizer.Lineno();
                }
            }

            if (statementBuffer.ToString().Trim().Length != 0)
            {
                throw new InvalidInputException("model definition file ended without a statement-ending token (semicolon); at line " + lineCtr + " of model definition file");
            }

            if (!pInsideInclude)
            {
                defineParameters(pSymbolMap, pModel);
            }
        }



        public virtual Model buildModel(Stream pInputStream, IncludeHandler pIncludeHandler)
        {
            Model model = new Model();
            model.Name = DEFAULT_MODEL_NAME;
            model.ReservedSymbolMapper = new ReservedSymbolMapperChemCommandLanguage();

            Hashtable symbolMap = new Hashtable();
            //J MutableInteger numReactions = new MutableInteger(0);
            int numReactions = 0;
            mNamespace = null;
            bool insideInclude = false;
            StreamReader bufferedReader = getBufferedReader(pInputStream);
            parseModelDefinition(bufferedReader, model, pIncludeHandler, symbolMap, numReactions, insideInclude);
            return (model);
        }

        public virtual void  writeModel(String pModelText, Stream pOutputStream)
        {
/*J
            StreamWriter outputStreamWriter = new OutputStreamWriter(pOutputStream);
            StreamWriter bufferedWriter = new StreamWriter(outputStreamWriter.BaseStream, outputStreamWriter.Encoding);
            StreamWriter printWriter = new StreamWriter(bufferedWriter.BaseStream, bufferedWriter.Encoding);
            printWriter.Write(pModelText);
            printWriter.Flush();
*/
            using (StreamWriter writer = new StreamWriter(pOutputStream))
            {
                writer.Write(pModelText);
                writer.Flush();
            }
        }

        public virtual StreamReader getBufferedReader(Stream pInputStream)
        {
/*
            StreamReader inputStreamReader = new InputStreamReader(pInputStream);
            //UPGRADE_TODO: The differences in the expected value  of parameters for constructor 'java.io.BufferedReader.BufferedReader'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
            StreamReader bufferedReader = new StreamReader(inputStreamReader.BaseStream, inputStreamReader.CurrentEncoding);
            return bufferedReader;
*/
            return new StreamReader(pInputStream);
        }

        public virtual String readModel(Stream pInputStream)
        {
/*J
            StreamReader inputStreamReader = new InputStreamReader(pInputStream);
            //UPGRADE_TODO: The differences in the expected value  of parameters for constructor 'java.io.BufferedReader.BufferedReader'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
            StreamReader bufferedReader = new StreamReader(inputStreamReader.BaseStream, inputStreamReader.CurrentEncoding);
*/
            StringBuilder stringBuffer = new StringBuilder();
            using (StreamReader bufferedReader = new StreamReader(pInputStream))
            {
                String line = null;
                while (null != (line = bufferedReader.ReadLine()))
                {
                    stringBuffer.Append(line + "\n");
                }
            }
            return stringBuffer.ToString();
        }


        public static bool isValidSymbol(String pSymbolName)
        {
            //J return (VALID_SYMBOL_PATTERN.matcher(pSymbolName).matches());
            return VALID_SYMBOL_PATTERN.IsMatch(pSymbolName);
        }

/*J
        static ModelBuilderCommandLanguage()
        {
            {
                sCharset = Charset.forName(REQUIRED_CHAR_SET);
            }
        }
*/
    }
}
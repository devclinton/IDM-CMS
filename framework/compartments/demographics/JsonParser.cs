using System;
using System.Globalization;
using System.Linq;
using System.Text;
using compartments.demographics.elements;
using compartments.demographics.interfaces;

namespace compartments.demographics
{
    public delegate void ElementParsedHandler(object sender, string name, JsonElement element);

    public class JsonParser
    {
        private readonly ICharSource _source;

        public static JsonParser CreateJsonParser(ICharSource source)
        {
            var parser = new JsonParser(source);
            return parser;
        }

        protected JsonParser(ICharSource source)
        {
            _source = source;
        }

        public event ElementParsedHandler ElementParsed;

        protected void OnElementParsed(string name, JsonElement element)
        {
            ElementParsedHandler handler = ElementParsed;
            if (handler != null) handler(this, name, element);
        }

        public ICharSource CharSource { get { return _source; } }

        public JsonElement ParseElement()
        {
            JsonElement element;
            Terminate = false;
            SkipWhitespace();

            switch (_source.Current)
            {
                case '{':
                    element = ParseObject();
                    break;

                case '[':
                    element = ParseArray();
                    break;

                default:
                    throw new Exception("Expecting object or array.");
            }

            OnElementParsed(null, element);

            return element;
        }

        public bool Terminate { get; set; }

        private JsonElement ParseArray()
        {
            JsonArray array = null;

            if (_source.Current == '[')
            {
                array = new JsonArray();
                _source.Next();
                SkipWhitespace();

                if (_source.Current != ']')
                {
                    bool done = false;

                    while (!done)
                    {
                        JsonElement value = ParseValue();
                        value.Parent = array;
                        array.Add(value);
                        OnElementParsed(null, value);
                        if (Terminate) return array;
                        SkipWhitespace();

                        switch (_source.Next())
                        {
                            case ',':
                                // Gather another value
                                break;

                            case ']':
                                done = true;
                                break;

                            default:
                                throw new Exception("Expected another value of end of array.");
                        }
                    }
                }
                else
                {
                    _source.Next();  // Consume closing ']'.
                }
            }

            return array;
        }

        private void SkipWhitespace()
        {
            while (char.IsWhiteSpace(_source.Current))
            {
                _source.Next();
            }
        }

        private JsonElement ParseObject()
        {
            JsonObject obj = null;

            SkipWhitespace();
            if (_source.Current == '{')
            {
                obj = new JsonObject();
                _source.Next();
                SkipWhitespace();

                if (_source.Current != '}')
                {
                    bool done = false;

                    while (!done)
                    {
                        SkipWhitespace();

                        JsonString name;
                        switch (_source.Current)
                        {
                            case '"':
                                name = ParseString();
                                break;

                            default:
                                throw new Exception("Expected string of string-value pair.");
                        }

                        SkipWhitespace();
                        if (_source.Current == ':')
                        {
                            _source.Next();  // Consume ':'
                            JsonElement value = ParseValue();
                            value.Parent = obj;
                            obj.Add(name.Value, value);
                            OnElementParsed(name.Value, value);
                            if (Terminate) return obj;
                            SkipWhitespace();

                            switch (_source.Next())
                            {
                                case ',':
                                    // Gather another name-value pair
                                    break;

                                case '}':
                                    done = true;
                                    break;

                                default:
                                    throw new Exception("Expected another name-value pair or end of object.");
                            }
                        }
                        else
                        {
                            throw new Exception("Expected ':' of name-value pair.");
                        }
                    }
                }
                else
                {
                    _source.Next();  // Consume the closing '}'.
                }
            }

            return obj;
        }

        private JsonElement ParseValue()
        {
            JsonElement element;

            SkipWhitespace();
            char peek;
            switch (peek = _source.Current)
            {
                case '"':
                    element = ParseString();
                    break;

                case '{':
                    element = ParseObject();
                    break;

                case '[':
                    element = ParseArray();
                    break;

                case 't':
                case 'f':
                case 'n':
                    element = ParseKeyword();
                    break;

                default:
                    if ("-0123456789".IndexOf(peek) >= 0)
                    {
                        element = ParseNumber();
                    }
                    else
                    {
                        throw new Exception("Unknown value in JSON.");
                    }
                    break;
            }

            return element;
        }

        private JsonElement ParseNumber()
        {
            JsonNumber number;
            var sb = new StringBuilder();

            while ("-+01234567890.eE".IndexOf(_source.Current) >= 0)
            {
                sb.Append(_source.Next());
            }

            double value;
            if (Double.TryParse(sb.ToString(), NumberStyles.AllowExponent | NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, null, out value))
            {
                number = new JsonNumber { Value = value };
            }
            else
            {
                throw new Exception("Invalid numeric format.");
            }

            return number;
        }

        private JsonElement ParseKeyword()
        {
            JsonElement element;
            string test;

            switch (_source.Current)
            {
                case 't':
                    test = "true";
                    element = new JsonBool { Value = true };
                    break;

                case 'f':
                    test = "false";
                    element = new JsonBool { Value = false };
                    break;

                case 'n':
                    test = "null";
                    element = new JsonNull();
                    break;

                default:
                    throw new Exception("Unknown keyword in JSON.");
            }

            // Compares the subsequent values from _source against the matching
            // character of the test string.
            if (test.Any(t => _source.Next() != t))
            {
                throw new Exception("Unknown keyword in JSON.");
            }

            return element;
        }

        private JsonString ParseString()
        {
            JsonString str = null;

            SkipWhitespace();
            if (_source.Next() == '"')
            {
                str = new JsonString();
                bool done = false;
                var sb = new StringBuilder();

                while (!done)
                {
                    char next;
                    switch (next = _source.Next())
                    {
                        default:
                            sb.Append(next);
                            break;

                        case '\\':
                            switch (next = _source.Next())
                            {
                                case '"':
                                case '\\':
                                case '/':
                                    sb.Append(next);
                                    break;

                                case 'b':
                                    sb.Append('\b');
                                    break;

                                case 'f':
                                    sb.Append('\f');
                                    break;

                                case 'n':
                                    sb.Append('\n');
                                    break;

                                case 'r':
                                    sb.Append('\r');
                                    break;

                                case 't':
                                    sb.Append('\t');
                                    break;

                                case 'u':
                                    {
                                        int hex = 0;
                                        for (int i = 0; i < 4; i++)
                                        {
                                            next = char.ToUpper(_source.Next());
                                            int index;
                                            if ((index = "0123456789ABCDEF".IndexOf(next)) >= 0)
                                            {
                                                hex <<= 4;
                                                hex += index;
                                            }
                                            else
                                            {
                                                throw new Exception("Invalid hex digit in unicode escape.");
                                            }
                                        }
                                        sb.Append((char) hex);
                                    }
                                    break;

                                default:
                                    throw new Exception("Unknown escape in string.");
                            }
                            break;

                        case '"':
                            str.Value = sb.ToString();
                            done = true;
                            break;
                    }
                }
            }

            return str;
        }
    }
}

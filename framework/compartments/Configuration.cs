using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace compartments
{
    public class Configuration
    {
        public class Parameter
        {
            private readonly JToken _token;

            public Parameter(JToken token)
            {
                _token = token;
            }

            public Parameter this[int index]
            {
                get
                {
                    if (_token.Type == JTokenType.Array)
                        return new Parameter(_token[index]);
                    
                    throw new ArgumentException("Parameter '" + ((JProperty)_token.Parent).Name + "' is not an array.");
                }
            }

            public static implicit operator bool(Parameter p)
            {
                if (p._token.Type == JTokenType.Boolean)
                    return (bool)p._token;
                
                throw new ArgumentException("Parameter '" + ((JProperty)p._token.Parent).Name + "' is not a boolean value.");
            }

            public static implicit operator int(Parameter p)
            {
                if (p._token.Type == JTokenType.Integer)
                    return (int)p._token;
                
                if (p._token.Type == JTokenType.Float)
                    return (int)(float)p._token;

                throw new ArgumentException("Parameter '" + ((JProperty)p._token.Parent).Name + "' is not numeric.");
            }

            public static implicit operator float(Parameter p)
            {
                if (p._token.Type == JTokenType.Float ||
                    p._token.Type == JTokenType.Integer)
                    return (float)p._token;

                throw new ArgumentException("Parameter '" + ((JProperty)p._token.Parent).Name + "' is not numeric.");
            }

            public double AsDouble()
            {
                return (float)this;
            }

            public static implicit operator string(Parameter p)
            {
                if (p._token.Type == JTokenType.String)
                    return (string)p._token;

                throw new ArgumentException("Parameter '" + ((JProperty)p._token.Parent).Name + "' is not a string.");
            }

            public static implicit operator List<Parameter>(Parameter p)
            {
                if (p._token.Type == JTokenType.Array)
                {
                    var list = new List<Parameter>();
                    foreach (JToken token in p._token)
                        list.Add(new Parameter(token));
                    return list;
                }

                throw new ArgumentException("Parameter '" + ((JProperty)p._token.Parent).Name + "' is not an array.");
            }

            public static implicit operator Configuration(Parameter p)
            {
                if (p._token.Type == JTokenType.Object)
                    return new Configuration(p._token as JObject);

                throw new ArgumentException("Parameter '" + ((JProperty)p._token.Parent).Name + "' is not a configuration node.");
            }

            public Configuration AsConfiguration()
            {
                return this;
            }
        }

        private static readonly Configuration EmptyConfig = new Configuration((JObject)JsonConvert.DeserializeObject("{}"));
        private static Configuration _config = EmptyConfig;
        private JObject _root; // = null;

        public static Configuration CurrentConfiguration
        {
            get
            {
                return _config;
            }
            set {
                _config = value ?? EmptyConfig;
            }
        }

        protected Configuration() {}

        public Configuration(string filename)
        {
            if (filename != string.Empty)
            {
                try
                {
                    using (StreamReader reader = File.OpenText(filename))
                    {
                        string text = reader.ReadToEnd();
                        try
                        {
                            _root = (JObject)JsonConvert.DeserializeObject(text);
                        }
                        catch (Exception e)
                        {
                            Console.Error.WriteLine(e.Message);
                            throw;
                        }
                    }
                }
                catch (FileNotFoundException fnf)
                {
                    Console.Error.WriteLine(fnf.Message);
                    throw;
                }
            }
        }

        public static Configuration ConfigurationFromString(string jsonData)
        {
            Configuration newConfig; // = null;

            try
            {
                var json = (JObject)JsonConvert.DeserializeObject(jsonData);
                newConfig = new Configuration { _root = json };
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                throw;
            }
            
            return newConfig;
        }

        protected Configuration(JObject node)
        {
            _root = node;
        }

        protected JToken GetParameter(string name)
        {
            string[] split = name.Split(new[] { '.', '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
            JToken token = _root[split[0]];
            for (int i = 1; i < split.Length; i++)
                if (token != null)
                    token = token[split[i]];

            return token;
        }

        public bool HasParameter(string name)
        {
            return (GetParameter(name) != null);
        }

        public Parameter Root
        {
            get { return new Parameter(_root); }
        }

        public Parameter this[string key]
        {
            get
            {
                JToken token = GetParameter(key);
                if (token == null)
                    throw new ArgumentException("Parameter '" + key + "' not present in configuration.");

                return new Parameter(token);
            }
        }

        public bool GetParameterWithDefault(string name, bool def)
        {
            JToken token = GetParameter(name);
            return (token != null) ? (bool)token : def;
        }

        public int GetParameterWithDefault(string name, int def)
        {
            JToken token = GetParameter(name);
            return (token != null) ? (int)token : def;
        }

        public float GetParameterWithDefault(string name, float def)
        {
            JToken token = GetParameter(name);
            return (token != null) ? (float)token : def;
        }

        public string GetParameterWithDefault(string name, string def)
        {
            JToken token = GetParameter(name);
            return (token != null) ? (string)token : def;
        }

        public double[] GetParameterWithDefault(string name, double[] def)
        {
            if (HasParameter(name))
            {
                List<Parameter> valueList = new Parameter(GetParameter(name));
                var doubles = new double[valueList.Count];
                for (int i = 0; i < doubles.Length; i++)
                {
                    doubles[i] = valueList[i];
                }

                return doubles;
            }

            return def;
        }

        public int[] GetParameterWithDefault(string name, int[] def)
        {
            if (HasParameter(name))
            {
                List<Parameter> valueList = new Parameter(GetParameter(name));
                var ints = new int[valueList.Count];
                for (int i = 0; i < ints.Length; i++)
                {
                    ints[i] = valueList[i];
                }

                return ints;
            }

            return def;
        }
    }
}

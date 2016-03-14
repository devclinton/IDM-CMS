using System;
using System.Collections.Generic;
using System.Reflection;

namespace compartments.CommandLine
{
    public class CommandLineParameters
    {
        public class OptionInfo
        {
            readonly string _description;
            object _value;

            // Constructor for required parameters
            public OptionInfo(string name, string description, Type type)
            {
                if (name == null)
                    throw new ArgumentNullException(nameof(name), "Options must have a valid name (non-null).");

                if (name.Length < 1)
                    throw new ArgumentException("Options must have a valid name (not empty).", nameof(name));

                if (type == null)
                    throw new ArgumentException("Required parameters must have a type supplied.");

                Names       = name.Split('|');
                Name        = Names[0];
                _description = description;
                IsRequired    = true;
                IsPresent     = false;
                Type        = type;
                _value       = null;
            }

            // Constructor for optional parameters
            public OptionInfo(string name, string description, object defaultValue)
            {
                if (name == null)
                    throw new ArgumentNullException(nameof(name), "Options must have a valid name (non-null).");

                if (name.Length < 1)
                    throw new ArgumentException("Options must have a valid name (not empty).", nameof(name));

                if (defaultValue == null)
                    throw new ArgumentException("Optional parameters must have a default supplied.");

                Names = name.Split('|');
                Name        = Names[0];
                _description = description;
                IsRequired    = false;
                IsPresent     = true;

                Type     = defaultValue.GetType();
                Fallback = defaultValue;
                _value    = defaultValue;
            }

            public string Name { get; }
            public string[] Names { get; }
            public string Description => _description ?? string.Empty;
            public Type Type { get; }
            public bool IsRequired { get; }
            public bool IsPresent { get; private set; }
            public object Fallback { get; }

            public static implicit operator bool(OptionInfo optionInfo)
            {
                if (optionInfo._value is bool) return (bool)optionInfo._value;

                throw new ArgumentException("Option '" + optionInfo.Name + "' isn't of type bool.");
            }

            internal bool BooleanValue
            {
                set
                {
                    if (Type != typeof(bool)) throw new ArgumentException("Option '" + Name + "' isn't of type bool.");

                    _value   = value;
                    IsPresent = true;
                }
            }

            public static implicit operator string(OptionInfo optionInfo)
            {
                // ReSharper disable once CanBeReplacedWithTryCastAndCheckForNull
                if (optionInfo._value is string) return (string)optionInfo._value;

                throw new ArgumentException("Option '" + optionInfo.Name + "' isn't of type string.");
            }

            internal string StringValue
            {
                set
                {
                    if (Type != typeof(string)) throw new ArgumentException("Option '" + Name + "' isn't of type string.");

                    _value   = value;
                    IsPresent = true;
                }
            }

            public static implicit operator int(OptionInfo optionInfo)
            {
                if (optionInfo._value is int) return (int)optionInfo._value;

                throw new ArgumentException("Option '" + optionInfo.Name + "' isn't of type int.");
            }

            public int IntegerValue
            {
                set
                {
                    if (Type != typeof(int)) throw new ArgumentException("Option '" + Name + "' isn't of type int.");

                    _value   = value;
                    IsPresent = true;
                }
            }
        }

        private readonly string[] _parameters;

        private readonly IEnumerable<OptionInfo> _options; 
        private readonly Dictionary<string, OptionInfo> _settings;

        public CommandLineParameters(string[] argv, IEnumerable<OptionInfo> opts)
        {
            _parameters = argv;

            // ReSharper disable PossibleMultipleEnumeration
            _options = opts;
            _settings = new Dictionary<string, OptionInfo>();
            foreach (OptionInfo option in opts)
                foreach (String name in option.Names)
                    _settings.Add(name.ToUpper(), option);

            // ReSharper restore PossibleMultipleEnumeration

            Arguments = new List<string>();
            IsValid   = false;

            Process();
        }

        public List<String> Arguments { get; }

        protected void Process()
        {
            bool encounteredProblem = false;

            for (int i = 0; i < _parameters.Length; i++)
            {
                string parameter = _parameters[i];
                switch (parameter[0])
                {
                    case '-':
                    case '/':
                        parameter = parameter.Substring(1);
                        if (parameter.ToUpper() == "HELP")
                        {
                            ShowHelp();
                            IsValid = false;
                            return;
                        }

                        if (_settings.ContainsKey(parameter.ToUpper()))
                        {
                            OptionInfo option = _settings[parameter.ToUpper()];
                            if (option.Type == typeof(bool))
                                option.BooleanValue = true;
                            else if (option.Type == typeof(string) || option.Type == typeof(int))
                            {
                                if (++i < _parameters.Length)
                                {
                                    if (option.Type == typeof(string))
                                        option.StringValue = _parameters[i];
                                    else
                                    {
                                        int value;
                                        if (Int32.TryParse(_parameters[i], out value))
                                            option.IntegerValue = value;
                                        else
                                        {
                                            Console.Error.WriteLine("Bad numeric argument '{0}'.", _parameters[i]);
                                            encounteredProblem = true;
                                        }
                                    }
                                }
                                else
                                {
                                    Console.Error.WriteLine("Missing argument for option '{0}'.", parameter);
                                    encounteredProblem = true;
                                }
                            }
                        }

                        if (parameter == "?")
                        {
                            ShowHelp();
                            IsValid = false;
                            return;
                        }

                        break;

                    case '?':
                        ShowHelp();
                        IsValid = false;
                        return;

                    default:
                        // Un-switched argument
                        Arguments.Add(parameter);
                        break;
                }
            }

            IsValid = !encounteredProblem;

            foreach (OptionInfo option in _settings.Values)
            {
                if (option.IsRequired && !option.IsPresent)
                {
                    Console.Error.WriteLine("Missing required parameter '{0}'.", option.Name);
                    IsValid = false;
                }
            }
        }

        protected void ShowHelp()
        {
            Console.Error.WriteLine("Usage: {0}", System.IO.Path.GetFileName(Assembly.GetExecutingAssembly().Location));
            foreach (OptionInfo option in _options)
            {
                Console.Error.Write('\t');
                if (option.IsRequired)
                {
                    Console.Error.WriteLine("-{0} {1}", option.Name, option.Description);
                }
                else
                {
                    Console.Error.WriteLine("[-{0} {1} ({2})]", option.Name, option.Description, option.Fallback);
                }
            }
        }

        public bool IsValid { get; private set; }

        public OptionInfo this[string option] => _settings[option.ToUpper()];
    }
}

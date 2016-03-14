using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace compartments.emod.utils
{
    public class BiasingParametersDeserializer
    {
        public class MissingRareEventSpecificationException : ArgumentException { public MissingRareEventSpecificationException(string message) : base(message) {} }
        public class ExpressionSpecificationException : ArgumentException { public ExpressionSpecificationException(string message) : base(message) {} }
        public class ReactionArrayCountException : ArgumentException { public ReactionArrayCountException(string message, string parameter) : base(message, parameter) { } }
        public class RareEventInfoArrayLengthException : ArgumentException { public RareEventInfoArrayLengthException(string message, string parameter) : base(message, parameter) { } }
        public class GammaArrayLengthException : ArgumentException { public GammaArrayLengthException(string message, string parameter) : base(message, parameter) { } }
        public class CutoffArrayLengthException : ArgumentException { public CutoffArrayLengthException(string message, string parameter) : base(message, parameter) { } }

        protected BiasingParametersDeserializer() {}

        public static BiasingParameters ReadParametersFromJsonFile(string jsonFilename)
        {
            BiasingParameters parameters;

            try
            {
                using (TextReader reader = new StreamReader(jsonFilename))
                {
                    var serializedParameters = reader.ReadToEnd();
                    parameters = ParametersFromString(serializedParameters);
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Error reading parameters from '{0}' - \"{1}\".", jsonFilename, e.Message);
                throw;
            }

            return parameters;
        }

        protected static BiasingParameters ParametersFromString(string serializedParameters)
        {
            var parameters = new BiasingParameters();
            JObject root;
            try
            {
                root = (JObject)JsonConvert.DeserializeObject(serializedParameters);
            }
            catch (JsonReaderException jre)
            {
                Console.Error.WriteLine("Error parsing JSON data: '{0}'", jre.Message);
                throw new ArgumentException("Error parsing JSON data.", jre);
            }

            ReadRareEventSpec(root, ref parameters);
            ReadLocalesArray(root, ref parameters);

            return parameters;
        }

        protected static void ReadRareEventSpec(JObject root, ref BiasingParameters parameters)
        {
            JToken rareEventSpecToken = root["RARE_EVENT"];
            var rareEventSpec = rareEventSpecToken as JObject;
            if (rareEventSpec == null) throw new MissingRareEventSpecificationException("JSON data doesn't contain correct 'RARE_EVENT' section.");
            var rareEventExpressionInfo = rareEventSpec["EXPRESSION"] as JObject;
            if (rareEventExpressionInfo == null) throw new ExpressionSpecificationException("JSON data doesn't contain correct 'EXPRESSION' section.");
            ReadRareEventExpressionInfo(rareEventExpressionInfo, ref parameters);
            var intermediateRareEventCount = (int) rareEventSpec["IRE_COUNT"];
            var thresholds = (JArray) rareEventSpec["THRESHOLDS"];
            for (int i = 0; i < intermediateRareEventCount; i++)
                parameters.RareEvent.Thresholds.Add((float) thresholds[i]);
        }

        protected static void ReadRareEventExpressionInfo(JObject rareEventExpressionInfo, ref BiasingParameters parameters)
        {
            parameters.RareEvent.ExpressionLocale = (string) rareEventExpressionInfo["LOCALE"];
            parameters.RareEvent.ExpressionName   = (string) rareEventExpressionInfo["NAME"];
        }

        protected static void ReadLocalesArray(JObject root, ref BiasingParameters parameters)
        {
            var locales = (JArray) root["LOCALES"];
            foreach (JToken token in locales)
            {
                var locale = (JObject) token;
                var localeInfo = ReadLocaleInformation(locale, parameters);
                parameters.Locales.Add(localeInfo);
            }
        }

        protected static BiasingParameters.LocaleInfo ReadLocaleInformation(JObject locale, BiasingParameters parameters)
        {
            var localeInfo = new BiasingParameters.LocaleInfo { Name = (string) locale["NAME"] };
            var reactionCount = (int) locale["REACTION_COUNT"];
            var reactions = (JArray) locale["REACTIONS"];
            if (reactionCount != reactions.Count) throw new ReactionArrayCountException(string.Format("JSON data for locale '{0}' invalid, REACTION_COUNT doesn't match REACTION array size.", localeInfo.Name), localeInfo.Name);
            foreach (JToken token in reactions)
            {
                var reaction = (JObject) token;
                var reactionInfo = ReadReactionInformation(reaction, parameters.RareEvent.IntermediateRareEventCount);
                localeInfo.Reactions.Add(reactionInfo);
            }

            return localeInfo;
        }

        protected static BiasingParameters.ReactionInfo ReadReactionInformation(JObject reaction, int intermediateRareEventCount)
        {
            var reactionInfo = new BiasingParameters.ReactionInfo {Name = (string) reaction["NAME"]};
            var rareEventInfoArray = (JArray) reaction["RARE_EVENT_INFO"];
            if (rareEventInfoArray.Count != intermediateRareEventCount) throw new RareEventInfoArrayLengthException(string.Format("JSON data for reaction '{0}' invalid, RARE_EVENT_INFO array length doesn't match IRE_COUNT.", reactionInfo.Name), reactionInfo.Name);
            foreach (var rareEventInfo in from JObject eventInfo in rareEventInfoArray select ReadRareEventInfo(eventInfo, reactionInfo))
            {
                reactionInfo.RareEvents.Add(rareEventInfo);
            }

            return reactionInfo;
        }

        protected static BiasingParameters.RareEventInfo ReadRareEventInfo(JObject eventInfo, BiasingParameters.ReactionInfo reactionInfo)
        {
            var rareEventInfo = new BiasingParameters.RareEventInfo {BinCount = (int) eventInfo["BIN_COUNT"]};
            var gammasArray = (JArray) eventInfo["GAMMAS"];
            if (gammasArray.Count != rareEventInfo.BinCount) throw new GammaArrayLengthException(string.Format("JSON data for reaction '{0}' invalid, GAMMAS array length doesn't match BIN_COUNT.", reactionInfo.Name), reactionInfo.Name);
            for (var i = 0; i < rareEventInfo.BinCount; i++)
                rareEventInfo.Gammas[i] = (double) gammasArray[i];
            var cutoffArray = (JArray) eventInfo["CUTOFF"];
            if (cutoffArray.Count != (rareEventInfo.BinCount - 1) && rareEventInfo.BinCount > 1) throw new CutoffArrayLengthException(string.Format("JSON data for reaction '{0}' invalid, CUTOFF array length doesn't match BIN_COUNT - 1.", reactionInfo.Name), reactionInfo.Name);
            if (rareEventInfo.BinCount == 1)
                rareEventInfo.Thresholds[0] = (int) cutoffArray[0];
            else
            {
                for (var j = 0; j < rareEventInfo.BinCount - 1; j++)
                    rareEventInfo.Thresholds[j] = (double)cutoffArray[j];
            }
            return rareEventInfo;
        }
    }
}

using System.IO;
using System.Text;

namespace compartments.emod.utils
{
    public class BiasingParametersSerializer
    {
        protected BiasingParametersSerializer()
        {
        }

        public static void WriteParametersToJsonFile(BiasingParameters parameters, string jsonFilename)
        {
            using (TextWriter writer = new StreamWriter(jsonFilename))
            {
                string serializedParameters = StringFromParameters(parameters);
                writer.Write(serializedParameters);
            }
        }

        protected static string StringFromParameters(BiasingParameters parameters)
        {
            var builder = new StringBuilder();

            builder.AppendLine("{");
            WriteRareEventSpec(parameters, builder);
            WriteLocalesArray(parameters, builder);
            builder.AppendLine("}");

            return builder.ToString();
        }

        protected static void WriteRareEventSpec(BiasingParameters parameters, StringBuilder builder)
        {
            builder.AppendLine("    \"RARE_EVENT\" : {");
            WriteRareEventExpressionInfo(parameters, builder);
            builder.AppendFormat("        \"IRE_COUNT\" : {0},", parameters.RareEvent.IntermediateRareEventCount).AppendLine();
            builder.AppendLine("        \"THRESHOLDS\" :");
            WriteRareEventThresholds(parameters, builder);
            builder.AppendLine("    },");
        }

        protected static void WriteRareEventExpressionInfo(BiasingParameters parameters, StringBuilder builder)
        {
            builder.AppendLine("        \"EXPRESSION\" : {");
            builder.AppendFormat("            \"LOCALE\" : \"{0}\",", parameters.RareEvent.ExpressionLocale).AppendLine();
            builder.AppendFormat("            \"NAME\" : \"{0}\"", parameters.RareEvent.ExpressionName).AppendLine();
            builder.AppendLine("        },");
        }

        protected static void WriteRareEventThresholds(BiasingParameters parameters, StringBuilder builder)
        {
            builder.Append("            [ ");
            int count = 0;
            foreach (var value in parameters.RareEvent.Thresholds)
            {
                builder.Append(value);
                if (++count < parameters.RareEvent.IntermediateRareEventCount)
                    builder.Append(", ");
            }
            builder.AppendLine(" ]");
        }

        protected static void WriteLocalesArray(BiasingParameters parameters, StringBuilder builder)
        {
            builder.AppendLine("    \"LOCALES\" : [");
            int count = 0;
            foreach (var locale in parameters.Locales)
            {
                WriteLocaleInformation(locale, builder);
                if (++count < parameters.Locales.Count)
                    builder.AppendLine(",");
                else
                    builder.AppendLine();
            }
            builder.AppendLine("    ]");
        }

        protected static void WriteLocaleInformation(BiasingParameters.LocaleInfo locale, StringBuilder builder)
        {
            builder.AppendLine("        {");
            builder.AppendFormat("            \"NAME\" : \"{0}\",", locale.Name).AppendLine();
            builder.AppendFormat("            \"REACTION_COUNT\" : {0},", locale.ReactionCount).AppendLine();
            builder.AppendLine("            \"REACTIONS\" : [");
            int count = 0;
            foreach (var reaction in locale.Reactions)
            {
                WriteReactionInformation(reaction, builder);
                if (++count < locale.ReactionCount)
                    builder.AppendLine(",");
                else
                    builder.AppendLine();
            }
            builder.AppendLine("            ]");
            builder.Append("        }");
        }

        protected static void WriteReactionInformation(BiasingParameters.ReactionInfo reaction, StringBuilder builder)
        {
            builder.AppendLine("                {");
            builder.AppendFormat("                \"NAME\" : \"{0}\",", reaction.Name).AppendLine();
            builder.AppendLine("                \"RARE_EVENT_INFO\" : [");
            int count = 0;
            foreach (var rareEvent in reaction.RareEvents)
            {
                WriteRareEventInfo(rareEvent, builder);
                if (++count < reaction.RareEvents.Count)
                    builder.AppendLine(",");
                else
                    builder.AppendLine();
            }
            builder.AppendLine("                    ]");
            builder.Append("                }");
        }

        protected static void WriteRareEventInfo(BiasingParameters.RareEventInfo rareEvent, StringBuilder builder)
        {
            builder.AppendLine("                        {");
            builder.AppendFormat("                             \"BIN_COUNT\" : {0},", rareEvent.BinCount).AppendLine();
            WriteGammaValues(rareEvent, builder);
            WriteThresholdValues(rareEvent, builder);
            builder.Append("                        }");
        }

        protected static void WriteGammaValues(BiasingParameters.RareEventInfo rareEvent, StringBuilder builder)
        {
            builder.Append("                             \"GAMMAS\" : [");
            int count = 0;
            foreach (var value in rareEvent.Gammas)
            {
                builder.Append(value);
                if (++count < rareEvent.Gammas.Length)
                    builder.Append(", ");
            }
            builder.AppendLine(" ],");
        }

        protected static void WriteThresholdValues(BiasingParameters.RareEventInfo rareEvent, StringBuilder builder)
        {
            builder.Append("                             \"CUTOFF\" : [");
            int count = 0;
            foreach (var value in rareEvent.Thresholds)
            {
                builder.Append(value);
                if (++count < rareEvent.Thresholds.Length)
                    builder.Append(", ");
            }
            builder.AppendLine(" ]");
        }
    }
}

namespace compartments.solvers.solverbase
{
    public class CsvOutputOptions
    {
        public string Filename { get; set; }
        public bool CompressOutput { get; set; }
        public bool WriteCsvFile { get; set; }
        public bool WriteRealizationIndex { get; set; }

        public CsvOutputOptions()
        {
            _writeVersionInfo = Configuration.CurrentConfiguration.GetParameterWithDefault("output._writeVersionInfo", true);
            _writeSampleTimes = Configuration.CurrentConfiguration.GetParameterWithDefault("output._writeSampleTimes", true);
            _writeObservableInfo = Configuration.CurrentConfiguration.GetParameterWithDefault("output._writeObservableInfo", true);
        }

        // These private variables are only present to support scenario testing.
        private static bool _writeVersionInfo = true;
        private static bool _writeSampleTimes = true;
        private static bool _writeObservableInfo = true;
    }
}
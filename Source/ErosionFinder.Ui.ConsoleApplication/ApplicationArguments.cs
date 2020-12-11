using CommandLine;

namespace ErosionFinder.Ui.ConsoleApplication
{
    class ApplicationArguments
    {
        private const string DefaultOutputFilePath = "Violations.json";

        [Option('s', "solution", Required = true,
            HelpText = "Solution file ( .sln ) path")]
        public string SolutionFilePath { get; set; }

        [Option('l', "layers and rules", Required = true,
            HelpText = "Architectural layers and rules file ( .json ) path")]
        public string ArchitecturalLayersAndRulesFilePath { get; set; }

        [Option('o', "Output file path", Required = false,
            HelpText = "Output file path")]
        public string OutputFilePath { get; set; } = DefaultOutputFilePath;
    }
}
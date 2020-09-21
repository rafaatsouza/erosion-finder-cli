using CommandLine;

namespace ErosionFinder.Ui.ConsoleApplication
{
    class ApplicationArguments
    {
        [Option('s', "solution", Required = true,
            HelpText = "Solution file ( .sln ) path")]
        public string SolutionFilePath { get; set; }

        [Option('c', "constraint", Required = true,
            HelpText = "Constraints file ( .json ) path")]
        public string ConstraintsFilePath { get; set; }
    }
}
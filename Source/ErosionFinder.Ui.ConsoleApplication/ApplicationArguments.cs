using CommandLine;
using Serilog.Events;

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

        [Option('v', "verbosity", Required = false, Default = null,
            HelpText = "Log verbosity level; if not defined, only errors will be logged.")]
        public LogEventLevel? LogLevel { get; set; }
    }
}
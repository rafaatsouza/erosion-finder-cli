using CommandLine;

namespace ErosionFinder.Ui.ConsoleApplication
{
    static class CommandLineParser
    {
        public static ApplicationArguments GetArguments(string[] args)
        {
            var result = Parser.Default
                .ParseArguments<ApplicationArguments>(args);

            if (result is Parsed<ApplicationArguments> arguments)
            {
                return arguments.Value;
            }

            return null;
        }
    }
}
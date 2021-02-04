using CommandLine;
using ErosionFinderCLI.Models;
using System;
using System.IO;

namespace ErosionFinderCLI.Helpers
{
    static class CommandLineHelper
    {
        public static ConformanceCheckParameters GetParameters(string[] args)
        {
            var arguments = GetArguments(args);
            
            if (arguments == null)
                throw new ArgumentNullException(nameof(arguments));

            if (string.IsNullOrEmpty(arguments.OutputFilePath) ||
                string.IsNullOrEmpty(Path.GetFullPath(arguments.OutputFilePath)))
                throw new ArgumentException("Invalid path", nameof(arguments.OutputFilePath));

            if (!CheckFilePath(arguments.SolutionFilePath))
                throw new ArgumentException("Invalid path", nameof(arguments.SolutionFilePath));

            if (!CheckFilePath(arguments.ArchitecturalLayersAndRulesFilePath)) 
                throw new ArgumentException("Invalid path", 
                    nameof(arguments.ArchitecturalLayersAndRulesFilePath));

            var outputFileInfo = new FileInfo(arguments.OutputFilePath);

            var outputFileName = outputFileInfo.Name;
            var outputFolderPath = outputFileInfo.Directory.FullName;

            return new ConformanceCheckParameters()
            {
                SolutionFilePath = arguments.SolutionFilePath,
                ArchitecturalLayersAndRulesFilePath = arguments.ArchitecturalLayersAndRulesFilePath,
                OutputFileName = outputFileName,
                OutputFolderPath = outputFolderPath
            };
        }

        private static bool CheckFilePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            try
            {
                var fileInfo = new FileInfo(path);

                return fileInfo.Exists;                
            }
            catch (System.IO.PathTooLongException) 
            {
                return false;
            }
            catch (NotSupportedException) 
            { 
                return false;
            }
        }

        private static ApplicationArguments GetArguments(string[] args)
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
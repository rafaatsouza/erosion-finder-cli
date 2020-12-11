using ErosionFinder.Data.Converter;
using ErosionFinder.Data.Exceptions.Base;
using ErosionFinder.Data.Models;
using Microsoft.Build.Locator;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ErosionFinder.Ui.ConsoleApplication
{
    static class Program
    {
        private const string ReportFileName = "Violations.json";

        static Program()
        {
            if (MSBuildLocator.CanRegister)
            {
                MSBuildLocator.RegisterDefaults();
            }

            if (!MSBuildLocator.IsRegistered)
            {
                throw new Exception("MSBuild could not be registered");
            }
        }

        static async Task Main(string[] args)
        {
            var arguments = CommandLineParser.GetArguments(args);

            if (arguments == null)
                return;

            try
            {
                var cancellationTokenSource = new CancellationTokenSource();

                var constraints = GetConstraintsByFilePath(
                    arguments.ArchitecturalLayersAndRulesFilePath);

                var violations = await ErosionFinderMethods
                    .GetViolationsBySolutionFilePathAndConstraintsAsync(
                        arguments.SolutionFilePath, constraints, cancellationTokenSource.Token);

                var reportFilePath = await ReportGenerator
                    .WriteReportAsync(ReportFileName, violations);

                Console.WriteLine($"Report generated: {reportFilePath}");
            }
            catch (ErosionFinderException ex)
            {
                Console.WriteLine($"{ex.Key} Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Critical Error: {ex.Message}");
            }
        }

        private static ArchitecturalConstraints GetConstraintsByFilePath(string constraintsFilePath)
        {
            var constraintsFile = new FileInfo(constraintsFilePath);

            if (!constraintsFile.Exists)
                return null;

            using (var streamReader = new StreamReader(constraintsFilePath))
            {
                var json = streamReader.ReadToEnd();
                
                return JsonConvert.DeserializeObject<ArchitecturalConstraints>(
                    json, new NamespacesGroupingDeserializer());
            }
        }
    }
}
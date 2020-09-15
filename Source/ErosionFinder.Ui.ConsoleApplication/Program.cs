using ErosionFinder.Data.Converter;
using ErosionFinder.Data.Exceptions.Base;
using ErosionFinder.Data.Interfaces;
using ErosionFinder.Data.Models;
using Microsoft.Build.Locator;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ErosionFinder.Ui.ConsoleApplication
{
    static class Program
    {
        private static readonly IErosionFinderService erosionFinderService;

        private const string ReportFileName = "Violations.json";

        static Program()
        {
            MSBuildLocator.RegisterDefaults();

            if (!MSBuildLocator.IsRegistered)
            {
                throw new Exception("MSBuild could not be registered");
            }
            
            erosionFinderService = ServiceCollectionProvider.GetService<IErosionFinderService>()
                ?? throw new ArgumentNullException(nameof(IErosionFinderService));
        }

        static async Task Main(string[] args)
        {
            var arguments = CommandLineParser.GetArguments(args);

            if (arguments == null)
                return;

            try
            {
                if (arguments.LogLevel.HasValue)
                {
                    LoggerConfigurationProvider.AlterLogLevel(arguments.LogLevel.Value);
                }

                var cancellationTokenSource = new CancellationTokenSource();

                var constraints = GetConstraintsByFilePath(arguments.ConstraintsFilePath);

                var violations = await erosionFinderService.GetViolationsBySolutionFilePathAndConstraintsAsync(
                    arguments.SolutionFilePath, constraints, cancellationTokenSource.Token);

                var reportFilePath = await WriteReportAsync(violations);

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

            if (constraintsFile.Exists)
            {
                using (var streamReader = new StreamReader(constraintsFilePath))
                {
                    var json = streamReader.ReadToEnd();
                    var constraints = JsonConvert
                        .DeserializeObject<ArchitecturalConstraints>(json, new NamespacesGroupingDeserializer());

                    return constraints;
                }
            }

            return null;
        }

        private static async Task<string> WriteReportAsync(IEnumerable<Violation> violations)
        {
            OverwriteFile(ReportFileName);

            using (var file = File.CreateText(ReportFileName))
            {
                var jsonContent = JsonConvert.SerializeObject(violations, new JsonSerializerSettings()
                {
                    Formatting = Formatting.Indented,
                    Converters = new List<JsonConverter>() { new Newtonsoft.Json.Converters.StringEnumConverter() },
                    ContractResolver = new OrderContractResolver()
                });

                await file.WriteAsync(jsonContent);
            }

            return ReportFileName;
        }

        private static void OverwriteFile(string reportFileName)
        {
            var fileInfo = new FileInfo(reportFileName);

            if (fileInfo.Exists)
                fileInfo.Delete();
        }
    }
}
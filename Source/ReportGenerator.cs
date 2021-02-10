using ErosionFinder.Data.Models;
using ErosionFinderCLI.Helpers;
using ErosionFinderCLI.Models;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ErosionFinderCLI
{
    static class ReportGenerator
    {
        private const string regexTemplatePattern = "\\{\\{(.*?)\\}\\}";
        private const string templateFileName = "report.html";
        private const string OutputReportResourcesFolder = "Resources";

        private readonly static Type modelType = typeof(Report);
        private readonly static string[] resources = 
            new string[] { "bootstrap.min.css", "bootstrap.min.js", 
                "jquery-3.2.1.slim.min.js", "erosion-finder-report.min.css", 
                "chart.min.js", "erosion-finder-report.min.js" };

        public static async Task WriteReportAsync(string reportFolderFullName, 
            string reportFileName, ArchitecturalConformanceCheck conformanceCheck)
        {
            var resourcesTask = WriteResourceFilesAsync(
                reportFolderFullName);

            var reportFileFullName = Path.Join(
                reportFolderFullName, reportFileName);

            DeleteFileIfExists(reportFileFullName);

            using (var file = File.CreateText(reportFileFullName))
            {
                var reportContent = GetReportContent(conformanceCheck);

                await file.WriteAsync(reportContent);
            }

            await resourcesTask;
        }

        private static string GetReportContent(
            ArchitecturalConformanceCheck conformanceCheck)
        {
            var templateContent = ResourceHelper
                .GetResourceTextContent(templateFileName);

            var model = new Report(conformanceCheck);

            return Regex.Replace(templateContent, regexTemplatePattern, m =>
                {
                    if (m.Groups.Count == 0)
                        return m.Value;

                    var property = modelType.GetProperty(m.Groups[1].Value);

                    var value = property?.GetValue(model, null)?.ToString();

                    return value ?? m.Value;
                });
        }

        private static async Task WriteResourceFilesAsync(string directory)
        {
            var folder = GetResourcesFolderPath(directory);

            foreach (var resource in resources)
            {
                var filePath = Path.Join(folder, resource);

                DeleteFileIfExists(filePath);

                using (var file = File.CreateText(filePath))
                {
                    await file.WriteAsync(
                        ResourceHelper.GetResourceTextContent(resource));
                }
            }
        }

        private static string GetResourcesFolderPath(string baseDirectory)
        {
            var folder = Path.Join(baseDirectory, 
                OutputReportResourcesFolder);

            var directoryInfo = new DirectoryInfo(folder);

            if (!directoryInfo.Exists)
                Directory.CreateDirectory(folder);

            return folder;
        }

        private static void DeleteFileIfExists(string filePath)
        {
            var fileInfo = new FileInfo(filePath);

            if (fileInfo.Exists)
                fileInfo.Delete();
        }
    }
}
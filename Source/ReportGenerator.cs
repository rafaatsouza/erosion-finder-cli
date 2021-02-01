using ErosionFinder.Data.Models;
using ErosionFinderCLI.Helpers;
using ErosionFinderCLI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ErosionFinderCLI
{
    static class ReportGenerator
    {
        private const string regexTemplatePattern = "\\{\\{(.*?)\\}\\}";
        private const string templateFileName = "report.html";
        private const string ErosionFinderReportResources = "Resources";

        private readonly static Type modelType = typeof(Report);
        private readonly static ICollection<string> textResources = 
            new string[] { "bootstrap.min.css", "bootstrap.min.js", 
                "jquery-3.2.1.slim.min.js", "erosion-finder-report.css", 
                "chart.min.js", "erosion-finder-report.min.js" };

        public static async Task WriteReportAsync(string reportFolderFullName, 
            string reportFileName, ArchitecturalConformanceCheck conformanceCheck)
        {
            var resourcesTask = WriteResourceFilesAsync(
                reportFolderFullName);

            var reportFileFullName = Path.Join(
                reportFolderFullName, reportFileName);

            DeleteFile(reportFileFullName);

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
            var folder = Path.Join(directory, ErosionFinderReportResources);

            var directoryInfo = new DirectoryInfo(folder);

            if (!directoryInfo.Exists)
                Directory.CreateDirectory(folder);

            foreach (var resource in textResources)
            {
                var filePath = Path.Join(folder, resource);
                var content = ResourceHelper.GetResourceTextContent(resource);

                DeleteFile(filePath);

                using (var file = File.CreateText(filePath))
                {
                    await file.WriteAsync(content);
                }
            }
        }

        private static void DeleteFile(string filePath)
        {
            var fileInfo = new FileInfo(filePath);

            if (fileInfo.Exists)
                fileInfo.Delete();
        }
    }
}
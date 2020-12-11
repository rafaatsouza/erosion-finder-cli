using ErosionFinder.Data.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ErosionFinder.Ui.ConsoleApplication
{
    static class ReportGenerator
    {
        private static JsonSerializerSettings SerializerSettings { get; }
            = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                Converters = new List<JsonConverter>() { new StringEnumConverter() },
                ContractResolver = new OrderContractResolver()
            };

        public static async Task WriteReportAsync(
            string reportFileName, IEnumerable<Violation> violations)
        {
            OverwriteFile(reportFileName);

            using (var file = File.CreateText(reportFileName))
            {
                var jsonContent = JsonConvert
                    .SerializeObject(violations, SerializerSettings);

                await file.WriteAsync(jsonContent);
            }
        }

        private static void OverwriteFile(
            string reportFileName)
        {
            var fileInfo = new FileInfo(reportFileName);

            if (fileInfo.Exists)
                fileInfo.Delete();
        }
    }
}
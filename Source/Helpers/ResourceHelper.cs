using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;

namespace ErosionFinderCLI.Helpers
{
    static class ResourceHelper
    {
        private readonly static Assembly assembly = Assembly.GetExecutingAssembly();

        public static string GetResourceTextContent(
            string resourceFileName)
        {
            var resourceName = GetResourceNameByFileName(resourceFileName);

            using (var stream = GetResourceStream(resourceName))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        private static Stream GetResourceStream(string resource)
        {
            var resourceName = GetResourceNameByFileName(resource);

            return assembly.GetManifestResourceStream(resourceName);
        }

        private static string GetResourceNameByFileName(string fileName)
        {
            var resourceName = assembly.GetManifestResourceNames()
                .FirstOrDefault(n => n.EndsWith(fileName));

            if (string.IsNullOrEmpty(resourceName))
                throw new MissingManifestResourceException(
                    $"Could not find the resource for the {fileName} file");

            return resourceName;
        }
    }
}
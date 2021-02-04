using System.Collections.Generic;
using ErosionFinder.Data.Models;
using Newtonsoft.Json;

namespace ErosionFinderCLI.Models
{
    class Report
    {
        /// <summary>
        /// Solution name
        /// </summary>
        public string SolutionName { get; }

        /// <summary>
        /// Followed rule serialized list
        /// </summary>
        public string FollowedRules { get; }

        /// <summary>
        /// Transgressed rule serialized list
        /// </summary>
        public string TransgressedRules { get; }
        
        public Report(ArchitecturalConformanceCheck conformanceCheck)
        {
            SolutionName = conformanceCheck.SolutionName;

            FollowedRules = JsonConvert.SerializeObject(
                conformanceCheck.FollowedRules, SerializerSettings);
            TransgressedRules = JsonConvert.SerializeObject(
                conformanceCheck.TransgressedRules, SerializerSettings);
        }

        private readonly static JsonSerializerSettings SerializerSettings
            = new JsonSerializerSettings()
            {
                Converters = new List<JsonConverter>() 
                { 
                    new ErosionFinder.Data.Converter.NamespacesGroupingDeserializer(),
                    new Newtonsoft.Json.Converters.StringEnumConverter()
                }
            };
    }    
}
using System.Collections.Generic;
using System.Linq;
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
        /// Total number of Violated rules
        /// </summary>
        public int ViolatedRulesCount {get; }

        /// <summary>
        /// Total number of Followed rules
        /// </summary>
        public int FollowedRulesCount {get; }

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

            FollowedRulesCount = conformanceCheck.FollowedRules.Count();
            ViolatedRulesCount = conformanceCheck.TransgressedRules.Count();

            FollowedRules = JsonConvert.SerializeObject(
                conformanceCheck.FollowedRules, SerializerSettings);
            TransgressedRules = JsonConvert.SerializeObject(
                conformanceCheck.TransgressedRules, SerializerSettings);
        }

        private int GetStructureTypeCount(StructureType type,
            IDictionary<StructureType, int> structuresCount)
        {
            if (!structuresCount.ContainsKey(type))
                return 0;

            return structuresCount[type];
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
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ErosionFinder.Ui.ConsoleApplication
{
    class OrderContractResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            => base.CreateProperties(type, memberSerialization)
                .OrderBy(p => p.PropertyName).ThenBy(p => p.UnderlyingName)
                .ToList();
    }
}
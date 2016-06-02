using System;
using System.Runtime.Serialization;

namespace JsonGenerator.JsonEntities
{
    [DataContract]
    public class JsonPackageHeader
    {
        [DataMember(Name = "table", EmitDefaultValue = false, Order = 1)]
        public String Entity { get; set; }

        [DataMember(Name = "items", EmitDefaultValue = false, Order = 2)]
        public JsonPackageItem[] Items { get; set; }

        [DataMember(Name = "items_layout", EmitDefaultValue = false, Order = 3)]
        public JsonPackageLayoutPanel[][] Panels { get; set; }

        [DataMember(Name = "actions", EmitDefaultValue = false, Order = 4)]
        public JsonPackageAction[] Actions { get; set; }

        [DataMember(Name = "std_actions", EmitDefaultValue = false, Order = 5)]
        public JsonPackageAction[] StdActions { get; set; }
    }
}
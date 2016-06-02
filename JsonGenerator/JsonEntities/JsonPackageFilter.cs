using System;
using System.Runtime.Serialization;

namespace JsonGenerator.JsonEntities
{
    [DataContract]
    public class JsonPackageFilter
    {
        [DataMember(Name = "filter_path", EmitDefaultValue = false, Order = 1)]
        public String FilterPath { get; set; }

        [DataMember(Name = "filter_source", EmitDefaultValue = false, Order = 2)]
        public String FilterSource { get; set; }

        [DataMember(Name = "cond_items", EmitDefaultValue = false, Order = 3)]
        public Object[] ConditionItems { get; set; }

        [DataMember(Name = "result_items", EmitDefaultValue = false, Order = 4)]
        public Int32[] ResultItems { get; set; }
    }
}
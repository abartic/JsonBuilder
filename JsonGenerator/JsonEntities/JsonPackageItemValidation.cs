using System;
using System.Runtime.Serialization;

namespace JsonGenerator.JsonEntities
{
    [DataContract]
    public class JsonPackageItemValidation
    {
        [DataMember(Name = "required", EmitDefaultValue = false, Order = 1)]
        public Boolean Required { get; set; }

        [DataMember(Name = "max_length", EmitDefaultValue = false, Order = 2)]
        public Int32 MaxmaxLength { get; set; }
    }
}
using System;
using System.Runtime.Serialization;

namespace JsonGenerator.JsonEntities
{
    [DataContract]
    public class JsonPackageDetail
    {
        [DataMember(Name = "relation", EmitDefaultValue = false, Order = 1)]
        public String Relation { get; set; }

        [DataMember(Name = "control", EmitDefaultValue = false, Order = 2)]
        public String Control { get; set; }

        [DataMember(Name = "datasource", EmitDefaultValue = false, Order = 3)]
        public String DataSource { get; set; }

        [DataMember(Name = "persist", EmitDefaultValue = false, Order = 4)]
        public String Persist { get; set; }

        [DataMember(Name = "title", EmitDefaultValue = false, Order = 5)]
        public String Title { get; set; }

        [DataMember(Name = "fields", EmitDefaultValue = false, Order = 6)]
        public Int32[] Fields { get; set; }

        [DataMember(Name = "items", EmitDefaultValue = false, Order = 7)]
        public JsonPackageItem[] Items { get; set; }

        [DataMember(Name = "items_layout", EmitDefaultValue = false, Order = 8)]
        public JsonPackageLayoutPanel[][] Panels { get; set; }

        [DataMember(Name = "details_actions", EmitDefaultValue = false, Order = 9)]
        public JsonPackageAction[] Actions { get; set; }

        [DataMember(Name = "std_details_actions", EmitDefaultValue = false, Order = 10)]
        public JsonPackageAction[] StdActions { get; set; }

        JsonPackageLayoutPanel panel;
        public JsonPackageLayoutPanel Panel
        {
            get
            {
                return panel;
            }
            set
            {
                panel = value;
            }
        }

        
    }
}
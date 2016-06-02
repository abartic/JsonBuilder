using JsonGenerator.DbEntities;
using System;
using System.Runtime.Serialization;

namespace JsonGenerator.JsonEntities
{
    [DataContract]
    public class JsonPackageLayoutPanel
    {
        [OnSerializing]
        void OnSerializing(StreamingContext ctx)
        {
            if (name == "[name]")
            {
                name = null;
            }
        }

        [OnDeserialized]
        void OnDeserialized(StreamingContext ctx)
        {
            if (String.IsNullOrWhiteSpace(name) == true)
            {
                name = "[name]";
            }
        }

        String name;
        [DataMember(Name = "id", EmitDefaultValue = false, Order = 1)]
        public String Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            
            }
        }

        String header;
        [DataMember(Name = "header", EmitDefaultValue = false, Order = 2)]
        public String Header
        {
            get
            {
                return header;
            }
            set
            {
                header = String.IsNullOrWhiteSpace(value) == false ? value : null;
            }
        }

        [IgnoreDataMember]
        String headerTranslation;
        public String HeaderTranslation
        {
            get
            {
                return headerTranslation;
            }
            set
            {
                headerTranslation = value;
            }
        }

        [IgnoreDataMember]
        public LangObj LangObj
        {
            get;
            set;
        }

        [DataMember(Name = "alignment", EmitDefaultValue = false, Order = 3)]
        public String Alignment { get; set; }

        [DataMember(Name = "columns", EmitDefaultValue = false, Order = 4)]
        public Int32 Columns { get; set; }

        [DataMember(Name = "items", EmitDefaultValue = false, Order = 5)]
        public Object[] Items { get; set; }

        public int Row { get; set; }
    }
}
using JsonGenerator.DbEntities;
using System;
using System.Runtime.Serialization;

namespace JsonGenerator.JsonEntities
{
    [DataContract]
    public class JsonPackageAction
    {
        [DataMember(Name = "caption", EmitDefaultValue = false, Order = 1)]
        public String Caption { get; set; }

        [IgnoreDataMember]
        public String CaptionTranslation { get; set; }

        [IgnoreDataMember]
        public LangObj LangObj
        {
            get;
            set;
        }

        [IgnoreDataMember]
        public Boolean IsStandard
        {
            get;
            set;
        }

        [DataMember(Name = "ui_class", EmitDefaultValue = false, Order = 2)]
        public String UIClass { get; set; }

        [DataMember(Name = "name", EmitDefaultValue = false, Order = 3)]
        public String Name { get; set; }

        [DataMember(Name = "forAdditionalEntity", EmitDefaultValue = false, Order = 4)]
        public Boolean ForAdditionalEntity { get; set; }

        [OnSerializing]
        private void OnDeseriazable(StreamingContext context)
        {
            if (this.Name == "NewItem" || this.Name == "UpdateItem")
                this.ForAdditionalEntity = true;
        }
    }
}
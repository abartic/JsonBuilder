using JsonGenerator.DbEntities;
using JsonGenerator.Language;
using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Web;

namespace JsonGenerator.JsonEntities
{
    [DataContract]
    public class JsonPackageItem //: Caliburn.Micro.PropertyChangedBase
     {
        

        [DataMember(Name = "id", EmitDefaultValue = false, Order = 1)]
        public String Id
        {
            get;
            set;
        }

        [DataMember(Name = "title", EmitDefaultValue = false, Order = 2)]
        public String Title { get; set; }

        [IgnoreDataMember]
        public String TitleTranslation { get; set; }

        [IgnoreDataMember]
        public LangObj LangObj { get; set; }

        [IgnoreDataMember]
        String _field;

        [DataMember(Name = "field", EmitDefaultValue = false, Order = 3)]
        public String Field
        {
            get
            {
                return _field;
            }
            set
            {
                _field = value;
                //if (String.IsNullOrWhiteSpace(Title) == true)
                //{
                //    this.Title = Translator.Instance.Translate(value);
                //    if (String.IsNullOrWhiteSpace(Title) == true)
                //        this.Title = value;
                //}
            }
        }

      

        [DataMember(Name = "resource", EmitDefaultValue = false, Order = 4)]
        public String Resource { get; set; }

        [DataMember(Name = "default_value", EmitDefaultValue = false, Order = 5)]
        public String DefaultValue { get; set; }

        [DataMember(Name = "type", EmitDefaultValue = false, Order = 6)]
        public String Type { get; set; }

        [DataMember(Name = "dbtype", EmitDefaultValue = false, Order = 7)]
        public String DbType { get; set; }

        [DataMember(Name = "format", EmitDefaultValue = false, Order = 8)]
        public String Format { get; set; }

        [DataMember(Name = "isreadonly", EmitDefaultValue = false, Order = 9)]
        public Boolean IsReadOnly { get; set; }

        [DataMember(Name = "nullable", EmitDefaultValue = false, Order = 10)]
        public Boolean Nullable { get; set; }

        [DataMember(Name = "sortable", EmitDefaultValue = false, Order = 11)]
        public Boolean Sortable { get; set; }

        [DataMember(Name = "f_sortable", EmitDefaultValue = false, Order = 12)]
        public Boolean FilterSortable { get; set; }

        [DataMember(Name = "opened", EmitDefaultValue = false, Order = 13)]
        public Boolean Opened { get; set; }

        [DataMember(Name = "path", EmitDefaultValue = false, Order = 14)]
        public String Path { get; set; }

        [DataMember(Name = "display_path",EmitDefaultValue = false, Order = 15)]
        public String DisplayPath { get; set; }

        [DataMember(Name = "value_path", EmitDefaultValue = false, Order = 16)]
        public String ValuePath { get; set; }

        [DataMember(Name = "item_content_path", EmitDefaultValue = false, Order = 17)]
        public String ItemContentPath { get; set; }

        [DataMember(Name = "item_header_path", EmitDefaultValue = false, Order = 17)]
        public String ItemHeaderPath { get; set; }

        [DataMember(Name = "control", EmitDefaultValue = false, Order = 19)]
        public String Control { get; set; }

        [DataMember(Name = "selector", EmitDefaultValue = false, Order = 20)]
        public String Selector { get; set; }

        [DataMember(Name = "selector_entity", EmitDefaultValue = false, Order = 21)]
        public String SelectorEntity { get; set; }

        [DataMember(Name = "selector_params", EmitDefaultValue = false, Order = 22)]
        public String SelectorParams { get; set; }

        [DataMember(Name = "selector_keys", EmitDefaultValue = false, Order = 23)]
        public String SelectorDisplayKeys { get; set; }

        [DataMember(Name = "filter", EmitDefaultValue = false, Order = 24)]
        public String Filter { get; set; }

        [DataMember(Name = "decimals", EmitDefaultValue = false, Order = 25)]
        public Int32 Decimals { get; set; }

        [DataMember(Name = "validations", EmitDefaultValue = false, Order = 26)]
        public JsonPackageItemValidation Validations { get; set; }

        public Column Column
        {
            get;
            set;
        }

        public Int32? FilterConditionIndex
        {
            get;
            set;
        }

        public Int32? FilterResultIndex
        {
            get;
            set;
        }

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

        public Int32? PanelIndex
        {
            get;
            set;
        }

        [OnSerializing]
        private void OnDeseriazable(StreamingContext context)
        {
            //if (String.IsNullOrWhiteSpace(Title) == true)
            //{
            //    this.Title = Translator.Instance.Translate(this.Field);
            //    if (String.IsNullOrWhiteSpace(Title) == true)
            //        this.Title = this.Field;
            //}
        }
    }


   

  
}
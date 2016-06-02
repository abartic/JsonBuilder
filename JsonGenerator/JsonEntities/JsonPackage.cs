using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JsonGenerator.JsonEntities
{
    [DataContract]
    class JsonPackage
    {
        //[DataMember(Name = "shell_code", EmitDefaultValue = false, Order =1)]
        [IgnoreDataMember]
        public Int32 ShellCode { get; set; }

        [DataMember(Name = "dependencies", EmitDefaultValue = false, Order = 2)]
        public String[] Dependencies { get; set; }

        [DataMember(Name = "header", EmitDefaultValue = false, Order = 3)]
        public JsonPackageHeader Header { get; set; }

        [DataMember(Name = "details", EmitDefaultValue = false, Order = 4)]
        public List<JsonPackageDetail> Details { get; set; }

        [DataMember(Name = "filter", EmitDefaultValue = false, Order = 5)]
        public JsonPackageFilter Filter { get; set; }

        public static JsonPackageAction[] HeaderStdActions = new JsonPackageAction[] {
            new JsonPackageAction() { Name="NewItem",Caption="COMM_EDIT_NEW",UIClass="btn-success", ForAdditionalEntity=true, IsStandard=true},
            new JsonPackageAction() { Name="UndoItem",Caption="COMM_EDIT_UNDO",UIClass="btn-warning", IsStandard=true},
            new JsonPackageAction() { Name="DeleteItem",Caption="COMM_EDIT_DELETE",UIClass="btn-danger", IsStandard=true},
            new JsonPackageAction() { Name="UpdateItem",Caption="COMM_EDIT_SAVE",UIClass="btn-primary",ForAdditionalEntity=true, IsStandard=true }};

        public static JsonPackageAction[] DetailStdActions = new JsonPackageAction[] {
            new JsonPackageAction() { Name="NewDetailItem",Caption="COMM_NEW_DETAIL",UIClass="btn-success",ForAdditionalEntity=true, IsStandard=true},
            new JsonPackageAction() { Name="DeleteDetailItem",Caption="COMM_DELETE_DETAIL",UIClass="btn-danger",ForAdditionalEntity=true, IsStandard=true}};
    }
}

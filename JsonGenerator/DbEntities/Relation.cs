using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonGenerator.DbEntities
{
    class Relation
    {
        public String SourceTableName { get; set; }
        public String SourceColumnName { get; set; }
        public String TargetTableName { get; set; }

        public String TargetEntityName { get; set; }
        public String TargetColumnName { get; set; }
        public String RelationProperty { get; private set; }


        public Relation(String targetTableName, String targetColumnName, String sourceTableName, String sourceColumnName)
        {
            this.SourceTableName = sourceTableName;
            this.SourceColumnName = sourceColumnName;
            this.TargetTableName = targetTableName;
            this.TargetColumnName = targetColumnName;
            if (this.SourceTableName.Length > 1)
            {
                this.RelationProperty = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(this.TargetTableName.ToLower().Replace("_", " ")).Replace(" ", "");
                this.RelationProperty = String.Concat(Char.ToLower(this.RelationProperty[0]), this.RelationProperty.Substring(1));
            }

            this.TargetEntityName = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(this.TargetTableName.ToLower().Replace("_", " ")).Replace(" ", "");
            //var plural = System.Data.Entity.Design.PluralizationServices.PluralizationService.CreateService(CultureInfo.GetCultureInfo("en-us"));
            //this.TargetEntityName = plural.Singularize(this.TargetEntityName);
            this.TargetEntityName = Char.ToLower(this.TargetEntityName[0]) + this.TargetEntityName.Substring(1);
        }

    
        public static List<Relation> FetchRelations()
        {
            var select = DAL.ExecuteQuery(@"select kc.table_name as target_table_name, kc.column_name as target_column_name, 
                    cu.table_name as source_table_name, cu.column_name as source_column_name from 
	                information_schema.key_column_usage kc inner join
	                information_schema.constraint_column_usage cu on kc.constraint_name = cu.constraint_name 
                    where kc.constraint_name like '%_fk_%';");
            List<Relation> columns = new List<Relation>();
            select.ToList().ForEach(so => columns.Add(new Relation((String)so[0], (String)so[1], (String)so[2], (String)so[3])));
            return columns;
        }
    }
}

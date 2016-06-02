using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonGenerator.DbEntities
{
    class Table 
    {
        public String TableName { get; set; }
        public String EntityName { get; private set; }

        public Table(String table_name)
        {
            this.TableName = table_name;
            if (String.IsNullOrWhiteSpace(this.TableName) == false)
            {
                this.EntityName = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(this.TableName.ToLower().Replace("_", " ")).Replace(" ", "");
                var plural = System.Data.Entity.Design.PluralizationServices.PluralizationService.CreateService(CultureInfo.GetCultureInfo("en-us"));
                this.EntityName = plural.Singularize(this.EntityName);
                //this.EntityName = String.Concat(Char.ToLower(this.EntityName[0]), this.EntityName.Substring(1)).Remove(this.EntityName.Length-1);
            }
        }
        
        public static List<Table> FetchTables()
        {
            var select = DAL.ExecuteQuery(@"select table_name from information_schema.tables where table_schema = 'public' order by 1;");
            List<Table> tables = new List<Table>();
            select.ToList().ForEach(so => tables.Add(new Table((String)so[0])));
            return tables;
        }

        public static List<Table> FetchEntities()
        {
            var select = DAL.ExecuteQuery(@"select table_name from information_schema.tables where table_schema = 'public' order by 1;");
            List<Table> tables = new List<Table>();
            tables.Add(new Table(null));
            select.ToList().ForEach(so => tables.Add(new Table((String)so[0])));
            return tables;
        }
    }

  
}

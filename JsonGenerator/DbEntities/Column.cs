using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonGenerator.DbEntities
{
    public class Column
    {
        public String TableName { get; set; }
        public String ColumnName { get; set; }
        public Boolean IsNullable { get; set; }
        public String DataType { get; set; }
        public Int32 CharsMaxLength { get; set; }

        public String PropertyType { get; protected set; }
        public Int32 Decimals { get; protected set; }
        private void SetPropertyType()
        {

            switch (this.DataType)
            {
                case "bool":
                    this.PropertyType = "boolean";
                    break;

                case "varchar":
                case "char":
                    this.PropertyType = "string";
                    break;

                case "timestamp":
                case "timestamptz":
                    this.PropertyType = "date";
                    break;

                case "int2":
                case "int4":
                    this.PropertyType = "number";
                    break;

                case "float8":
                    this.PropertyType = "number";
                    this.Decimals = 2;
                    break;

            }
        }

        public String FieldName
        {
            get;
            protected set;
        }

        public Column(String tableName, String columnName, Boolean isNullable, String dataType, Int32 charsMaxLength)
        {
            this.TableName = tableName;
            this.ColumnName = columnName;
            this.IsNullable = isNullable;
            this.DataType = dataType;
            this.CharsMaxLength = charsMaxLength;

            SetPropertyType();
            SetFieldName();
        }

        void SetFieldName()
        {
            StringBuilder fieldName = new StringBuilder();
            for(int i = 0; i < this.ColumnName.Length; )
            {
                char c = this.ColumnName[i++];
                if (c == '_')
                    fieldName.Append(Char.ToUpper(this.ColumnName[i++]));
                else
                    fieldName.Append(c);
            }
            this.FieldName = fieldName.ToString();
        }

        public static List<Column> FetchColumns()
        {
            var select = DAL.ExecuteQuery(@"SELECT table_name, column_name, is_nullable, udt_name, coalesce(character_maximum_length, 0) AS character_maximum_length FROM information_schema.columns ORDER BY column_name;");
            List<Column> columns = new List<Column>();
            select.ToList().ForEach(so => columns.Add(new Column((String)so[0], (String)so[1], (String)so[2] == "YES", (String)so[3], (Int32)so[4])));
            return columns;
        }
    }
}

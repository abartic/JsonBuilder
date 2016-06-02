using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace JsonGenerator.DbEntities
{
    class ShellsTree : Caliburn.Micro.PropertyChangedBase
    {

        public static Shell FetchShellsTree()
        {
            var select = DAL.ExecuteQuery(@"SELECT data FROM hierarch_structs where code ='main_menu';");
            var shells = select.FirstOrDefault().Select(so => (String)so).FirstOrDefault();

            XmlSerializer xmlser = new XmlSerializer(typeof(Shell));
            System.IO.StringReader sr = new System.IO.StringReader(shells);
            Shell root = (Shell)xmlser.Deserialize(sr);
            return root;
        }

        public static void SaveShellsTree()
        {
            Npgsql.NpgsqlConnection connection = null;
            try
            {
                int maxlevels = DbSchema.Instance.RootTreeShells.SetLevel();
                DbSchema.Instance.RootTreeShells.SetShellCode(0, 0, maxlevels);

                XmlAttributes attrs = new XmlAttributes();
                attrs.XmlIgnore = true;
                XmlAttributeOverrides attrOverrides = new XmlAttributeOverrides();
                attrOverrides.Add(typeof(Caliburn.Micro.PropertyChangedBase), "IsNotifying", attrs);

                XmlSerializer xmlser = new XmlSerializer(typeof(BaseShell), attrOverrides);

                StringBuilder sb = new StringBuilder();
                System.IO.StringWriter sw = new System.IO.StringWriter(sb);
                xmlser.Serialize(sw, (BaseShell)DbSchema.Instance.RootTreeShells);

                String query = @"update hierarch_structs set data = @data where code ='main_menu';";

                connection = new Npgsql.NpgsqlConnection(ConfigurationManager.ConnectionStrings["crmConnString"].ConnectionString);
                connection.Open();
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                command.Parameters.Add(new NpgsqlParameter("@data", sb.ToString()));

                int affected = 0;
                affected = command.ExecuteNonQuery();

                if (affected == 0)
                    throw new Exception("Save failed!");

                //reset shell_code & level
                query = @"update shells set shell_code = NULL, shell_class = NULL, level = NULL, shell_type = NULL, header = NULL;";
                command = new NpgsqlCommand(query, connection);
                command.ExecuteNonQuery();

                Dictionary<String, Shell> shells = new Dictionary<string, Shell>();
                DbSchema.Instance.RootTreeShells.GetShellsAsDict(ref shells);
                foreach (var shellkv in shells)
                {

                    query = @"update shells set shell_code = @shell_code, shell_class = @shell_class, level = @level, shell_type = @shell_type, header = @header, shell_descr =@shell_descr where shell_name = @shell_name;";

                    command = new NpgsqlCommand(query, connection);
                    command.Parameters.Add(new NpgsqlParameter("@shell_code", shellkv.Value.ShellCode));
                    command.Parameters.Add(new NpgsqlParameter("@shell_class", shellkv.Value.ShellClass));
                    command.Parameters.Add(new NpgsqlParameter("@level", shellkv.Value.Level));
                    command.Parameters.Add(new NpgsqlParameter("@shell_type", shellkv.Value.ShellType));
                    command.Parameters.Add(new NpgsqlParameter("@header", shellkv.Value.Header));
                    command.Parameters.Add(new NpgsqlParameter("@shell_name", shellkv.Key));
                    command.Parameters.Add(new NpgsqlParameter("@shell_descr", shellkv.Value.ShellDescr));
                    affected = command.ExecuteNonQuery();

                    if (affected == 0)
                    {
                        query = @"insert into shells (shell_code, shell_class, level, shell_type, header, shell_name, shell_descr) 
                                values (@shell_code, @shell_class, @level, @shell_type, @header, @shell_name, @shell_descr);";
                        command.CommandText = query;
                        affected = command.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (connection != null)
                    connection.Close();
            }
        }
    }
}

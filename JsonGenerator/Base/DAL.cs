using JsonGenerator.DbEntities;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonGenerator
{
    class DAL
    {
        public static List<Object[]> ExecuteQuery(String query)
        {
            List<Object[]> results = new List<object[]>();

            try
            {
                Npgsql.NpgsqlConnection connection = new Npgsql.NpgsqlConnection(ConfigurationManager.ConnectionStrings["crmConnString"].ConnectionString);
                connection.Open();
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                
                NpgsqlDataReader dr = command.ExecuteReader();

                while (dr.Read())
                {
                    Object[] result = new Object[dr.FieldCount];
                    dr.GetValues(result);
                    results.Add(result);
                }
                connection.Close();
            }
            catch
            {
                ;
            }

            return results;
        }


        public static void SaveShell(Shell shell)
        {
            String query = String.Empty;
            if (shell.Id == 0)
            {
                query =
                   @"insert into shells(
                    shell_name, shell_descr, header, shell_code, icon, level, shell_type, controllers, template) 
                    values (@shell_name,
                    @shell_descr,
                    @header,
                    @shell_code,
                    @icon,
                    @level,
                    @shell_type,
                    @controllers,
                    @template);
                    SELECT currval('shells_id_seq');";
            }
            else
            {
                query =
                    @"update shells set 
                    shell_name = @shell_name,
                    shell_descr = @shell_descr,
                    header = @header,
                    shell_code = @shell_code,
                    icon = @icon,
                    level = @level,
                    shell_type = @shell_type,
                    controllers = @controllers,
                    template = @template 
                where id = @shellid";
            }
            Npgsql.NpgsqlConnection connection = new Npgsql.NpgsqlConnection(ConfigurationManager.ConnectionStrings["crmConnString"].ConnectionString);
            connection.Open();
            NpgsqlCommand command = new NpgsqlCommand(query, connection);
            command.Parameters.Add(new NpgsqlParameter("@template", shell.Template));
            command.Parameters.Add(new NpgsqlParameter("@shellid", shell.Id));
            command.Parameters.Add(new NpgsqlParameter("@shell_name", shell.ShellName));
            command.Parameters.Add(new NpgsqlParameter("@shell_descr", shell.ShellDescr));
            command.Parameters.Add(new NpgsqlParameter("@header", shell.Header));
            //command.Parameters.Add(new NpgsqlParameter("@shell_code", shell.ShellCode));
            command.Parameters.Add(new NpgsqlParameter("@icon", shell.Icon));
            //command.Parameters.Add(new NpgsqlParameter("@level", shell.Level));
            command.Parameters.Add(new NpgsqlParameter("@shell_type", shell.ShellType));
            command.Parameters.Add(new NpgsqlParameter("@controllers", shell.Controllers));

            int affected = 0;
            if (shell.Id.HasValue == false)
            {
                Object obj = command.ExecuteScalar();
                shell.Id = Convert.ToInt32(obj);
                affected = shell.Id.Value;
            }
            else
            {
                affected = command.ExecuteNonQuery();
            }

            connection.Close();

            if (affected == 0)
                throw new Exception("Save failed!");
        }

        public static void DeleteShell(Shell shell)
        {
            String query = @"delete from shells where id = @shellid";
            Npgsql.NpgsqlConnection connection = new Npgsql.NpgsqlConnection(ConfigurationManager.ConnectionStrings["crmConnString"].ConnectionString);
            connection.Open();
            NpgsqlCommand command = new NpgsqlCommand(query, connection);
            command.Parameters.Add(new NpgsqlParameter("@shellid", shell.Id));
            int affected = command.ExecuteNonQuery();
            connection.Close();
            if (affected == 0)
                throw new Exception("Delete failed!");
        }

        public static DataTable CreateMessagesTable()
        {
            DataTable dt = new DataTable("lang_objs");
            dt.Columns.Add(new DataColumn("lang_code", typeof(String)));
            dt.Columns.Add(new DataColumn("lang_obj_name", typeof(String)));
            dt.Columns.Add(new DataColumn("lang_obj_descr", typeof(String)));
            return dt;            
        }

        public static void UpdateMessages(DataTable dt)
        {
            String updatequery = @"update lang_objs set lang_obj_descr = @lang_obj_descr where lang_code = @lang_code and lang_obj_name = @lang_obj_name";
            String insertquery = @"insert into lang_objs (lang_code, lang_obj_name, lang_obj_descr) values (@lang_code, @lang_obj_name, @lang_obj_descr)";
            String deletequery = @"delete from lang_objs where lang_code = @lang_code and lang_obj_name = @lang_obj_name";
            Npgsql.NpgsqlConnection connection = new Npgsql.NpgsqlConnection(ConfigurationManager.ConnectionStrings["crmConnString"].ConnectionString);
            connection.Open();
            NpgsqlDataAdapter da = new NpgsqlDataAdapter();
            NpgsqlCommand  command = new NpgsqlCommand(updatequery, connection);
            command.Parameters.Add(new NpgsqlParameter("@lang_code", NpgsqlTypes.NpgsqlDbType.Varchar, 5, "lang_code"));
            command.Parameters.Add(new NpgsqlParameter("@lang_obj_name", NpgsqlTypes.NpgsqlDbType.Varchar, 50, "lang_obj_name"));
            command.Parameters.Add(new NpgsqlParameter("@lang_obj_descr", NpgsqlTypes.NpgsqlDbType.Varchar, 100, "lang_obj_descr"));
            da.UpdateCommand = command;

            command = new NpgsqlCommand(insertquery, connection);
            command.Parameters.Add(new NpgsqlParameter("@lang_code", NpgsqlTypes.NpgsqlDbType.Varchar, 5, "lang_code"));
            command.Parameters.Add(new NpgsqlParameter("@lang_obj_name", NpgsqlTypes.NpgsqlDbType.Varchar, 50, "lang_obj_name"));
            command.Parameters.Add(new NpgsqlParameter("@lang_obj_descr", NpgsqlTypes.NpgsqlDbType.Varchar, 100, "lang_obj_descr"));
            da.InsertCommand = command;

            command = new NpgsqlCommand(deletequery, connection);
            command.Parameters.Add(new NpgsqlParameter("@lang_code", NpgsqlTypes.NpgsqlDbType.Varchar, 5, "lang_code"));
            command.Parameters.Add(new NpgsqlParameter("@lang_obj_name", NpgsqlTypes.NpgsqlDbType.Varchar, 50, "lang_obj_name"));
            da.DeleteCommand = command;

            int affected = da.Update(dt);
            connection.Close();
            if (affected == 0)
                throw new Exception("Update failed!");
        }

        public static DataTable FetchMessages(String langCode)
        {
            String selectquery = @"select * from lang_objs where lang_code = @lang_code;";
            
            Npgsql.NpgsqlConnection connection = new Npgsql.NpgsqlConnection(ConfigurationManager.ConnectionStrings["crmConnString"].ConnectionString);
            connection.Open();
            NpgsqlDataAdapter da = new NpgsqlDataAdapter();
            NpgsqlCommand command = new NpgsqlCommand(selectquery, connection);
            command.Parameters.Add(new NpgsqlParameter("@lang_code", langCode));
            da.SelectCommand = command;

            DataTable dt = new DataTable("lang_objs");
            var r = da.Fill(dt);
            connection.Close();

            return dt;
        }

        public static void ImportMessages(String langCode)
        {
            DataTable dt = DAL.CreateMessagesTable();
            var fileRoot = System.Configuration.ConfigurationManager.AppSettings["messages_file_root"] as String;
            var languageCode = System.Configuration.ConfigurationManager.AppSettings["languageCode"] as String;
            var messagesFile = System.IO.Path.Combine(fileRoot, String.Format("messages_{0}", langCode));

            using (StreamReader reader = new StreamReader(messagesFile))
            {
                while (reader.EndOfStream == false)
                {
                    String temp = reader.ReadLine();
                    if (String.IsNullOrWhiteSpace(temp))
                        continue;

                    String[] langObjs = temp.Split(new char[] { '=' });
                    DataRow dr = dt.NewRow();
                    dr.ItemArray = new object[] { langCode, langObjs[0], langObjs[1] };
                    dt.Rows.Add(dr);
                }

            }
            DAL.UpdateMessages(dt);
        }

        public static void SaveMessages(List<LangObj> messages)
        {
            DataTable dt = DAL.CreateMessagesTable();
            var languageCode = System.Configuration.ConfigurationManager.AppSettings["languageCode"] as String;
            foreach (var message in messages)
            {
                DataRow dr = dt.NewRow();
                dr.ItemArray = new object[] { languageCode, message.LangObjName, message.LangObjDescr  };
                dt.Rows.Add(dr);
                dr.AcceptChanges();
                if (message.Id > 0)
                    dr.SetModified();
                else
                    dr.SetAdded();
                
            }
            DAL.UpdateMessages(dt);
        }

        public static void ExportMessages(String langCode)
        {
            DataTable dt = DAL.FetchMessages(langCode);

            var fileRoot = System.Configuration.ConfigurationManager.AppSettings["messages_file_root"] as String;
            var languageCode = System.Configuration.ConfigurationManager.AppSettings["languageCode"] as String;
            var messagesFile = System.IO.Path.Combine(fileRoot, String.Format("messages_{0}", langCode));

            using (StreamWriter writer = new StreamWriter(messagesFile, false))
            {
                foreach (DataRow row in dt.Rows)
                {
                    String lang_obj_name = row["lang_obj_name"] as String;
                    String lang_obj_descr = row["lang_obj_descr"] as String;
                    writer.WriteLine(String.Format("{0}={1}", lang_obj_name, lang_obj_descr));
                }
                writer.Flush();
            }
            
        }
    }


}


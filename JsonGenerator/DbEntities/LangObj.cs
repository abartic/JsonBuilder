using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonGenerator.DbEntities
{
    public class LangObj
    {
        public LangObj(int id, string langObjName, string langObjDescr)
        {
            Id = id;
            LangObjName = langObjName;
            LangObjDescr = langObjDescr;
        }

        public Int32 Id { get; set; }

        public String LangObjName { get; set; }

        public String LangObjDescr { get; set; }

        public static Dictionary<String, LangObj> FetchLangObjs()
        {
            var languageCode = System.Configuration.ConfigurationManager.AppSettings["languageCode"] as String;

            var select = DAL.ExecuteQuery(
                String.Format(@"SELECT id, lang_obj_name, lang_obj_descr  FROM lang_objs where lang_code = '{0}';", languageCode));
            List<LangObj> objs = new List<LangObj>();
            
            select.ToList().ForEach(so => objs.Add(new LangObj((Int32)so[0], (String)so[1], (String)so[2])));
            var dict = new Dictionary<String, LangObj>(StringComparer.CurrentCultureIgnoreCase);
            objs.ForEach(e => dict.Add(e.LangObjName, e));
            return dict;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonGenerator.DbEntities
{
    class Selector
    {
        public String Type { get; set; }

        public Selector(String type)
        {
            this.Type = type;
        }

        public static List<Selector> FetchSelectors()
        {
            var select = DAL.ExecuteQuery(@"SELECT distinct type  FROM enum_values order by 1;");
            List<Selector> selectors = new List<Selector>();
            selectors.Add(new Selector(null));
            select.ToList().ForEach(so => selectors.Add(new Selector((String)so[0])));
            return selectors;
        }
    }
}

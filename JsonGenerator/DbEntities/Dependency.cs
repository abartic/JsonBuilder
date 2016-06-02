using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonGenerator.DbEntities
{
    class Dependency
    {
        public String Type { get; set; }
        
        

        public Dependency(String type)
        {
            this.Type = type;
            
        }

        public static List<Dependency> FetchDependencies()
        {
            var select = DAL.ExecuteQuery(@"SELECT distinct type  FROM enum_values ORDER BY type;");
            List<Dependency> dependencies = new List<Dependency>();
            select.ToList().ForEach(so => dependencies.Add(new Dependency((String)so[0])));
            return dependencies;
        }
    }
}

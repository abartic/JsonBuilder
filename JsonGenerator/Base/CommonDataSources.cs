using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonGenerator.Base
{
    class CommonDataSources
    {
        
        public Caliburn.Micro.BindableCollection<String> Types { get; private set; }
        public Caliburn.Micro.BindableCollection<String> DbTypes { get; private set; }
        public Caliburn.Micro.BindableCollection<String> Controls { get; private set; }
        public Caliburn.Micro.BindableCollection<String> DetailsControls { get; private set; }
        public Caliburn.Micro.BindableCollection<String> ShellTypes { get; private set; }
        public Caliburn.Micro.BindableCollection<String> PersistTypes { get; private set; }

        private CommonDataSources()
        { }

        static CommonDataSources instance = null;
        public static CommonDataSources Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CommonDataSources();
                }
                return instance;
            }
        }

        public void LoadData()
        {
            Types = new BindableCollection<string>() { "string", "date", "time", "number", "boolean" };
            PersistTypes = new BindableCollection<string>() { String.Empty, "changes-only" };
            DbTypes = new BindableCollection<string>() { String.Empty, "time" };
            Controls = new BindableCollection<string>() { String.Empty, "datagrid", "textarea", "combobox", "accordion", "treeview", "label", "radiobtn_list", "imagepicker", "password_editor" };
            DetailsControls = new BindableCollection<string>() { String.Empty, "datagrid", "treeview" };
            ShellTypes = new BindableCollection<string>() { String.Empty, "system", "group", "form", "form_filter", "custom" };
        }
    }
}

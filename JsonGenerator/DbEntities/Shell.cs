using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace JsonGenerator.DbEntities
{

    [XmlRoot(ElementName = "item", Namespace ="")]
    public class BaseShell : Caliburn.Micro.PropertyChangedBase
    {

        String shellName;
        [XmlAttribute("shell_name")]
        public String ShellName {
            get
            {
                return shellName;
            }
            set
            {
                shellName = value;
                NotifyOfPropertyChange(() => ShellName);

            }
        }


        String header;
        [XmlAttribute("header")]
        public String Header
        {
            get
            {
                return header;
            }
            set
            {
                header = value;
                NotifyOfPropertyChange(() => Header);

            }
        }

        String shellType;
        [XmlAttribute("shell_type")]
        public String ShellType
        {
            get
            {
                return shellType;
            }
            set
            {
                shellType = value;
                NotifyOfPropertyChange(() => ShellType);

            }
        }

        String shellDescr;

        [XmlAttribute("shell_descr")]
        public String ShellDescr
        {
            get
            {
                return shellDescr;
            }
            set
            {
                shellDescr = value;
                NotifyOfPropertyChange(() => ShellDescr);
            }
        }

        Int64 schellCode;
        [XmlAttribute("shell_code")]
        public Int64 ShellCode
        {
            get
            {
                return schellCode;
            }
            set
            {
                schellCode = value;
            }
        }

        Int32 level;
        [XmlAttribute("level")]
        public Int32 Level
        {
            get
            {
                return level;
            }
            set
            {
                level = value;
            }
        }

        Caliburn.Micro.BindableCollection<Shell> items = null;
        [XmlArray("items", IsNullable = false)]
        [XmlArrayItem("item", Type = typeof(Shell), IsNullable = false)]
        public Caliburn.Micro.BindableCollection<Shell> Items
        {
            get { return items; }
            set
            {
                items = value;
                if (items != null)
                    items.CollectionChanged += Items_CollectionChanged;
            }
        }

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (Shell shell in e.NewItems)
                {
                    shell.Parent = this;
                }
            }
        }

        [XmlIgnore]
        public BaseShell Parent { get; internal set; }

       

    }

    [XmlRoot("item")]
    
    public class Shell : BaseShell
    {
        public Shell()
        { }

        [XmlIgnore]
        public Int32? Id { get; set; }

       

        [XmlIgnore]
        public String Template { get; set; }


        [XmlIgnore]
        public String Icon { get; set; }

        [XmlIgnore]
        public String Controllers { get; set; }

        [XmlAttribute("shell_class")]
        public string ShellClass { get; set; }
        public string FilterSource { get; set; }
        public string FilterPath { get; set; }





        //Boolean isEditing;
        //[XmlIgnore]
        //public bool IsEditing
        //{
        //    get
        //    {
        //        return isEditing;
        //    }
        //    set
        //    {
        //        isEditing = value;
        //        NotifyOfPropertyChange(() => IsEditing);
        //    }
        //}

        //[XmlIgnore]
        // public Boolean IsSelected { get; set; }


        public Shell(Int32 id, 
            String shell_name, String shell_descr, String template, String header, String shell_type, String icon, String controllers,
            Int64 shell_code, Int32 level)
        {
            this.Id = id;
            this.ShellName = shell_name;
            this.ShellDescr = shell_descr;
            
            this.Template = template;

            this.Header = header;
            this.ShellType = shell_type;
            this.Icon = icon;
            this.Controllers = controllers;
            this.ShellCode = shell_code;
            this.Level = level;

        }

        public static List<Shell> FetchShells()
        {
            var select = DAL.ExecuteQuery(@"SELECT id, shell_name, shell_descr, template, header, shell_type, icon, controllers, shell_code,level FROM shells order by shell_name asc;");
            var shells = select.ToList().Select(so => new Shell(
                (Int32)so[0], 
                (String)so[1],
                so[2] != DBNull.Value ? (String)so[2] : "",
                
                so[3] != DBNull.Value ? (String)so[3] : "",
                so[4] != DBNull.Value ? (String)so[4] : "",
                so[5] != DBNull.Value ? (String)so[5] : "",
                so[6] != DBNull.Value ? (String)so[6] : "", 
                so[7] != DBNull.Value ? (String)so[7] : "",
                so[8] != DBNull.Value ? (Int64)so[8] : 0,
                so[9] != DBNull.Value ? (Int32)so[9] : 0)).ToList();
            return shells;
        }

        public void SetShellCode(Int64 shellcode, Int32 index, Int32 maxlevels)
        {
            if (this.Level == 0)
            {
                this.ShellCode = 0;
                this.Level = -1;
                this.ShellClass = this.ShellName;
            }
            else
            {
                this.ShellCode = shellcode + ((this.Level == 1 ? 10 : 0) + index) * (Int32)Math.Pow(100, maxlevels - this.Level + 1);
                this.Level = maxlevels - this.Level + 1;
                this.ShellClass = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(this.ShellName.ToLower().Replace("_", " ")).Replace(" ", "");

            }

            if (this.Items != null)
            {
                if (this.Items.Any() == false)
                {
                    this.Items = null;
                }
                else
                {
                    int j = 0;
                    this.Items.ToList().ForEach(i =>
                    {
                        i.SetShellCode(this.ShellCode, ++j, maxlevels);
                    });
                }
            }
            
        }


        public int SetLevel(int level = 0)
        {
            this.Level = level;
            int maxlevel = level;
            
            if (this.Items != null)
            {
                level++;
                foreach (var item in this.Items)
                {
                    var inner_level = item.SetLevel(level);
                    if (inner_level > maxlevel)
                        maxlevel = inner_level;
                }
            }

            return maxlevel;
        }

        public void GetShellsAsDict(ref Dictionary<String, Shell> shells)
        {
            shells.Add(this.ShellName, this);
            if (this.Items != null)
            {
                foreach(var item in this.Items)
                    item.GetShellsAsDict(ref shells);
            }
        }
    }


}

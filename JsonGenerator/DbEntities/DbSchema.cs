using Caliburn.Micro;
using JsonGenerator.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonGenerator.DbEntities
{
    class DbSchema
    {
        public Dictionary<String, LangObj> LangObjs { get; private set; }

        public Caliburn.Micro.BindableCollection<Table> Tables { get; private set; }
        public Caliburn.Micro.BindableCollection<Table> Entities { get; private set; }
        public Caliburn.Micro.BindableCollection<Shell> Shells { get; private set; }
        public Caliburn.Micro.BindableCollection<Relation> Relations { get; private set; }
        public Caliburn.Micro.BindableCollection<Column> Columns { get; private set; }
        public Caliburn.Micro.BindableCollection<Dependency> Dependencies { get; private set; }
        public Caliburn.Micro.BindableCollection<Selector> Selectors { get; private set; }
        public Shell RootTreeShells { get; private set; }

        public Caliburn.Micro.BindableCollection<Shell> TreeShells { get { return RootTreeShells.Items; } }

        private DbSchema()
        { }

        static DbSchema instance = null;
        public static DbSchema Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DbSchema();
                }
                return instance;
            }
        }

        public void LoadSchema()
        {
            Shells = new Caliburn.Micro.BindableCollection<Shell>(Shell.FetchShells());
            Tables = new Caliburn.Micro.BindableCollection<Table>(Table.FetchTables());
            Entities = new Caliburn.Micro.BindableCollection<Table>(Table.FetchEntities());
            Relations = new Caliburn.Micro.BindableCollection<Relation>(Relation.FetchRelations());
            Columns = new Caliburn.Micro.BindableCollection<Column>(Column.FetchColumns());
            Dependencies = new Caliburn.Micro.BindableCollection<Dependency>(Dependency.FetchDependencies());
            Selectors = new Caliburn.Micro.BindableCollection<Selector>(Selector.FetchSelectors());
            LangObjs = LangObj.FetchLangObjs();
            //DAL.ImportMessages("en-US");
        }

        public void LoadTreeShells()
        {
            Shells = new Caliburn.Micro.BindableCollection<Shell>(Shell.FetchShells());
            var mainshell = ShellsTree.FetchShellsTree();
            RootTreeShells = mainshell; // CreateTreeShells(mainshell);
        }
        Shell CreateTreeShells(Shell p_shell)
        {
            Shell e_shell = Shells.FirstOrDefault(s => String.Compare(s.ShellName, p_shell.ShellName, StringComparison.InvariantCultureIgnoreCase) == 0);
            e_shell.ShellDescr = p_shell.ShellDescr;
            e_shell.ShellType = p_shell.ShellType;
            e_shell.ShellName = p_shell.ShellName;
            e_shell.Header = p_shell.Header;
            if (e_shell == null)
                return null;
            
            if (p_shell.Items != null)
            {
                
                foreach (Shell i_shell in p_shell.Items)
                {
                    Shell shell = CreateTreeShells(i_shell);
                    if (shell != null)
                    {
                        if (e_shell.Items == null)
                            e_shell.Items = new BindableCollection<Shell>();
                        e_shell.Items.Add(shell);
                    }
                }
            }
            return e_shell;
        }
    }


    
}

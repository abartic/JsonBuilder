using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Npgsql;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Configuration;
using Caliburn.Micro;

using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Windows;
using JsonGenerator.DbEntities;
using JsonGenerator.Base;
using JsonGenerator.JsonEntities;
using System.Xml;

namespace JsonGenerator
{

    [Export("TreeShellsViewModel", typeof(TreeShellsViewModel))]
    class TreeShellsViewModel : Screen
    {

        //[ImportingConstructor]
        public TreeShellsViewModel(Boolean isEditable)
        {
            this.IsEditable = isEditable;
            DbSchema.Instance.LoadTreeShells();
            if (DbSchema.Instance.TreeShells != null)
                SelectedItem = DbSchema.Instance.TreeShells.FirstOrDefault();
        }

        JsonBuilderViewModel jsonModel = null;
        public JsonBuilderViewModel JsonModel
        {
            get { return jsonModel; }
            set
            {
                jsonModel = value;
                NotifyOfPropertyChange(() => JsonModel);
            }
        }

        Shell shell = null;
        public Shell Shell
        {
            get { return shell; }
            set
            {
                shell = value;
                NotifyOfPropertyChange(() => Shell);
            }
        }

        Shell selectedShell = null;
        public Shell SelectedItem
        {
            get { return selectedShell; }
            set
            {
                selectedShell = value;
                NotifyOfPropertyChange(() => SelectedItem);
            }
        }

        public Boolean Canceled { get; set; }

        public Boolean isEditable;
        public Boolean IsEditable {
            get { return isEditable; }
            set
            {
                isEditable = value;
                NotifyOfPropertyChange(() => IsEditable);
            }
        }


        public void Select()
        {
            Canceled = false;
            TryClose(false);
        }

        public void Cancel()
        {
            Canceled = true;
            TryClose(false);
        }

        public void OnSelectionChangedAction(ActionExecutionContext context)
        {
            RoutedPropertyChangedEventArgs<Object> keyargs = context.EventArgs as RoutedPropertyChangedEventArgs<Object>;
            if (keyargs != null)
            {
                SelectedItem = keyargs.NewValue as Shell;
            }
        }

        //public void MakeShellCode()
        //{
        //    Canceled = false;
        //    TryClose(false);
        //}

        public void NewChild()
        {
            if (SelectedItem == null)
                return;

            Shell newchild = new Shell(0, "<new>", "", "", "", "form_filter", "", "",0,0);
            if (SelectedItem.Items == null)
                SelectedItem.Items = new BindableCollection<Shell>();
            //newchild.IsSelected = true;

            SelectedItem.Items.Add(newchild);
            SelectedItem = newchild;
        }

        public void NewSibling()
        {
            if (SelectedItem == null) 
                return;

            Shell newsibling = new Shell(0, "<new>", "", "", "", SelectedItem.ShellType, "", "",0,0);
            //newsibling.IsSelected = true;

            SelectedItem.Parent.Items.Add(newsibling);
            SelectedItem = newsibling;
        }

        public void Delete()
        {
            if (SelectedItem == null)
                return;

            if (SelectedItem.Parent == null)
                DbSchema.Instance.TreeShells.Remove(this.SelectedItem);
            else
                SelectedItem.Parent.Items.Remove(this.SelectedItem);
            SelectedItem = DbSchema.Instance.TreeShells.FirstOrDefault();
        }

        public void MoveUp()
        {
            BindableCollection<Shell> shells = null;
            BaseShell parent = this.SelectedItem.Parent;
            if (parent == null)
            {
                shells = DbSchema.Instance.TreeShells;
            }
            else
            {
                shells = parent.Items;
            }
            Int32 index = shells.IndexOf(this.SelectedItem);
            if (index > 0)
                shells.Move(index, --index);
        }

        public void MoveDown()
        {
            BindableCollection<Shell> shells = null;
            BaseShell parent = this.SelectedItem.Parent;
            if (parent == null)
            {
                shells = DbSchema.Instance.TreeShells;
            }
            else
            {
                shells = parent.Items;
            }
            Int32 index = shells.IndexOf(this.SelectedItem);
            if (index < (shells.Count - 1))
                shells.Move(index, ++index);
        }

        public void Save()
        {
            ShellsTree.SaveShellsTree();
        }
    }
}

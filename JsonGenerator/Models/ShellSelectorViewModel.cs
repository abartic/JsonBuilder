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

namespace JsonGenerator
{

    [Export("ShellSelectorViewModel", typeof(ShellSelectorViewModel))]
    class ShellSelectorViewModel : Screen
    {
        public ShellSelectorViewModel()
        {
            this.Shells = DbSchema.Instance.Shells;
        }

        Shell selectedShell = null;
        public Shell SelectedShell
        {
            get { return selectedShell; }
            set
            {
                selectedShell = value;
                NotifyOfPropertyChange(() => SelectedShell);
            }
        }

        public BindableCollection<Shell> Shells { get; set; }

        public void New()
        {
            selectedShell = null;
            TryClose(true);
        }

        public void Select()
        {
            TryClose(true);    
        }

        public void Cancel()
        {
            Canceled = true;
            TryClose(false);
        }
        
        public Boolean Canceled { get; set; }
    }
    
}

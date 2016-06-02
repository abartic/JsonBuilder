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

    //[Export(typeof(ShellViewModel))]
    [Export(typeof(IShell))]
    class ShellViewModel : Conductor<object>, IShell, IViewAware
    {
        public IWindowManager WindowManager { get; set; }
        public IEventAggregator EventAggregator { get; set; }

        [ImportingConstructor]
        public ShellViewModel(IWindowManager windowManager, IEventAggregator eventAggregator)
        {
            this.WindowManager = windowManager;

            this.EventAggregator = eventAggregator;
            
        }

        //JsonBuilderViewModel jsonModel = null;
        //public JsonBuilderViewModel JsonModel
        //{
        //    get { return jsonModel; }
        //    set
        //    {
        //        jsonModel = value;
        //        NotifyOfPropertyChange(() => JsonModel);
        //    }
        //}

        Shell shell = null;
        public Shell Shell
        {
            get { return shell; }
            set {
                shell = value;
                NotifyOfPropertyChange(() => Shell);
            }
        }
        
        
        public void EditShell()
        {

            ShellSelectorViewModel shellSelectorViewModel = new ShellSelectorViewModel();
            WindowManager.ShowDialog(shellSelectorViewModel, "Master");
            if (shellSelectorViewModel.Canceled == false)
            {
                var jsonModel = new JsonBuilderViewModel(this, shellSelectorViewModel.SelectedShell, this.WindowManager);
                ActivateItem(jsonModel);
            }
            
        }

        Boolean isLoadingShowed = false;
        public Boolean IsLoadingShowed
        {
            get
            {
                return isLoadingShowed;
            }
            set
            {
                isLoadingShowed = value;
                NotifyOfPropertyChange(()=>IsLoadingShowed);
            }
        }

       

        //public void Delete()
        //{
        //    try
        //    {
        //        if (MessageBox.Show("Delete the shell?", App.Current.MainWindow.Title, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        //            DAL.DeleteShell(this.jsonModel.Shell);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}

        public void Exit()
        {
            Application.Current.MainWindow.Close();
        }

        protected override void OnViewReady(Object view)
        {
            base.OnActivate();
            Caliburn.Micro.Action.Invoke(this, "RefreshDbSchema");
        }

        //public void NewShell()
        //{
        //    this.jsonModel = new JsonBuilderViewModel(null, this.WindowManager);
        //    ActivateItem(this.jsonModel);
        //}

        public void EditTreeShells()
        {
            TreeShellsViewModel treeShellViewModel = new TreeShellsViewModel(true);
            ActivateItem(treeShellViewModel);
        }

        public IEnumerable<IResult> RefreshDbSchema()
        {
            yield return new BaseCoRoutine(delegate ()
            {
                this.IsLoadingShowed = true;
            });
            yield return new BackgroundCoRoutine(delegate ()
            {
                CommonDataSources.Instance.LoadData();
                DbSchema.Instance.LoadSchema();


            });
            yield return new BaseCoRoutine(delegate ()
            {
                this.IsLoadingShowed = false;
            });

        }

        public IEnumerable<IResult> SaveLangObjs()
        {
            yield return new BaseCoRoutine(delegate ()
            {
                this.IsLoadingShowed = true;
            });
            yield return new BackgroundCoRoutine(delegate ()
            {
                DAL.ExportMessages("ro-RO");
                //DAL.ExportMessages("en-US");
            });
            yield return new BaseCoRoutine(delegate ()
            {
                this.IsLoadingShowed = false;
            });

        }
    }

    public interface IShell
    {
    }
}

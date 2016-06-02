using Caliburn.Micro;
using JsonGenerator.Base;
using JsonGenerator.DbEntities;
using JsonGenerator.JsonEntities;
using JsonGenerator.Language;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace JsonGenerator
{

    [Export(typeof(JsonBuilderViewModel))]
    class JsonBuilderViewModel : Screen
    {
        public Shell shell { get; set; }
        public IWindowManager WindowManager { get; set; }

        public Shell Shell
        {
            get
            {
                return shell;
            }
            set
            {
                shell = value;
                NotifyOfPropertyChange(() => Shell);
            }
        }

        public Caliburn.Micro.BindableCollection<ShellTable> ShellTables { get; set; }
        public DbSchema DbSchema
        {
            get
            {
                return DbSchema.Instance;
            }
        }

        ShellViewModel ShellViewModel { get; set; }

        [ImportingConstructor]
        public JsonBuilderViewModel(ShellViewModel shellViewModel, Shell shell, IWindowManager windowManager)
        {
            this.ShellViewModel = ShellViewModel;
            this.WindowManager = windowManager;

            if (shell == null)
            {
                this.shell = new Shell(0, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty,0,0);
            }
            else
            {
                this.Shell = shell;
                if (String.IsNullOrWhiteSpace(shell.Template) == false)
                {
                    ReadJsonPackage();
                }
            }

            if (this.JsonPackage != null)
            {
                this.isMultiTable = this.JsonPackage.Details != null;
                this.MainTable = this.DbSchema.Tables.FirstOrDefault(t => String.Compare(t.TableName, this.JsonPackage.Header.Entity, StringComparison.InvariantCultureIgnoreCase) == 0);
                ShellTables = new BindableCollection<ShellTable>();
                var mainShellTable = new ShellTable(this, this.MainTable, this.JsonPackage.Header);

                //actions
                mainShellTable.ShellStdActions = new BindableCollection<JsonPackageAction>(this.JsonPackage.Header.StdActions ?? JsonPackage.HeaderStdActions);
                if (this.JsonPackage.Header.Actions != null)
                    mainShellTable.ShellActions = new BindableCollection<JsonPackageAction>(this.JsonPackage.Header.Actions);
                else
                    mainShellTable.ShellActions = new BindableCollection<JsonPackageAction>();
                if (this.JsonPackage.Header.Panels != null)
                {
                    mainShellTable.ShellPanels = new BindableCollection<JsonPackageLayoutPanel>();
                    int row = 1;
                    foreach (var panels in this.JsonPackage.Header.Panels)
                    {
                        foreach (var panel in panels)
                        {
                            panel.Row = row;
                            mainShellTable.ShellPanels.Add(panel);
                        }
                        row++;
                    }
                }
                else
                {
                    mainShellTable.ShellPanels = new BindableCollection<JsonPackageLayoutPanel>();
                }

                this.mainShellTable = mainShellTable;
                ShellTables.Add(mainShellTable);

                this.SelectedShellTable = ShellTables.FirstOrDefault();

                if (this.JsonPackage.Details != null)
                {
                    this.JsonPackage.Details.ToList().ForEach(dt =>
                    {
                        var relation = this.DbSchema.Relations.FirstOrDefault(r => String.Compare(r.RelationProperty, dt.Relation, StringComparison.InvariantCultureIgnoreCase) == 0);
                        var detailShellTable = new ShellTable(this, relation, dt);
                        mainShellTable.Items.Add(detailShellTable);
                        detailShellTable.ShellStdActions = new BindableCollection<JsonPackageAction>(dt.StdActions ?? JsonPackage.DetailStdActions);
                        if (dt.Actions != null)
                            detailShellTable.ShellActions = new BindableCollection<JsonPackageAction>(dt.Actions);
                        else
                            detailShellTable.ShellActions = new BindableCollection<JsonPackageAction>();

                        if (dt.Panels != null)
                        {
                            detailShellTable.ShellPanels = new BindableCollection<JsonPackageLayoutPanel>();
                            int row = 1;
                            foreach (var panels in dt.Panels)
                            {
                                foreach (var panel in panels)
                                {
                                    panel.Row = row;
                                    detailShellTable.ShellPanels.Add(panel);
                                }
                                row++;
                            }
                        }
                        else
                        {
                            detailShellTable.ShellPanels = new BindableCollection<JsonPackageLayoutPanel>();
                        }
                    });
                }
                else
                {
                    this.SelectedShellTable.Items.Clear();
                }
                
                
                
                this.RelationTables = new BindableCollection<Table>(this.ShellTables.First().Items.Select(i => i.Table));

                var dependencies = this.DbSchema.Dependencies.Where(d => this.JsonPackage.Dependencies.Contains(d.Type));
                this.ShellDependencies = new BindableCollection<Dependency>(dependencies);

                this.IsMultiTable = this.SelectedShellTable.Items.Any();

                this.Shell.FilterSource = this.JsonPackage.Filter.FilterSource;
                this.Shell.FilterPath = this.JsonPackage.Filter.FilterPath;
            }
            else
            {
                this.JsonPackage = new JsonPackage();

                this.JsonPackage.Filter = new JsonPackageFilter();
               

                this.MainTable = this.DbSchema.Tables.FirstOrDefault();
                ShellTables = new BindableCollection<ShellTable>();

                var mainShellTable = new ShellTable(this, this.MainTable, null);
                mainShellTable.ShellStdActions = new BindableCollection<JsonPackageAction>(JsonPackage.HeaderStdActions);
                mainShellTable.ShellActions = new BindableCollection<JsonPackageAction>();
                this.mainShellTable = mainShellTable;
                ShellTables.Add(mainShellTable);

                this.DbSchema.Relations
                    .Where(r => String.Compare(r.SourceTableName, this.MainTable.TableName, StringComparison.InvariantCultureIgnoreCase) == 0)
                    .ToList()
                    .ForEach(r => {
                        var detailShellTable = new ShellTable(this, r, null);
                        mainShellTable.Items.Add(detailShellTable);
                        detailShellTable.ShellStdActions = new BindableCollection<JsonPackageAction>(JsonPackage.DetailStdActions);
                        detailShellTable.ShellActions = new BindableCollection<JsonPackageAction>();
                    });

                this.SelectedShellTable = mainShellTable;
                this.RelationTables = new BindableCollection<Table>(this.ShellTables.First().Items.Select(i => i.Table));
                this.ShellDependencies = new BindableCollection<Dependency>();

                this.JsonPackage.Header = new JsonPackageHeader() { Entity = this.MainTable.EntityName, Items = mainShellTable.JsonPackageItems.ToArray() };
            }

            SetTranslation(mainShellTable);
        }

        void SetTranslation(ShellTable table)
        {
            foreach (var i in table.JsonPackageItems)
            {
                var lang_obj_name = String.Format("FRM_{0}_{1}", table.EntityInfo, i.Field).ToUpper();
                LangObj langObj = null;
                if (i.Title == null || i.Title.StartsWith("FRM_") == false)
                    i.Title = lang_obj_name;

                if (DbSchema.LangObjs.TryGetValue(lang_obj_name, out langObj))
                {
                    i.TitleTranslation = langObj.LangObjDescr;
                    i.LangObj = langObj;
                }
                else
                {
                    i.LangObj = new LangObj(0, lang_obj_name,lang_obj_name);
                    if (i.Field.Contains("_"))
                        i.TitleTranslation = Translator.Instance.Translate(i.Field.Replace("_", ""));
                    else
                        i.TitleTranslation = Translator.Instance.Translate(i.Field);
                    i.TitleTranslation = string.Format("{0}{1}", Char.ToUpper(i.TitleTranslation[0]), i.TitleTranslation.Substring(1));
                }
            }
            
            foreach (var a in table.ShellActions)
            {
                String lang_obj_name = null;
                if (a.Caption.StartsWith("FRM_") != true && a.IsStandard == false)
                    lang_obj_name = String.Format("FRM_{0}_ACT_{1}", table.EntityInfo, a.Caption).ToUpper();
                else
                    lang_obj_name = a.Caption;

                LangObj langObj = null;
                
                if (DbSchema.LangObjs.TryGetValue(lang_obj_name, out langObj))
                {
                    a.CaptionTranslation = langObj.LangObjDescr;
                    a.LangObj = langObj;
                }
                else
                {
                    a.LangObj = new LangObj(0, lang_obj_name, lang_obj_name);
                }
            }
            foreach (var panel in table.ShellPanels)
            {
                String lang_obj_name = null;
                if (panel.Header.StartsWith("FRM_") != true)
                    lang_obj_name = String.Format("FRM_{0}_PNL_{1}", table.EntityInfo, panel.Header).ToUpper();
                else
                    lang_obj_name = panel.Header;
                LangObj langObj = null;

                if (DbSchema.LangObjs.TryGetValue(lang_obj_name, out langObj))
                {
                    panel.HeaderTranslation = langObj.LangObjDescr;
                    panel.LangObj = langObj;
                }
                else
                {
                    panel.LangObj = new LangObj(0, lang_obj_name, lang_obj_name);
                }
            }
            foreach (var relationTable in table.Items)
                SetTranslation(relationTable);
        }

        public JsonPackage JsonPackage { get; set; }
        private void ReadJsonPackage()
        {
            try
            {
                MemoryStream stream = new MemoryStream();
                StreamWriter writer = new StreamWriter(stream);
                writer.Write(shell.Template);
                writer.Flush();
                stream.Position = 0;
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(JsonPackage));
                this.JsonPackage = ser.ReadObject(stream) as JsonPackage;
                stream.Close();
            }
            catch {
                this.JsonPackage = null;
            }
        }

        public void LoadDbSchema()
        {
            DbSchema.LoadSchema();
            NotifyOfPropertyChange(() => DbSchema);
        }

        public Boolean CanNewDetail
        {
            get
            {
                return IsMultiTable && SelectedShellTable != null && SelectedShellTable.Relation == null;
            }
        }

        public void NewDetail()
        {
            if (this.JsonPackage.Details == null)
                this.JsonPackage.Details = new List<JsonPackageDetail>();
            this.RelationTables = new BindableCollection<Table>();

            var relations = DbSchema.Relations.Where(r => r.SourceTableName == this.MainShellTable.Table.TableName);
            relations.ToList().ForEach(r =>
            {
                var table = DbSchema.Instance.Tables.FirstOrDefault(t => String.Compare(t.TableName, r.TargetTableName) == 0);
                this.RelationTables.Add(table);
                
            });

            var relation = relations.FirstOrDefault();
            var detail = new ShellTable(this, relation, new JsonPackageDetail()
            {
                Items = DbSchema.Columns.Where(c => c.TableName == relation.TargetTableName).Select(c => new JsonPackageItem() { Field = c.FieldName, Nullable = c.IsNullable, Type = c.PropertyType, Column = c, Decimals = c.Decimals }).ToArray()
            });

            this.JsonPackage.Details.Add(detail.PackageDetail);
            SelectedShellTable.Items.Add(detail);

            this.NotifyOfPropertyChange(() => IsMultiTable);

            SetTranslation(detail);
            SelectedShellTable = detail;
        }

        public Boolean CanDeleteDetail
        {
            get
            {
                return IsMultiTable && (SelectedShellTable != null) && (SelectedShellTable.Relation != null);
            }
        }

        public void DeleteDetail()
        {
            if (MessageBox.Show("Delete the selected detail", App.Current.MainWindow.Title, MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;

            if (ShellTables.FirstOrDefault() == null)
                return;

            ShellTables.FirstOrDefault().Items.Remove(this.SelectedShellTable);
            this.SelectedShellTable = ShellTables.First();
            this.NotifyOfPropertyChange(() => IsMultiTable);
        }

        Boolean isMultiTable = false;
        public Boolean IsMultiTable
        {
            get
            {
                return isMultiTable;
            }
            set
            {
                isMultiTable = value;
                NotifyOfPropertyChange(() => IsMultiTable);
                NotifyOfPropertyChange(() => CanNewDetail);
                NotifyOfPropertyChange(() => CanDeleteDetail);
            }
        }

        ShellTable selectedShellTable = null;
        public ShellTable SelectedShellTable
        {
            get { return selectedShellTable; }
            set
            {
                selectedShellTable = value;
                NotifyOfPropertyChange(() => SelectedShellTable);
                NotifyOfPropertyChange(() => CanNewDetail);
                NotifyOfPropertyChange(() => CanDeleteDetail);
                this.IsMainTableSelected = (selectedShellTable.Relation == null);
                this.IsRelationTableSelected = (selectedShellTable.Relation != null);
            }
        }



        Table mainTable = null;
        public Table MainTable
        {
            get { return mainTable; }
            set
            {
                mainTable = value;
                NotifyOfPropertyChange(() => MainTable);
                if (this.ShellTables != null && this.ShellTables.Any())
                {
                    this.RelationTables = new BindableCollection<Table>(this.ShellTables.First().Items.Select(i => i.Table));
                }
            }
        }

        ShellTable mainShellTable = null;
        public ShellTable MainShellTable
        {
            get
            {
                return mainShellTable;
            }
            set
            {
                mainShellTable = value;
                NotifyOfPropertyChange(() => MainShellTable);
            }
        }

        BindableCollection<Table> relationTables = null;
        public BindableCollection<Table> RelationTables
        {
            get
            {
                return relationTables;
            }
            set
            {
                relationTables = value;
                NotifyOfPropertyChange(() => RelationTables);
            }
        }

        Boolean isMainTableSelected = true;
        public Boolean IsMainTableSelected
        {
            get
            {
                return isMainTableSelected;
            }
            set
            {
                isMainTableSelected = value;
                NotifyOfPropertyChange(() => IsMainTableSelected);
            }
        }

        Boolean isRelationTableSelected = true;
        public Boolean IsRelationTableSelected
        {
            get
            {
                return isRelationTableSelected;
            }
            set
            {
                isRelationTableSelected = value;
                NotifyOfPropertyChange(() => IsRelationTableSelected);
            }
        }

        #region Dependency
        public Caliburn.Micro.BindableCollection<Dependency> ShellDependencies { get; set; }

        Dependency selectedShellDependency;
        public Dependency SelectedShellDependency {
            get
            {
                return selectedShellDependency;
            }
            set
            {
                selectedShellDependency = value;
                NotifyOfPropertyChange(() => SelectedShellDependency);
            }
        }

        public void AddDependency()
        {
            if (ShellDependencies == null)
                ShellDependencies = new BindableCollection<Dependency>();

            ShellDependencies.Add(new Dependency(""));
        }

        public void DeleteDependency()
        {
            if (this.SelectedShellDependency != null)
                ShellDependencies.Remove(this.SelectedShellDependency);
        }
        #endregion Dependency

        #region Std Actions

        public void AddShellStdAction()
        {
            if (this.SelectedShellTable != null)
                this.SelectedShellTable.ShellStdActions.Add(new JsonPackageAction());
        }

        public void AddAllShellStdAction()
        {
            this.SelectedShellTable.ShellStdActions.Clear();
            if (this.SelectedShellTable == this.MainShellTable)
            {
                this.SelectedShellTable.ShellStdActions.AddRange(new JsonPackageAction[] {
                new JsonPackageAction() { Name="NewItem",Caption="COMM_EDIT_NEW",UIClass="btn-success", ForAdditionalEntity=true},
                new JsonPackageAction() { Name="UndoItem",Caption="COMM_EDIT_UNDO",UIClass="btn-warning"},
                new JsonPackageAction() { Name="DeleteItem",Caption="COMM_EDIT_DELETE",UIClass="btn-danger"},
                new JsonPackageAction() { Name="UpdateItem",Caption="COMM_EDIT_SAVE",UIClass="btn-primary", ForAdditionalEntity=true }});
            }
            else
            {
                this.SelectedShellTable.ShellStdActions.AddRange(new JsonPackageAction[] {

                    new JsonPackageAction() { Name = "NewDetailItem", Caption = "COMM_NEW_DETAIL", UIClass = "btn-success" , ForAdditionalEntity=true},
                    new JsonPackageAction() { Name = "DeleteDetailItem", Caption = "COMM_DELETE_DETAIL", UIClass = "btn-danger", ForAdditionalEntity=true }});
            }
        }
        
        public void DeleteShellStdAction()
        {
            if (this.SelectedShellTable != null && this.SelectedShellTable.SelectedShellStdAction != null)
                this.SelectedShellTable.ShellStdActions.Remove(this.SelectedShellTable.SelectedShellStdAction);
        }

        #endregion Std Actions

        #region Actions

        public void AddShellAction()
        {
            if (this.SelectedShellTable == null)
                this.SelectedShellTable.ShellActions = new BindableCollection<JsonPackageAction>();

            this.SelectedShellTable.ShellActions.Add(new JsonPackageAction());
        }

        public void DeleteShellAction()
        {
            if (this.SelectedShellTable != null && this.SelectedShellTable.SelectedShellAction != null)
                this.SelectedShellTable.ShellActions.Remove(this.SelectedShellTable.SelectedShellAction);
        }
        #endregion Actions

        #region Items

        JsonPackageItem selectedShellTableItem = null;
        public JsonPackageItem SelectedShellTableItem
        {
            get
            {
                return selectedShellTableItem;
            }
            set

            {
                selectedShellTableItem = value;
                NotifyOfPropertyChange(() => SelectedShellTableItem);
                NotifyOfPropertyChange(() => CanDeleteItem);
            
            }

        }
        public Boolean CanAddItem
        {
            get
            {
                return this.SelectedShellTable != null && this.SelectedShellTable.JsonPackageItems != null;
            }
        }

        public void AddItem()
        {
            if (SelectedShellTable.JsonPackageItems == null)
                SelectedShellTable.JsonPackageItems = new BindableCollection<JsonPackageItem>();

            SelectedShellTable.JsonPackageItems.Add(new JsonPackageItem());
        }

        public Boolean CanDeleteItem
        {
            get
            {
                return this.SelectedShellTableItem != null;
            }
        }

        public void DeleteItem()
        {
            SelectedShellTable.JsonPackageItems.Remove(SelectedShellTableItem);
        }
        #endregion Items

        #region ShellPanel

        JsonPackageLayoutPanel selectedShellPanel = null;
        public JsonPackageLayoutPanel SelectedShellPanel
        {
            get
            {
                return selectedShellPanel;
            }
            set

            {
                selectedShellPanel = value;
                NotifyOfPropertyChange(() => SelectedShellTableItem);
                NotifyOfPropertyChange(() => CanDeleteShellPanel);

            }

        }
        public Boolean CanAddShellPanel
        {
            get
            {
                return this.SelectedShellTable != null;
            }
        }

        public void AddShellPanel()
        {

            if (SelectedShellTable.ShellPanels == null)
                SelectedShellTable.ShellPanels = new BindableCollection<JsonPackageLayoutPanel>();

            
            SelectedShellTable.ShellPanels.Add(new JsonPackageLayoutPanel() { Name = (SelectedShellTable.ShellPanels.Count + 1).ToString()});
        }

        public Boolean CanDeleteShellPanel
        {
            get
            {
                return this.SelectedShellPanel != null;
            }
        }

        

        public void DeleteShellPanel()
        {
            SelectedShellTable.ShellPanels.Remove(SelectedShellPanel);
        }
        #endregion ShellPanel

        public void SelectShellName() 
        {
            TreeShellsViewModel treeShellViewModel = new TreeShellsViewModel(false);
            WindowManager.ShowDialog(treeShellViewModel);

            if (treeShellViewModel.Canceled == false && treeShellViewModel.SelectedItem != null)
            {
                //this.Shell.ShellCode = treeShellViewModel.SelectedItem.ShellCode;
                this.Shell.Header = treeShellViewModel.SelectedItem.Header;
                this.Shell.ShellDescr = treeShellViewModel.SelectedItem.ShellDescr;
                this.Shell.ShellName = treeShellViewModel.SelectedItem.ShellName;
            }
        }

        public void Save()
        {
            this.SaveJson();
        }

      

        public void SaveJson()
        {
            List<LangObj> messages = new List<LangObj>();
            this.JsonPackage = new JsonPackage();


            List<JsonPackageLayoutPanel[]> rowpanels = new List<JsonPackageLayoutPanel[]>();
            if (this.MainShellTable.Items != null && this.MainShellTable.Items.Any())
            {
                rowpanels = new List<JsonPackageLayoutPanel[]>();
                this.JsonPackage.Details = new List<JsonPackageDetail>();
                this.MainShellTable.Items.ToList().ForEach(d => {

                    rowpanels = new List<JsonPackageLayoutPanel[]>();
                    foreach (var panels in d.ShellPanels.OrderBy(p => p.Columns).GroupBy(p => p.Row))
                    {
                        var columnpanels = new List<JsonPackageLayoutPanel>();
                        foreach (var panel in panels)
                        {
                            columnpanels.Add(panel);
                            panel.Items = d.JsonPackageItems
                            .Where(i => i.PanelIndex.HasValue && i.Panel == panel)
                            .Select(i => i.PanelIndex as Object)
                            .ToArray();
                        }
                        rowpanels.Add(columnpanels.ToArray());
                    }

                    var jsonPackageDetail = new JsonPackageDetail()
                    {
                        Items = d.JsonPackageItems.ToArray(),
                        Panels = rowpanels.ToArray(),
                        Actions = d.ShellActions.ToArray(),
                        StdActions = d.ShellStdActions.ToArray(),
                        Control = d.Control,
                        DataSource = d.DataSource,
                        Persist = d.PersistType,
                        Relation = d.Relation.TargetEntityName, // .TargetTableName,
                        Title = d.Title,
                        Fields = d.JsonPackageItems.Where(f => f.Filter != null).OrderBy(f=>f.Filter).Select(f=>Convert.ToInt32(f.Filter)).ToArray(),
                        
                    };
                    jsonPackageDetail.Items.ToList().ForEach(i=>
                    {
                        i.LangObj.LangObjDescr = i.TitleTranslation;
                        messages.Add(i.LangObj);
                    });
                    if (d.PackageDetail != null)
                        jsonPackageDetail.Panel = d.PackageDetail.Panel;
                    this.JsonPackage.Details.Add(jsonPackageDetail);

                    d.ShellActions.ToList().ForEach(a =>
                    {
                        a.LangObj.LangObjDescr = a.CaptionTranslation;
                        messages.Add(a.LangObj);
                    });
                    d.ShellPanels.ToList().ForEach(panel =>
                    {
                        panel.LangObj.LangObjDescr = panel.HeaderTranslation;
                        messages.Add(panel.LangObj);
                    });

                });
            }

            rowpanels = new List<JsonPackageLayoutPanel[]>();
            foreach (var panels in this.MainShellTable.ShellPanels.OrderBy(p => p.Columns).GroupBy(p => p.Row))
            {
                var columnpanels = new List<JsonPackageLayoutPanel>();
                foreach (var panel in panels)
                {
                    columnpanels.Add(panel);
                    var listPanelItems = this.MainShellTable.JsonPackageItems
                        .Where(i => i.PanelIndex.HasValue && i.Panel == panel)
                        .Select(i => i.PanelIndex as Object).ToList();
                    if (this.JsonPackage.Details != null)
                    {
                        this.JsonPackage.Details.ForEach(d =>
                        {
                            if (d.Panel == panel)
                            {
                                listPanelItems.Add(d.Relation);
                            }

                        });
                    }
                    panel.Items = listPanelItems.ToArray();
                }
                rowpanels.Add(columnpanels.ToArray());
            }
            this.JsonPackage.Header = new JsonPackageHeader()
            {
                Entity = this.MainShellTable.TableName,
                Items = this.MainShellTable.JsonPackageItems.ToArray(),
                Panels = rowpanels.ToArray(),
                Actions = this.MainShellTable.ShellActions.ToArray(),
                StdActions = this.MainShellTable.ShellStdActions.ToArray()
            };

            this.MainShellTable.ShellActions.ToList().ForEach(a =>
            {
                a.LangObj.LangObjDescr = a.CaptionTranslation;
                messages.Add(a.LangObj);
            });
            this.MainShellTable.ShellPanels.ToList().ForEach(panel =>
            {
                panel.LangObj.LangObjDescr = panel.HeaderTranslation;
                messages.Add(panel.LangObj);
            });

            this.JsonPackage.Header.Items.ToList().ForEach(i =>
            {
                i.LangObj.LangObjDescr = i.TitleTranslation;
                messages.Add(i.LangObj);
            });

            this.JsonPackage.Dependencies = this.ShellDependencies.Select(d => d.Type).ToArray();
            this.JsonPackage.Filter = new JsonPackageFilter();
            
            var conditionItems = this.JsonPackage.Header.Items
                .Where(i=>i.FilterConditionIndex.HasValue)
                .OrderBy(i => i.FilterConditionIndex).Select(i => {
                return i.FilterConditionIndex as Object;
            }).ToList();

            if (this.JsonPackage.Details != null)
            {
                this.JsonPackage.Details.ForEach(d =>
                {
                    d.Items.OrderBy(i => i.FilterConditionIndex)
                    .Where(i=>i.FilterConditionIndex.HasValue).ToList()
                    .ForEach(i =>
                        conditionItems.Add(new Object[] { d.Relation, d.Items.ToList().IndexOf(i) })
                    );

                });
            }
            this.JsonPackage.Filter.ConditionItems = conditionItems.ToArray();
            this.JsonPackage.Filter.ResultItems = this.JsonPackage.Header.Items
                .Where(i => i.FilterResultIndex.HasValue)
                .OrderBy(i => i.FilterResultIndex)
                .Select(i => i.FilterResultIndex.Value).ToArray();

            this.JsonPackage.Filter.FilterSource = this.Shell.FilterSource;
            this.JsonPackage.Filter.FilterPath = this.Shell.FilterPath;

            try
            {
                MemoryStream stream = new MemoryStream();
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(JsonPackage), new DataContractJsonSerializerSettings() { });
                ser.WriteObject(stream, this.JsonPackage);
                stream.Position = 0;
                StreamReader reader = new StreamReader(stream);
                String template = reader.ReadToEnd();
                this.Shell.Template = template;
                DAL.SaveShell(this.Shell);

                DAL.SaveMessages(messages);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        public IEnumerable<IResult> RefreshDbSchema()
        {
            yield return new BaseCoRoutine(delegate ()
            {
                this.ShellViewModel.IsLoadingShowed = true;
            });
            yield return new BackgroundCoRoutine(delegate ()
            {
                CommonDataSources.Instance.LoadData();
                this.LoadDbSchema();
            });
            yield return new BaseCoRoutine(delegate ()
            {
                this.ShellViewModel.IsLoadingShowed = false;
            });

        }
    }
}

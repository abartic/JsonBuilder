using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using JsonGenerator.JsonEntities;
using System.Windows;
using System.Windows.Threading;

namespace JsonGenerator.DbEntities
{
    class ShellTable : Caliburn.Micro.PropertyChangedBase, IShellTable
    {
        public ShellTable(JsonBuilderViewModel model, Table table, JsonPackageHeader header)
        {
            this.Model = model;
            this.table = table;
            if (header != null)
            {
                PackageHeader = header;
                this.JsonPackageItems = new Caliburn.Micro.BindableCollection<JsonPackageItem>(header.Items);
            }
            else
            {
                PackageHeader = new JsonPackageHeader();
                
            }
            ResetItems();
        }

        public ShellTable(JsonBuilderViewModel model, Relation relation, JsonPackageDetail detail)
        {
            this.Model = model;
            this.Relation = relation;
            this.table = DbSchema.Instance.Tables.FirstOrDefault(t => String.Compare(t.TableName, relation.TargetTableName) == 0);

            if (detail != null)
            {
                PackageDetail = detail;
                if (detail.Items != null)
                    this.JsonPackageItems = new Caliburn.Micro.BindableCollection<JsonPackageItem>(detail.Items);
                else
                    this.JsonPackageItems = new Caliburn.Micro.BindableCollection<JsonPackageItem>();
            }
            else
            {
                PackageDetail = new JsonPackageDetail();
                
            }

            ResetItems();
        }

        JsonPackageDetail packageDetail { get; set; }
        public JsonPackageDetail PackageDetail {
            get
            {
                return packageDetail;
            }
            set
            {
                packageDetail = value;
                NotifyOfPropertyChange(() => PackageDetail);
            }
        }

        JsonPackageHeader packageHeader { get; set; }
        public JsonPackageHeader PackageHeader
        {
            get
            {
                return packageHeader;
            }
            set
            {
                packageHeader = value;
                NotifyOfPropertyChange(() => PackageHeader);
            }
        }

        public Relation Relation { get; set; }
        public Caliburn.Micro.BindableCollection<ShellTable> Items { get; set; }
        public Caliburn.Micro.BindableCollection<Column> Columns { get; set; }
        public JsonBuilderViewModel Model { get; set; }

        public String Title { get; set; }
        public String Control { get; set; }
        public String PersistType{ get; set; }
        public String DataSource { get; set; }

        Caliburn.Micro.BindableCollection<JsonPackageItem> jsonPackageItems = null;
        public Caliburn.Micro.BindableCollection<JsonPackageItem> JsonPackageItems
        {
            get
            {
                return jsonPackageItems;
            }
            set
            {
                jsonPackageItems = value;
                NotifyOfPropertyChange(() => JsonPackageItems);
            }
        }

        public String TableName
        {
            get
            {
                return this.Relation != null ? this.Relation.TargetTableName : this.Table.TableName;
            }
        }

        public String EntityInfo
        {
            get
            {
                return this.Relation != null ? this.Relation.RelationProperty : this.Table.EntityName;
            }
        }

        Table table = null;
        public Table Table
        {
            get { return table; }
            set
            {
                
                var origValue = table;

                if (value == table)
                    return;

                
                table = value;

                if (MessageBox.Show("Allow change of selected item?", "Continue", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                {

                    Application.Current.Dispatcher.BeginInvoke(
                            new System.Action(() =>
                            {

                                table = origValue;
                                NotifyOfPropertyChange(() => Table);
                            }),
                            DispatcherPriority.ContextIdle, null);
                    return;
                }

               

                this.JsonPackageItems.Clear();
                this.JsonPackageItems = null;

                this.Relation = DbSchema.Instance.Relations.FirstOrDefault(r => String.Compare(r.TargetTableName, value.TableName, StringComparison.InvariantCultureIgnoreCase)==0);

                NotifyOfPropertyChange(() => Table);
                NotifyOfPropertyChange(() => TableName);
                NotifyOfPropertyChange(() => EntityInfo);
                                
                ResetItems();
            }
        }

     
        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    NotifyOfPropertyChange(()=>IsSelected);
                    if (isSelected)
                        Model.SelectedShellTable = this;
                }
            }
        }

        public void ResetItems()
        {
            this.Items = new Caliburn.Micro.BindableCollection<ShellTable>();

            //if (this.Model.IsMainTableSelected)
            //{
            //    var headerTable = Model.DbSchema.Tables
            //      .FirstOrDefault(t => String.Compare(t.TableName, this.Table.TableName, StringComparison.InvariantCultureIgnoreCase) == 0);

            //    if (headerTable != null)
            //    {

            //        this.Model.JsonPackage.Header.Entity = headerTable.TableName;
            //        this.Model.JsonPackage.Header.Panels = new JsonPackageLayoutPanel[0][];
            //        this.Model.JsonPackage.Header.StdActions = new JsonPackageAction[0];
            //    }
            //}
            //else
            //{
                //refresh relations for main table
                if (this.Relation == null)
                {
                    if (this.Model.JsonPackage.Details == null || this.Model.JsonPackage.Details.Any() == false)
                    {
                        Model.DbSchema.Relations
                          .Where(r => String.Compare(r.SourceTableName, this.Table.TableName, StringComparison.InvariantCultureIgnoreCase) == 0)
                          .ToList()
                          .ForEach(r =>
                          {
                              var detailShellTable = new ShellTable(Model, r, null);
                              this.Items.Add(detailShellTable);
                              detailShellTable.ShellStdActions = new BindableCollection<JsonPackageAction>(JsonPackage.DetailStdActions);
                              detailShellTable.ShellActions = new BindableCollection<JsonPackageAction>();
                          });


                        if (Model.ShellTables != null && Model.ShellTables.Any())
                        {
                            Model.RelationTables = new BindableCollection<Table>(Model.ShellTables.First().Items.Select(i => i.Table));
                        }

                        Model.IsMultiTable = this.Items.Any();
                    }
                }

                this.Columns = new Caliburn.Micro.BindableCollection<Column>(DbSchema.Instance.Columns.Where(c => c.TableName == this.Table.TableName));
                if (this.JsonPackageItems == null)
                {
                    this.JsonPackageItems = new BindableCollection<JsonPackageItem>();
                    this.Columns.ToList().ForEach(c => this.JsonPackageItems.Add(new JsonPackageItem() { Field = c.FieldName, Nullable = c.IsNullable, Type = c.PropertyType, Column = c, Decimals = c.Decimals }));
                }

                Boolean isHeader = this.Relation == null;
                Int32 index = 0;
                if (this.Model.JsonPackage.Filter != null)
                {
                    if (this.Model.JsonPackage.Filter.ConditionItems != null)
                    {
                        this.Model.JsonPackage.Filter.ConditionItems.ToList().ForEach(i =>
                        {
                            Int32 itemIndex = 0;
                            if (isHeader && i is Int32)
                            {
                                itemIndex = (Int32)i;
                                this.JsonPackageItems[itemIndex].FilterConditionIndex = index;
                            }
                            else if (this.Relation != null && (i is Int32) == false)
                            {
                                String relationName = ((Object[])i)[0] as String;
                                if (this.Relation.RelationProperty == relationName)
                                {
                                    itemIndex = (Int32)((Object[])i)[1];
                                    this.JsonPackageItems[itemIndex].FilterConditionIndex = index;
                                }
                            }
                            index++;
                        });
                    }

                    if (this.Model.JsonPackage.Filter.ResultItems != null)
                    {
                        isHeader = this.Relation == null;
                        index = 0;
                        this.Model.JsonPackage.Filter.ResultItems.ToList().ForEach(i =>
                        {
                            Int32 itemIndex = 0;
                            if (isHeader)
                            {
                                itemIndex = (Int32)i;
                                this.JsonPackageItems[itemIndex].FilterResultIndex = index;
                            }
                            index++;
                        });
                    }
                }

                if (this.Model.JsonPackage.Header != null && this.Model.JsonPackage.Header.Panels != null)
                {
                    foreach (var panels in this.Model.JsonPackage.Header.Panels)
                    {

                        foreach (var panel in panels)
                        {
                            int pindex = 0;
                            if (panel.Items == null) return;

                            foreach (Object objIndex in panel.Items)
                            {
                                if (objIndex is Int32)
                                {
                                    Int32 itemIndex = Convert.ToInt32(objIndex);
                                    this.Model.JsonPackage.Header.Items[itemIndex].Panel = panel;
                                    this.Model.JsonPackage.Header.Items[itemIndex].PanelIndex = pindex;
                                }
                                else
                                {
                                    String itemIndex = Convert.ToString(objIndex);
                                    this.Model.JsonPackage.Details.ToList().ForEach(d =>
                                    {
                                        d.Panel = panel;
                                    });
                                }
                                pindex++;
                            }

                        }
                    }
                }

                if (this.Model.JsonPackage.Details != null)
                {
                    this.Model.JsonPackage.Details.ForEach(d =>
                    {
                        if (d.Panels == null) return;

                        foreach (var panels in d.Panels)
                        {

                            foreach (var panel in panels)
                            {
                                int pindex = 0;
                                foreach (Object objIndex in panel.Items)
                                {
                                    if (objIndex is Int32)
                                    {
                                        Int32 itemIndex = Convert.ToInt32(objIndex);
                                        d.Items[itemIndex].Panel = panel;
                                        d.Items[itemIndex].PanelIndex = pindex;
                                    }

                                    pindex++;
                                }

                            }
                        }
                    });
                }
           // }
        }

        #region Std Actions
        public Caliburn.Micro.BindableCollection<JsonPackageAction> ShellStdActions { get; set; }

        JsonPackageAction selectedShellStdAction;
        public JsonPackageAction SelectedShellStdAction
        {
            get
            {
                return selectedShellStdAction;
            }
            set
            {
                selectedShellStdAction = value;
                NotifyOfPropertyChange(() => SelectedShellStdAction);
            }
        }

        public void AddShellStdAction()
        {
            ShellStdActions.Add(new JsonPackageAction());
        }

        public void DeleteShellStdAction()
        {
            if (this.SelectedShellStdAction != null)
                ShellStdActions.Remove(this.SelectedShellStdAction);
        }
        #endregion Std Actions

        #region Actions
        public Caliburn.Micro.BindableCollection<JsonPackageAction> ShellActions { get; set; }

        JsonPackageAction selectedShellAction;
        public JsonPackageAction SelectedShellAction
        {
            get
            {
                return selectedShellAction;
            }
            set
            {
                selectedShellAction = value;
                NotifyOfPropertyChange(() => SelectedShellAction);
            }
        }

        public void AddShellAction()
        {
            ShellActions.Add(new JsonPackageAction());
        }

        public void DeleteShellAction()
        {
            if (this.SelectedShellAction != null)
                ShellActions.Remove(this.SelectedShellAction);
        }
        #endregion Actions

        #region Panels
        Caliburn.Micro.BindableCollection<JsonPackageLayoutPanel> shellPanels;
        public Caliburn.Micro.BindableCollection<JsonPackageLayoutPanel> ShellPanels
        {

            get
            {
                return shellPanels;
            }
            set
            {
                shellPanels = value;
                NotifyOfPropertyChange(() => ShellPanels);
            }
        }

        JsonPackageLayoutPanel selectedShellPanel;
        public JsonPackageLayoutPanel SelectedShellPanel
        {
            get
            {
                return selectedShellPanel;
            }
            set
            {
                selectedShellPanel = value;
                NotifyOfPropertyChange(() => SelectedShellPanel);
            }
        }

        public void AddShellPanel()
        {
            ShellPanels.Add(new JsonPackageLayoutPanel() { Name = (ShellPanels.Count + 1).ToString() });
        }

        public void DeleteShellPanel()
        {
            if (this.SelectedShellAction != null)
                ShellPanels.Remove(this.SelectedShellPanel);
        }
        #endregion Panels
    }

    public interface IShellTable
    {
        String TableName { get; }
    }
}
